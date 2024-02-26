using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.DAL.interfaces
{
    public interface IVentaRepository : IGenericRepository<Venta>
    {
        Task<Venta> Register(Venta entity);
        Task<List<DetalleVenta>> Report(DateTime dateInit, DateTime dateEnd);
    }
}
