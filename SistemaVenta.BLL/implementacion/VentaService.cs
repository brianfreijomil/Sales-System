using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.interfaces;
using SistemaVenta.DAL.interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.implementacion
{
    public class VentaService : IVentaService
    {

        private readonly IVentaRepository _repository;
        private readonly IGenericRepository<Producto> _productRepository;

        public VentaService(IVentaRepository repository, IGenericRepository<Producto> productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        public async Task<List<Producto>> GetProducts(string search)
        {
            IQueryable<Producto> query = await _productRepository.GetByFilter(
                p => p.EsActivo == true && 
                p.Stock > 0 && string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(search));

            return query.Include(p => p.IdCategoriaNavigation).ToList();
        }

        public async Task<Venta> Register(Venta entity)
        {
            try
            {
                return await _repository.Register(entity);
            }
            catch{
                throw;
            }
        }

        public async Task<List<Venta>> Record(string numberSale, string dateInit, string dateEnd)
        {
            IQueryable<Venta> query = await _repository.GetByFilter();
            dateInit = dateInit is null ? "" : dateInit;
            dateEnd = dateEnd is null ? "" : dateEnd;

            if(dateInit != "" && dateEnd != "")
            {
                DateTime dateInitParsed = DateTime.ParseExact(dateInit, "dd/MM/yyyy", new CultureInfo("es-AR"));
                DateTime dateEndParsed = DateTime.ParseExact(dateEnd, "dd/MM/yyyy", new CultureInfo("es-AR"));

                return query.Where(v =>
                    v.FechaRegistro.Value.Date >= dateInitParsed.Date &&
                    v.FechaRegistro.Value.Date <= dateEndParsed.Date)

                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }
            else
            {
                return query.Where(v => v.NumeroVenta == numberSale)

                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }
        }

        public async Task<Venta> Detail(string numberSale)
        {
            IQueryable<Venta> query = await _repository.GetByFilter(v => v.NumeroVenta == numberSale);
            return query
                .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Include(u => u.IdUsuarioNavigation)
                .Include(dv => dv.DetalleVenta)
                .First();
        }
        

        public async Task<List<DetalleVenta>> Report(string dateInit, string dateEnd)
        {
            DateTime dateInitParsed = DateTime.ParseExact(dateInit, "dd/MM/yyyy", new CultureInfo("es-AR"));
            DateTime dateEndParsed = DateTime.ParseExact(dateEnd, "dd/MM/yyyy", new CultureInfo("es-AR"));

            List<DetalleVenta> report = await _repository.Report(dateInitParsed, dateEndParsed);
            return report;
        }
    }
}
