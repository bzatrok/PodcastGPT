using System.Linq.Expressions;
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
	
	public async Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate)
	{
		return await _dbSet.Where(predicate).ToListAsync();
	}

	public async Task<T> GetByIdAsync(object id)
	{
		return await _dbSet.FindAsync(id);
	}

	public async Task AddAsync(T entity)
	{
		try
		{
			await _dbSet.AddAsync(entity);
			await SaveChangesAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}

	public async Task SaveChangesAsync()
	{
		try
		{
			await _context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
	}

	public async Task DeleteAsync(object id)
	{
		T entityToDelete = _dbSet.Find(id);
		if (entityToDelete != null)
		{
			_dbSet.Remove(entityToDelete);
			await SaveChangesAsync();
		}
	}
}
