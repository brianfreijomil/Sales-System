using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.interfaces;
using SistemaVenta.DAL.interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.implementacion
{
    public class TipoDocumentoVentaService : ITipoDocumentoVentaService
    {

        private readonly IGenericRepository<TipoDocumentoVenta> _repository;

        public TipoDocumentoVentaService(IGenericRepository<TipoDocumentoVenta> repository)
        {
            _repository = repository;
        }

        public async Task<List<TipoDocumentoVenta>> List()
        {
            IQueryable<TipoDocumentoVenta> query = await _repository.GetByFilter();
            return query.ToList();
        }
    }
}
