using HomeWeather.Data.Context;
using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Data.Infrastructure
{
    public sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly HomeWeatherContext context;
        private readonly DbSet<TEntity> dbEntities;

        public Repository(HomeWeatherContext context)
        {
            this.context = context;
            dbEntities = this.context.Set<TEntity>();
        }

        public TEntity Add(TEntity entity) => dbEntities.Add(entity).Entity;

        public void AddRange(IEnumerable<TEntity> entities) => dbEntities.AddRange(entities);

        public bool Delete(TEntity entity) => dbEntities.Remove(entity).Entity != null;

        public void DeleteRange(IEnumerable<TEntity> entities) => dbEntities.RemoveRange(entities);

        public TEntity GetById(params object[] keys) => dbEntities.Find(keys);

        public IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes)
        {
            var dbSet = context.Set<TEntity>();
            IQueryable<TEntity> query = dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query ?? dbSet;
        }

        public TEntity Update(TEntity entity) => dbEntities.Update(entity).Entity;
    }
}
