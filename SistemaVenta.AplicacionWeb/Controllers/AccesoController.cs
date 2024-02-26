using Microsoft.AspNetCore.Mvc;

using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {

        private readonly IUsuarioService _usuarioService;

        public AccesoController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Login()
        {

            ClaimsPrincipal claimUser = HttpContext.User;

            if(claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        public IActionResult RestablecerClave()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMUsuarioLogin modelo)
        {

            try
            {
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";

                bool result = await _usuarioService.ResetPassword(modelo.Correo, urlPlantillaCorreo);

                if (result)
                {
                    ViewData["Mensaje"] = "Listo, su contraseña fue restablecida. Revise su correo.";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["MensajeError"] = "Tenemos problemas. Por favor intente de nuevo mas tarde.";
                    ViewData["Mensaje"] = null;
                }
            }
            catch(Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
                ViewData["Mensaje"] = null;
            }

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin modelo)
        {

            Usuario userFound = await _usuarioService.GetByCredentials(modelo.Correo, modelo.Clave);

            if (userFound == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            ViewData["Mensaje"] = null;

            List<Claim> claimList = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userFound.Nombre),
                new Claim(ClaimTypes.NameIdentifier, userFound.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, userFound.IdRol.ToString()),
                new Claim("UrlFoto", userFound.UrlFoto),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claimList,CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesion,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), properties);


            return RedirectToAction("Index", "Home");
        }
    }
}
