using Ergon.Interfaces;
using Ergon.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ergon.Repositories
{
    public class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected ErgonContext _context;

        public Repository(ErgonContext context)
        {
            _context = context;
        }

        // Create
        public async Task<T> Create(T item)
        {
            _context.Add(item);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Failed to create item.", ex);
            }
            return item;
        }

        // Get
        public async Task<T?> Get(K key)
        {
            return await _context.FindAsync<T>(key);
        }

        // GetAll
        public async Task<List<T>?> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // Update
        public async Task<T?> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem == null)
                throw new KeyNotFoundException($"No item found with key: {key}");

            _context.Entry(myItem).CurrentValues.SetValues(item);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbUpdateConcurrencyException("Concurrency conflict on update.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Failed to update item.", ex);
            }
            return myItem;
        }

        public async Task<T?> Delete(K key)
        {
            var item = await Get(key);
            if (item == null)
                throw new KeyNotFoundException($"No item found with key: {key}");

            _context.Remove(item);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Failed to delete item.", ex);
            }
            return item;
        }
    }
}
