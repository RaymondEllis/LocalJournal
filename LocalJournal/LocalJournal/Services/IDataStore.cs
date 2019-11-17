﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public interface IDataStore<T> where T : class
	{
		Task<bool> AddEntryAsync(T entry);
		Task<bool> UpdateEntryAsync(T entry);
		Task<bool> DeleteEntryAsync(T entry);
		Task<T?> GetEntryAsync(string id, bool ignoreBody = false);
		Task<IEnumerable<T>> GetEntriesAsync(bool forceRefresh = false);
	}
}
