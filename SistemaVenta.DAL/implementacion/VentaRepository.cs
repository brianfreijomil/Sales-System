using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {

        private readonly DbventaContext _dbventaContext;

        public VentaRepository(DbventaContext dbventaContext):base(dbventaContext)
        {
            _dbventaContext = dbventaContext;
        }

        public async Task<Venta> Register(Venta entity)
        {
            Venta saleGenerated = new Venta();

            using (var transaction = _dbventaContext.Database.BeginTransaction())
            {
                try
                {
                    foreach(DetalleVenta detailSale in entity.DetalleVenta)
                    {
                        Producto productFound = _dbventaContext.Productos.Where(p => p.IdProducto == detailSale.IdProducto).First();

                        productFound.Stock = productFound.Stock - detailSale.Cantidad;
                        _dbventaContext.Productos.Update(productFound);
                    }

                    await _dbventaContext.SaveChangesAsync();

                    NumeroCorrelativo correlativo = _dbventaContext.NumeroCorrelativos.Where(n => n.Gestion == "venta").First();

                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _dbventaContext.NumeroCorrelativos.Update(correlativo);
                    await _dbventaContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entity.NumeroVenta = numeroVenta;

                    await _dbventaContext.Venta.AddAsync(entity);
                    await _dbventaContext.SaveChangesAsync();

                    saleGenerated = entity;

                    transaction.Commit();

                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return saleGenerated;
        }

        public async Task<List<DetalleVenta>> Report(DateTime dateInit, DateTime dateEnd)
        {
            List<DetalleVenta> listDetails = await _dbventaContext.DetalleVenta
            .Include(v => v.IdVentaNavigation)
            .ThenInclude(u => u.IdUsuarioNavigation)
            .Include(v => v.IdVentaNavigation)
            .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
            .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= dateInit.Date &&
            dv.IdVentaNavigation.FechaRegistro.Value.Date <= dateEnd.Date).ToListAsync();

            return listDetails;
        }
    }
}
