using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;
using SistemaVenta.BLL.interfaces;
using SistemaVenta.DAL.interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.implementacion
{
    public class UsuarioService : IUsuarioService
    {

        private readonly IGenericRepository<Usuario> _repository;
        private readonly IFireBaseService _firebaseService;
        private readonly IUtilidades _utilities;
        private readonly ICorreoService _mailService;

        public UsuarioService(IGenericRepository<Usuario> repository, IFireBaseService firebaseService, IUtilidades utilities, 
            ICorreoService mailService)
        {
            _repository = repository;
            _firebaseService = firebaseService;
            _utilities = utilities;
            _mailService = mailService;
        }

        //obtengo todos los usuarios 
        public async Task<List<Usuario>> List()
        {
            IQueryable<Usuario> query = await _repository.GetByFilter();
            return query.Include(r => r.IdRolNavigation).ToList();
        }

        //crea un usuario
        public async Task<Usuario> Create(Usuario entity, Stream Photo = null, string NamePhoto = "", string UrlTemplateMail = "")
        {
            //busco usuario por correo
            IQueryable<Usuario> queryUserExist = await _repository.GetByFilter(u => u.Correo == entity.Correo);
            //chequeo si ya existe
            if (queryUserExist.Any())
                throw new TaskCanceledException("El correo ya existe");
            
            //en caso de que no exista un correo igual sigo
            try
            {
                //genero clave y la encripto
                string passwordGenerated = _utilities.GeneratePassword();
                entity.Clave = _utilities.ConvertSha256(passwordGenerated);
                entity.NombreFoto = NamePhoto;

                //si hay foto la persisto en firebase
                if(Photo != null)
                {
                    string urlPhoto = await _firebaseService.UploadStorage(Photo, "carpeta_usuario", NamePhoto);
                    entity.UrlFoto = urlPhoto;
                }

                //creo usuario
                Usuario userCreated = await _repository.Create(entity);

                //chequeo que se haya creado
                if (userCreated.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario");

                //
                if (UrlTemplateMail != "")
                {
                    UrlTemplateMail = UrlTemplateMail.Replace("[correo]", userCreated.Correo).Replace("[clave]", passwordGenerated);


                    string htmlMail = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlTemplateMail);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                                readerStream = new StreamReader(dataStream);
                            else
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            htmlMail = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();



                        }
                    }

                    if (htmlMail != "")
                        await _mailService.SendMail(userCreated.Correo, "Cuenta creada con exito", htmlMail);
                }
                //retorno usuario creado
                IQueryable<Usuario> query = await _repository.GetByFilter(u => u.IdUsuario == userCreated.IdUsuario);
                userCreated = query.Include(r => r.IdRolNavigation).First();

                return userCreated;

            }
            catch {

                throw;

            }
        }


        public async Task<Usuario> Update(Usuario entity, Stream Photo = null, string NamePhoto = "")
        {
            //se quiere cambir el correo entonces busco que no exista un correo igual al que mande!!!
            IQueryable<Usuario> queryUserExist = await _repository.GetByFilter(u => u.Correo == entity.Correo && u.IdUsuario != entity.IdUsuario);
            //chequeo si ya existe un usuario con ese correo
            if (queryUserExist.Any())
                throw new TaskCanceledException("El correo ya existe");

            try
            {
                IQueryable<Usuario> queryUser = await _repository.GetByFilter(u => u.IdUsuario == entity.IdUsuario);

                Usuario userEdit = queryUser.First();

                userEdit.Nombre = entity.Nombre;
                userEdit.Correo = entity.Correo;
                userEdit.Telefono = entity.Telefono;
                userEdit.IdRol = entity.IdRol;
                userEdit.EsActivo = entity.EsActivo;

                if (userEdit.NombreFoto == "")
                    userEdit.NombreFoto = NamePhoto;

                if (Photo != null)
                {
                    string urlPhoto = await _firebaseService.UploadStorage(Photo, "carpeta_usuario", userEdit.NombreFoto);
                    userEdit.UrlFoto = urlPhoto;
                }

                bool result = await _repository.Update(userEdit);

                if (!result)
                    throw new TaskCanceledException("No se pudo modificar el usuario");

                Usuario userEdited = queryUser.Include(r => r.IdRolNavigation).First();
                return userEdited;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Delete(int IdUser)
        {
            try
            {
                IQueryable<Usuario> queryUser = await _repository.GetByFilter(u => u.IdUsuario == IdUser);
                if (!queryUser.Any()) {
                    throw new TaskCanceledException("El usuario no existe");
                }

                Usuario userFound = queryUser.First();

                string namePhoto = userFound.NombreFoto;
                bool result = await _repository.Delete(userFound);

                if (result)
                    await _firebaseService.DeleteStorage("carpeta_usuario", namePhoto);

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Usuario> GetByCredentials(string Mail, string password)
        {
            string encryptedPassword = _utilities.ConvertSha256(password);

            IQueryable<Usuario> queryUserFound = await _repository.GetByFilter(u => u.Correo.Equals(Mail) && u.Clave.Equals(encryptedPassword));

            if(!queryUserFound.Any())
            {
                return null;
            }
            else
            {
                Usuario userFound = queryUserFound.First();

                return userFound;
            }

        }

        public async Task<Usuario> GetById(int IdUser)
        {
            IQueryable<Usuario> query = await _repository.GetByFilter(u => u.IdUsuario == IdUser);

            Usuario result = query.Include(r => r.IdRolNavigation).FirstOrDefault();

            return result;
        }

        public async Task<bool> SaveProfile(Usuario entity)
        {
            try
            {
                IQueryable<Usuario> queryUserFound = await _repository.GetByFilter(u => u.IdUsuario == entity.IdUsuario);

                if (!queryUserFound.Any())
                    throw new TaskCanceledException("El usuario no existe");


                Usuario userFound = queryUserFound.First();

                userFound.Correo = entity.Correo;
                userFound.Telefono = entity.Telefono;

                bool result = await _repository.Update(userFound);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdatePassword(int IdUser, string CurrentPassword, string NewPassword)
        {
            try
            {
                IQueryable<Usuario> queryUserFound = await _repository.GetByFilter(u => u.IdUsuario == IdUser);

                if (!queryUserFound.Any())
                    throw new TaskCanceledException("El usuario no existe");


                Usuario userFound = queryUserFound.First();

                if (userFound.Clave != _utilities.ConvertSha256(CurrentPassword))
                    throw new TaskCanceledException("La contraseña ingresada como actual no es correcta");

                userFound.Clave = _utilities.ConvertSha256(NewPassword);

                bool result = await _repository.Update(userFound);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ResetPassword(string Mail, string UrlTemplateMail)
        {
            try
            {
                IQueryable<Usuario> queryUserFound = await _repository.GetByFilter(u => u.Correo == Mail);

                if (!queryUserFound.Any())
                    throw new TaskCanceledException("No encontramos ningun usuario asociado al correo");

                Usuario userFound = queryUserFound.First();


                string passwordGenerated = _utilities.GeneratePassword();
                userFound.Clave = _utilities.ConvertSha256(passwordGenerated);

                UrlTemplateMail = UrlTemplateMail.Replace("[clave]", passwordGenerated);


                string htmlMail = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlTemplateMail);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader readerStream = null;

                        if (response.CharacterSet == null)
                            readerStream = new StreamReader(dataStream);
                        else
                            readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                        htmlMail = readerStream.ReadToEnd();
                        response.Close();
                        readerStream.Close();



                    }
                }

                //debe estar al pedo
                bool mailSent = false;

                if (htmlMail != "")
                    await _mailService.SendMail(Mail, "Contraseña Restablecida", htmlMail);

                bool result = await _repository.Update(userFound);

                return result;
            }
            catch
            {
                throw;
            }
        }

    }
}
