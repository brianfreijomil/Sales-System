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
    public class RolService : IRolService
    {

        private IGenericRepository<Rol> _repository;


        public RolService(IGenericRepository<Rol> repository)
        {
            _repository = repository;
        }

        public async Task<List<Rol>> List()
        {
            IQueryable<Rol> query = await _repository.GetByFilter();

            return query.ToList();
        }
    }
}
