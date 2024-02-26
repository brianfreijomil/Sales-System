using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.interfaces;
using SistemaVenta.DAL.interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.implementacion
{
    public class MenuService : IMenuService
    {

        private readonly IGenericRepository<Menu> _menuRepository;
        private readonly IGenericRepository<RolMenu> _rolMenuRepository;
        private readonly IGenericRepository<Usuario> _userRepository;

        public MenuService(IGenericRepository<Menu> menuRepository, IGenericRepository<RolMenu> rolMenuRepository, IGenericRepository<Usuario> userRepository)
        {
            _menuRepository = menuRepository;
            _rolMenuRepository = rolMenuRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Menu>> GetMenus(int idUser)
        {
            IQueryable<Usuario> tbUser = await _userRepository.GetByFilter(u => u.IdUsuario == idUser);
            IQueryable<RolMenu> tbRolMenu = await _rolMenuRepository.GetByFilter();
            IQueryable<Menu> tbMenu = await _menuRepository.GetByFilter();

            //obtengo el menu padre relacionando al usuario con rolMenu, a rolMenu, con menus y esos menus, con su menupadre
            IQueryable<Menu> MenuFather = (from u in tbUser join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          join mpadre in tbMenu on m.IdMenuPadre equals mpadre.IdMenu
                                          select mpadre).Distinct().AsQueryable();
            //obtengo los menuhijos relacionando el usuario con rolmenu, el rolmenu con menus, y esos menus separandolos de los padres
            IQueryable<Menu> MenuChildren = (from u in tbUser
                                          join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          where m.IdMenu != m.IdMenuPadre
                                          select m).Distinct().AsQueryable();

            //creo la lista de menus padres con sus menus hijos
            List<Menu> menuList = (from mpadre in MenuFather
                                    select new Menu()
                                    {
                                        Descripcion = mpadre.Descripcion,
                                        Icono = mpadre.Icono,
                                        Controlador = mpadre.Controlador,
                                        PaginaAccion = mpadre.PaginaAccion,
                                        InverseIdMenuPadreNavigation = (from mhijo in MenuChildren where mhijo.IdMenuPadre == mpadre.IdMenu select mhijo).ToList()
                                    }).ToList();

            return menuList;
        }

        public async Task<bool> HavePermisionMenu(int idUser, string controller, string action)
        {

            IQueryable<Usuario> tbUser = await _userRepository.GetByFilter(u => u.IdUsuario == idUser);
            IQueryable<RolMenu> tbRolMenu = await _rolMenuRepository.GetByFilter();
            IQueryable<Menu> tbMenu = await _menuRepository.GetByFilter();

            //obtengo el menu padre relacionando al usuario con rolMenu, a rolMenu, con menus y esos menus, con su menupadre
            Menu MenuFound = (from u in tbUser
                                          join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          where m.Controlador == controller && m.PaginaAccion == action
                                          select m).FirstOrDefault();

            if(MenuFound == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }

}
