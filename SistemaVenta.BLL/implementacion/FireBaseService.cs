using SistemaVenta.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.interfaces;
using Firebase.Auth;
using Firebase.Storage;
using SistemaVenta.Entity;
using SistemaVenta.DAL.interfaces;

namespace SistemaVenta.BLL.implementacion
{
    public class FireBaseService : IFireBaseService
    {

        private readonly IGenericRepository<Configuracion> _repository;

        public FireBaseService(IGenericRepository<Configuracion> repository)
        {
            _repository = repository;
        }

        public async Task<string> UploadStorage(Stream StreamFile, string FolderDestination, string NameFile)
        {
            string UrlImage = "";

            try
            {
                IQueryable<Configuracion> query = await _repository.GetByFilter(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
               )
                    .Child(Config[FolderDestination])
                    .Child(NameFile)
                    .PutAsync(StreamFile, cancellation.Token);

                UrlImage = await task;
                
            }
            catch
            {
                UrlImage = "";

            }
            return UrlImage;
        }

        public async Task<bool> DeleteStorage(string FolderDestination, string NameFile)
        {
            try
            {
                IQueryable<Configuracion> query = await _repository.GetByFilter(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
               )
                    .Child(Config[FolderDestination])
                    .Child(NameFile)
                    .DeleteAsync();

                await task;

                return true;

            }
            catch
            {
                return false;

            }
        }

 
    }
}
