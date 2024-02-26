using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _inegocioService;
        private readonly IVentaService _ventaService;

        public PlantillaController(IMapper mapper, INegocioService inegocioService, IVentaService ventaService)
        {
            _mapper = mapper;
            _inegocioService = inegocioService;
            _ventaService = ventaService;
        }


        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";


            return View();
        }

        public async Task<IActionResult> PDFVenta(string numeroVenta)
        {
            VMVenta vmVenta = _mapper.Map<VMVenta>(await _ventaService.Detail(numeroVenta));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _inegocioService.Get());

            VMPDFVenta modelo = new VMPDFVenta();

            modelo.Negocio = vmNegocio;
            modelo.Venta = vmVenta;


            return View(modelo);
        }

        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;

            return View();
        }
    }
}
