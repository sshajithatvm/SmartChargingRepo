using System.Linq.Expressions;

namespace SmartChargingApi.Repository;
public interface ISmartChargingRepository
{
    void Add<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate, string[] include) where T : class;
    Task<bool> SaveAll();
}

