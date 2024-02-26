using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SistemaVenta.DAL.implementacion
{
    //la clase GenericRepository implementa la interfaz IGenericRepository
    //y le otorga todos los metodos del crud
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        //Referencia a la DB
        private readonly DbventaContext _dbContext;

        //Contructor
        public GenericRepository(DbventaContext dbContext) {
            _dbContext = dbContext;
        }

        //GET
        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                TEntity entity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(filter);
                return entity;
            }
            catch {
                throw;
            }
        }

        //POST
        public async Task<TEntity> Create(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Add(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch
            {
                throw;
            }
        }

        //PUT
        public async Task<bool> Update(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Update(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        //DELETE
        public async Task<bool> Delete(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Remove(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        //GET ALL BY
        public async Task<IQueryable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> queryEntity = filter == null ? _dbContext.Set<TEntity>() : _dbContext.Set<TEntity>().Where(filter);
            return queryEntity;
        }

        
    }
}
