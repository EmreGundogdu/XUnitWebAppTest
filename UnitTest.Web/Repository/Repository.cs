using Microsoft.EntityFrameworkCore;
using UnitTest.Web.Models;

namespace UnitTest.Web.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly UnitTestDbContext _context;
        public Repository(UnitTestDbContext context)
        {
            _context = context;
        }

        public async Task Create(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            //_context.Update(entity);
            _context.SaveChanges();
        }
    }
}
