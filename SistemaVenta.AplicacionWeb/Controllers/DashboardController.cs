using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.CustomFilter;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashBoardService _dashBoardService;

        public DashboardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        [ClaimRequirement(controlador:"DashBoard",accion:"Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashBoard> gResponse = new GenericResponse<VMDashBoard>();

            try
            {
                VMDashBoard vmDashBoard = new VMDashBoard();

                vmDashBoard.TotalVentas = await _dashBoardService.TotalSalesLastWeek();
                vmDashBoard.TotalIngresos = await _dashBoardService.TotalIncomeLastWeek();
                vmDashBoard.TotalProductos = await _dashBoardService.TotalProducts();
                vmDashBoard.TotalCategorias = await _dashBoardService.TotalCategories();

                List<VMVentasSemana> listaVentasSemana = new List<VMVentasSemana>();
                List<VMProductosSemana> listaProductosSemana = new List<VMProductosSemana>();

                foreach(KeyValuePair<string,int> item in await _dashBoardService.SalesLastWeek())
                {
                    listaVentasSemana.Add(new VMVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }

                foreach (KeyValuePair<string, int> item in await _dashBoardService.ProductsTopLastWeek())
                {
                    listaProductosSemana.Add(new VMProductosSemana() 
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });
                }

                vmDashBoard.VentasUltimaSemana = listaVentasSemana;
                vmDashBoard.ProductosTopUltimaSemana = listaProductosSemana;

                gResponse.Estado = true;
                gResponse.Objeto = vmDashBoard;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK,gResponse);
        }
    }
}
