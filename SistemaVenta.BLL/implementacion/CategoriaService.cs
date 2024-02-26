using SistemaVenta.BLL.interfaces;
using SistemaVenta.Entity;
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
    public class CategoriaService : ICategoriaService
    {

        private readonly IGenericRepository<Categoria> _repository;

        public CategoriaService(IGenericRepository<Categoria> repository)
        {
            _repository = repository;
        }

        public async Task<List<Categoria>> List()
        {
            IQueryable<Categoria> query = await _repository.GetByFilter();
            return query.ToList();
        }

        public async Task<Categoria> Create(Categoria entity)
        {
            try
            {
                Categoria categoryCreated = await _repository.Create(entity);

                if (categoryCreated.IdCategoria == 0)
                {
                    throw new TaskCanceledException("No se pudo crear la categoria");
                }

                return categoryCreated;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Categoria> Update(Categoria entity)
        {
            try
            {
                IQueryable<Categoria> queryCategoryFound = await _repository.GetByFilter(c => c.IdCategoria == entity.IdCategoria);

                if(!queryCategoryFound.Any())
                {
                    throw new TaskCanceledException("No se ha encontrado la categoria");
                }

                Categoria categoryExisting = queryCategoryFound.First();

                categoryExisting.Descripcion = entity.Descripcion;
                categoryExisting.EsActivo = entity.EsActivo;

                bool result = await _repository.Update(categoryExisting);

                if (!result)
                    throw new TaskCanceledException("No se pudo modificar la categoria");

                return categoryExisting;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Delete(int idCategory)
        {
            try
            {
                IQueryable<Categoria> queryCategoryFound = await _repository.GetByFilter(c => c.IdCategoria == idCategory);

                if (!queryCategoryFound.Any())
                {
                    throw new TaskCanceledException("No se ha encontrado la categoria");
                }

                Categoria categoryExist = queryCategoryFound.First();

                bool result = await _repository.Delete(categoryExist);
                return result;
            }
            catch
            {
                throw;
            }
        }

    }
}
