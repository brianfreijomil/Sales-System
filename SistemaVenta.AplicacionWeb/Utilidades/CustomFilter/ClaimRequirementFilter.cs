
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SistemaVenta.BLL.interfaces;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Utilidades.CustomFilter
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {

        private string _controlador;
        private string _action;
        private IMenuService _menuService;

        public ClaimRequirementFilter(string controlador, string action, IMenuService menuService)
        {
            _controlador = controlador;
            _action = action;
            _menuService = menuService;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            ClaimsPrincipal claimsUser = context.HttpContext.User;

            string idUsuario = claimsUser.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault();

            bool tienePermiso = await _menuService.HavePermisionMenu(int.Parse(idUsuario), _controlador, _action);

            if(!tienePermiso)
            {
                context.Result = new RedirectToActionResult("PermisoDenegado","Home",null);
            }
        }
    }
}
