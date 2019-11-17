using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public interface IDataStore<T> where T : class
	{
		Task<bool> AddAsync(T item);
		Task<bool> UpdateAsync(T item);
		Task<bool> DeleteAsync(T item);
		Task<T?> GetAsync(string id, bool ignoreBody = false);
		Task<IEnumerable<T>> GetAllAsync(bool forceRefresh = false);
	}
}
