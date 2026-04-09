using Application.Common.Interfaces.General;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // الـ AnyAsync هنا ستستخدم الـ Global Query Filter تلقائياً
            // لأنها تعمل من خلال الـ DbContext المفلتر
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // تمرير CancellationToken يتطلب استخدام مصفوفة
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        // تم تعديل الدالة لتقبل CancellationToken
        // تحذير: تجنب استخدام هذه الدالة للجداول الكبيرة، يفضل استخدام تقنيات Pagination
        public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}