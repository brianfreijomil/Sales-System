using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.BLL.interfaces
{
    public interface ICategoriaService
    {

        Task<List<Categoria>> List();

        Task<Categoria> Create(Categoria entity);

        Task<Categoria> Update(Categoria entity);

        Task<bool> Delete(int idCategory);


    }
}
