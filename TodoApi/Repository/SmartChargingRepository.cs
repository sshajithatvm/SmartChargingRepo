using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SmartChargingApi.Repository;
public class SmartChargingRepository : ISmartChargingRepository
{
    private readonly SmartChargingDbContext _context;
    public SmartChargingRepository(SmartChargingDbContext context)
    {
        _context = context;
    }
    public async void Add<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);
    public void Delete<T>(T entity) where T : class => _context.Set<T>().Remove(entity);
    public void Update<T>(T entity) where T : class =>  _context.Set<T>().Update(entity);
    public async Task<bool> SaveAll() => (await _context.SaveChangesAsync().ConfigureAwait(continueOnCapturedContext: false) >0);
    public async Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate, string[] includes) where T : class
    {
        var items = _context.Set<T>().AsNoTracking().Where(predicate);
        foreach (var includeExpression in includes)
        {
            items = items.Include(includeExpression);
        }

        return await items.FirstOrDefaultAsync();
    }
}
