using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.interfaces
{
    public interface IMenuService
    {

        Task<List<Menu>> GetMenus(int idUser);

        Task<bool> HavePermisionMenu(int idUser, string controller, string action);

    }
}
