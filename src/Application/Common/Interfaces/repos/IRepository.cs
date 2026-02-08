namespace Application.Common.Interfaces.repos;

public interface IRepository<T> where T : class
{
   
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<string?> GetOwnerIdAsync(Guid id);
}