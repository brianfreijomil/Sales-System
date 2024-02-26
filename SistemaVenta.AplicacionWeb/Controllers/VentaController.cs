using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.interfaces;
using SistemaVenta.Entity;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SistemaVenta.AplicacionWeb.Utilidades.CustomFilter;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ITipoDocumentoVentaService _tipoDocumentoVentaService;
        private readonly IVentaService _ventaService;
        private readonly IConverter _converter;


        public VentaController(IMapper mapper, ITipoDocumentoVentaService tipoDocumentoVentaService, IVentaService ventaService, IConverter converter)
        {
            _mapper = mapper;
            _ventaService = ventaService;
            _tipoDocumentoVentaService = tipoDocumentoVentaService;
            _converter = converter;
        }


        [ClaimRequirement(controlador: "Venta", accion: "NuevaVenta")]
        public IActionResult NuevaVenta()
        {
            return View();
        }

        [ClaimRequirement(controlador: "Venta", accion: "HistorialVenta")]
        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoVenta() {

            List<VMTipoDocumentoVenta> vmListaTipoDocumentoVenta = _mapper.Map<List<VMTipoDocumentoVenta>>(await _tipoDocumentoVentaService.List());

            return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumentoVenta);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string search)
        {
            List<VMProducto> vmProductsList = _mapper.Map<List<VMProducto>>(await _ventaService.GetProducts(search));

            return StatusCode(StatusCodes.Status200OK, vmProductsList);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody]VMVenta model)
        {
            GenericResponse<VMVenta> gResponse = new GenericResponse<VMVenta>();

            try
            {

                ClaimsPrincipal claimsUser = HttpContext.User;

                string idUser = claimsUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                model.IdUsuario = int.Parse(idUser);

                Venta saleCreated = await _ventaService.Register(_mapper.Map<Venta>(model));
                model = _mapper.Map<VMVenta>(saleCreated);

                gResponse.Estado = true;
                gResponse.Objeto = model;

            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Historial(string numberSale, string dateInit, string dateEnd)
        {
            List<VMVenta> vmHistorialVenta = _mapper.Map<List<VMVenta>>(await _ventaService.Record(numberSale, dateInit, dateEnd));

            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }

        [HttpGet]
        public IActionResult MostrarPDFVenta(string numberSale)
        {
            string urlTemplateView = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFVenta?numeroVenta={numberSale}";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page = urlTemplateView
                    }
                }
            };

            var filePDF = _converter.Convert(pdf);
            return File(filePDF, "application/pdf");
        }
    }
}
