using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;
using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public HomeController(IUsuarioService usuarioService, IMapper mapper)
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        public IActionResult PermisoDenegado()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimsUser = HttpContext.User;

                string idUsuario = claimsUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                VMUsuario vmUsuario = _mapper.Map<VMUsuario> (await _usuarioService.GetById(int.Parse(idUsuario)));

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody]VMUsuario model)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimsUser = HttpContext.User;

                string idUsuario = claimsUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Usuario entity = _mapper.Map<Usuario>(model);
                entity.IdUsuario = int.Parse(idUsuario);

                bool result = await _usuarioService.SaveProfile(entity);

                gResponse.Estado = result;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave model)
        {
            GenericResponse<bool> gResponse = new GenericResponse<bool>();

            try
            {
                ClaimsPrincipal claimsUser = HttpContext.User;

                string idUsuario = claimsUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();


                bool result = await _usuarioService.UpdatePassword(
                    int.Parse(idUsuario), 
                    model.ClaveActual, 
                    model.ClaveNueva);

                gResponse.Estado = result;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return RedirectToAction("Login", "Acceso");
        }
    }
}
