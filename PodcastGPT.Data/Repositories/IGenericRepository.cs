using System.Linq.Expressions;

namespace PodcastGPT.Core.Repositories;

public interface IGenericRepository<T> where T : class
{
	Task<List<T>> GetAllAsync();
	Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate);
	Task<T> GetByIdAsync(object id);
	Task AddAsync(T entity);

	Task SaveChangesAsync();
	Task DeleteAsync(object id);
}
