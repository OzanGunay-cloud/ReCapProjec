using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks; // Task yapısı için gerekli
using Core.DataAccess.Abstract;
using Core.Entities; // IEntity referansı için
using Core.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Core.DataAccess
{
    /// <summary>
    /// Tüm tablolar için ortak asenkron veri erişim operasyonlarını içerir.
    /// </summary>
    /// <typeparam name="TEntity">Veritabanı tablosu (Car, Brand vb.)</typeparam>
    /// <typeparam name="TContext">Bağlantı nesnesi (ReCapContext)</typeparam>
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext
    {
        // Dependency Injection ile dışarıdan gelen context nesnesi
        private readonly TContext _context;

        public EfEntityRepositoryBase(TContext context)
        {
            _context = context;
        }

        // Task: Geriye bir şey dönmeyen (void) metotların asenkron karşılığıdır.
        public async Task AddAsync(TEntity entity)
        {
            // Veriyi takip listesine (Change Tracker) ekler.
            var addedEntity = _context.Entry(entity);

            addedEntity.State = EntityState.Added;

            // SaveChangesAsync: Veritabanına yazma işlemini asenkron yapar, thread'i bloklamaz.
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            var deletedEntity = _context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var updatedEntity = _context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Tek bir nesne getirmek için SingleOrDefaultAsync kullanılır.
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _context.Set<TEntity>().SingleOrDefaultAsync(filter);
        }

        // Liste dönerken ToListAsync kullanarak veritabanı cevabını asenkron bekleriz.
        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null
                ? await _context.Set<TEntity>().ToListAsync()
                : await _context.Set<TEntity>().Where(filter).ToListAsync();
        }
    }
}