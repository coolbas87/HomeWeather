using HomeWeather.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace HomeWeather.Data.Interfaces
{
    public interface IUnitOfWork<TEntity> : IDisposable where TEntity : class, IEntity
    {
        DbContext db { get; }

        IRepository<TEntity> GetRepository();

        void SaveChanges();
    }
}
