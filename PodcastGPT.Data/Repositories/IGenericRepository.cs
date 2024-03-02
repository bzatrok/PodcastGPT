namespace PodcastGPT.Core.Repositories;

public interface IGenericRepository<T> where T : class
{
	Task<List<T>> GetAllAsync();
	Task<T> GetByIdAsync(object id);
	Task InsertOrUpdateAsync(object id, T entity);

	// Task InsertAsync(T entity);
	// void Update(T entity);
	void Delete(object id);
}
