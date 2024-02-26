using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.interfaces;
using SistemaVenta.BLL.implementacion;
using Microsoft.AspNetCore.Authorization;
using SistemaVenta.AplicacionWeb.Utilidades.CustomFilter;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IVentaService _ventaService;

        public ReportesController(IMapper mapper, IVentaService ventaService)
        {
            _mapper = mapper;
            _ventaService = ventaService;
        }


        [ClaimRequirement(controlador: "Reportes", accion: "Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReporteVenta(string fechaInicio, string fechaFin)
        {
            List<VMReporteVenta> vmLista = _mapper.Map<List<VMReporteVenta>> (await _ventaService.Report(fechaInicio, fechaFin));
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }
    }
}
