using HomeWeather.Data.Context;
using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace HomeWeather.Data.Infrastructure
{
    public class UnitOfWork<TEntity> : IUnitOfWork<TEntity> where TEntity : class, IEntity
    {
        private readonly HomeWeatherContext context;
        private bool disposed = false;

        public UnitOfWork(HomeWeatherContext context) => this.context = context;

        public DbContext db => throw new NotImplementedException();

        public void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                db.Dispose();
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRepository<TEntity> GetRepository() => new Repository<TEntity>(context);

        public void SaveChanges() => context.SaveChanges();
    }
}
