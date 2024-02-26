using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.BLL.interfaces
{
    public interface IVentaService
    {

        Task<List<Producto>> GetProducts(string search);

        Task<Venta> Register(Venta entity);

        Task<List<Venta>> Record(string numberSale, string dateInit, string dateEnd);

        Task<Venta> Detail(string numberSale);

        Task<List<DetalleVenta>> Report(string dateInit, string dateEnd);
    }
}
