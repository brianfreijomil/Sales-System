using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.BLL.interfaces
{
    public interface IProductoService
    {

        Task<List<Producto>> List();

        Task<Producto> Create(Producto entity, Stream img = null, string NameImg = "");

        Task<Producto> Update(Producto entity, Stream img = null, string NameImg = "");

        Task<bool> Delete(int IdProduct);
    }
}
