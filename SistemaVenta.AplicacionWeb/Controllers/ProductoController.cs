using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.BLL.interfaces;
using SistemaVenta.Entity;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using NuGet.Protocol.Core.Types;
using SistemaVenta.BLL.implementacion;
using Microsoft.AspNetCore.Authorization;
using SistemaVenta.AplicacionWeb.Utilidades.CustomFilter;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {

        private readonly IProductoService _productoService;
        private readonly IMapper _mapper;

        public ProductoController(IProductoService productoService, IMapper mapper)
        {
            _productoService = productoService;
            _mapper = mapper;
        }

        [ClaimRequirement(controlador: "Producto", accion: "Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _productoService.List());
            return StatusCode(StatusCodes.Status200OK, new {data = vmListaProductos});
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();
            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreFoto = "";

                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Producto productoCreado = await _productoService.Create(_mapper.Map<Producto>(vmProducto), fotoStream, nombreFoto);

                vmProducto = _mapper.Map<VMProducto>(productoCreado);

                gResponse.Estado = true;
                gResponse.Objeto = vmProducto;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();
            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Producto productoEditado = await _productoService.Update(_mapper.Map<Producto>(vmProducto), fotoStream, nombreFoto);

                vmProducto = _mapper.Map<VMProducto>(productoEditado);

                gResponse.Estado = true;
                gResponse.Objeto = vmProducto;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdProducto)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _productoService.Delete(IdProducto);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
