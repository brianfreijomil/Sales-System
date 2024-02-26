using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.interfaces
{
    public interface IUsuarioService
    {
        Task<List<Usuario>> List();

        Task<Usuario> Create(Usuario entity, Stream Photo = null, string NamePhoto = "", string UrlTemplateMail = "");

        Task<Usuario> Update(Usuario entity, Stream Photo = null, string NamePhoto = "");

        Task<bool> Delete(int IdUser);

        Task<Usuario> GetByCredentials(string Mail, string Password);

        Task<Usuario> GetById(int IdUser);

        Task<bool> SaveProfile(Usuario entity);

        Task<bool> UpdatePassword(int IdUser, string currentPassword, string NewPassword);

        Task<bool> ResetPassword(string Mail, string UrlTemplateMail);
    }
}
