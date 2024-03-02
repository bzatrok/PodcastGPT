namespace PodcastGPT.Core.Repositories;

public interface IGenericRepository<T> where T : class
{
	Task<List<T>> GetAllAsync();
	Task<T> GetByIdAsync(object id);
	Task AddAsync(T entity);

	Task SaveChangesAsync();
	Task DeleteAsync(object id);
}
