using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.interfaces;
using SistemaVenta.Entity;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using SistemaVenta.AplicacionWeb.Utilidades.CustomFilter;

namespace SistemaVenta.AplicacionWeb.Controllers

{
    [Authorize]
    public class CategoriaController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(IMapper mapper, ICategoriaService categoriaService)
        {
            _mapper = mapper;
            _categoriaService = categoriaService;
            
        }


        [ClaimRequirement(controlador: "Categoria", accion: "Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMCategoria> vmCategoriaLista = _mapper.Map<List<VMCategoria>> (await _categoriaService.List());

            return StatusCode(StatusCodes.Status200OK, new { data = vmCategoriaLista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMCategoria modelo)
        {

            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {
                Categoria categoriaCreada = await _categoriaService.Create(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VMCategoria>(categoriaCreada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status201Created, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMCategoria modelo)
        {
            GenericResponse<VMCategoria> gResponse = new GenericResponse<VMCategoria>();

            try
            {
                Categoria categoriaEditada = await _categoriaService.Update(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VMCategoria>(categoriaEditada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdCategoria)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _categoriaService.Delete(IdCategoria);
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
