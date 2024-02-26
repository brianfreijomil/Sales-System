using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace SistemaVenta.DAL.interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Get(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> Create(TEntity entity);
        Task<bool> Update(TEntity entity);
        Task<bool> Delete(TEntity entity);

        //enrealidad seraia un GetBy()
        Task<IQueryable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> filter = null);
    }
}
