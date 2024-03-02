using PodcastGPT.Data;
using Microsoft.EntityFrameworkCore;

namespace PodcastGPT.Core.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
	private readonly DatabaseContext _context;
	private DbSet<T> _dbSet;

	public GenericRepository(DatabaseContext context)
	{
		_context = context;
		_dbSet = context.Set<T>();
	}

	public async Task<List<T>> GetAllAsync()
	{
		return await _dbSet.ToListAsync();
	}

	public async Task<T> GetByIdAsync(object id)
	{
		return await _dbSet.FindAsync(id);
	}

	public async Task InsertOrUpdateAsync(object id, T entity)
	{
		try
		{
			var result = await GetByIdAsync(id);
			if (result != null)
			{
				try
				{
					// you can't attach book since it doesn't exist in the database yet
					// attach result instead
					_context.Attach(result);
					result = entity; // this will update all the fields at once
					_context.SaveChanges();
				}
				catch (Exception ex)
				{
					throw;
				}
			}
			else
			{
				await _dbSet.AddAsync(entity);
			}

			// await _dbSet.AddAsync(entity);
			_context.SaveChanges();
		}
		catch (Exception ex)
		{
			throw;
		}
	}

	// public void Update(T entity)
	// {
	// 	_dbSet.Attach(entity);
	// 	_context.Entry(entity).State = EntityState.Modified;
	// 	_context.SaveChanges();
	// }

	public void Delete(object id)
	{
		T entityToDelete = _dbSet.Find(id);
		if (entityToDelete != null)
		{
			_dbSet.Remove(entityToDelete);
			_context.SaveChanges();
		}
	}
}
