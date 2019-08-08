using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public interface IDataStore<T>
	{
		Task<bool> AddEntryAsync(T entry);
		Task<bool> UpdateEntryAsync(T entry);
		Task<bool> DeleteEntryAsync(string id);
		Task<T> GetEntryAsync(string id, bool ignoreBody = false);
		Task<IEnumerable<T>> GetEntriesAsync(bool forceRefresh = false);
	}
}
