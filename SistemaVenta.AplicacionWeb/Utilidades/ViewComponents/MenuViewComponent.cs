using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.interfaces;

namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {

        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;

        public MenuViewComponent(IMenuService menuService, IMapper mapper)
        {
            _menuService = menuService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
            List<VMMenu> listaMenus;

            if(claimsUser.Identity.IsAuthenticated)
            {
                string idUsuario = claimsUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                listaMenus = _mapper.Map<List<VMMenu>>(await _menuService.GetMenus(int.Parse(idUsuario)));
            }
            else
            {
                listaMenus = new List<VMMenu>();
            }

            return View(listaMenus);
        }

    }
}
