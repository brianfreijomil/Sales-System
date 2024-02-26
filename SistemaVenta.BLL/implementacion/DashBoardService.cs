using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.interfaces;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.interfaces;
using SistemaVenta.Entity;
using System.Globalization;

namespace SistemaVenta.BLL.implementacion
{
    public class DashBoardService : IDashBoardService
    {

        private readonly IVentaRepository _saleRepository;
        private readonly IGenericRepository<DetalleVenta> _detailSaleRepository;
        private readonly IGenericRepository<Categoria> _categoryRepository;
        private readonly IGenericRepository<Producto> _productRepository;
        private DateTime DateInit = DateTime.Now;

        public DashBoardService(IVentaRepository ventaRepository, IGenericRepository<DetalleVenta> detalleVentaRepository,
            IGenericRepository<Categoria> categoriaRepository, IGenericRepository<Producto> productoRepository)
        {
            _saleRepository = ventaRepository;
            _categoryRepository = categoriaRepository;
            _productRepository = productoRepository;
            _detailSaleRepository = detalleVentaRepository;
            DateInit = DateInit.AddDays(-7); //fecha actual menos 7 dias, para manejar la ultima semana hasta hoy
        }

        public async Task<int> TotalSalesLastWeek()
        {
            try
            {
                IQueryable<Venta> query = await _saleRepository.GetByFilter(v => v.FechaRegistro.Value.Date >= DateInit.Date);
                int total = query.Count();
                return total;
            }
            catch {
                throw;
            }
        }

        public async Task<string> TotalIncomeLastWeek()
        {
            try
            {
                IQueryable<Venta> query = await _saleRepository.GetByFilter(v => v.FechaRegistro.Value.Date >= DateInit.Date);
                decimal total = query.Select(v => v.Total).Sum(v => v.Value);
                return Convert.ToString(total, new CultureInfo("es-AR"));
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalProducts()
        {
            try
            {
                IQueryable<Producto> query = await _productRepository.GetByFilter();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalCategories()
        {
            try
            {
                IQueryable<Categoria> query = await _categoryRepository.GetByFilter();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> SalesLastWeek()
        {
            try
            {
                IQueryable<Venta> query = await _saleRepository.GetByFilter(v => v.FechaRegistro.Value.Date >= DateInit.Date);

                Dictionary<string, int> result = query
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> ProductsTopLastWeek()
        {
            try
            {
                IQueryable<DetalleVenta> query = await _detailSaleRepository.GetByFilter();

                Dictionary<string, int> result = query
                    .Include(v => v.IdVentaNavigation)
                    .Where(v => v.IdVentaNavigation.FechaRegistro.Value.Date >= DateInit.Date)
                    .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count())
                    .Select(dv => new { fecha = dv.Key, total = dv.Count() }).Take(4)
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);

                return result;
            }
            catch
            {
                throw;
            }
        }
        
    }
}
