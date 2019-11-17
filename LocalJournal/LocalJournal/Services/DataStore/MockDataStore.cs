using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalJournal.Models;

namespace LocalJournal.Services
{
	public class MockDataStore : IDataStore<EntryBase>
	{
		readonly List<EntryBase> entries;

		public MockDataStore()
		{
			entries = new List<EntryBase>();
			var mockEntries = new List<TextEntry>
			{
				new TextEntry { Id = Guid.NewGuid().ToString(), Title = "[mock] First entry ", Body="This is body of a text entry." },
				new TextEntry { Id = Guid.NewGuid().ToString(), Title = "[mock] Second entry", Body="This is body of a text entry." },
				new TextEntry { Id = Guid.NewGuid().ToString(), Title = "[mock] Third entry ", Body="This is body of a text entry." },
				new TextEntry { Id = Guid.NewGuid().ToString(), Title = "[mock] Fourth entry", Body="This is body of a text entry." },
				new TextEntry { Id = Guid.NewGuid().ToString(), Title = "[mock] Fifth entry ", Body="This is body of a text entry." },
				new TextEntry { Id = Guid.NewGuid().ToString(), Title = "[mock] Sixth entry ", Body="This is body of a text entry." }
			};

			foreach (var entry in mockEntries)
			{
				entries.Add(entry);
			}
		}

		public async Task<bool> AddAsync(EntryBase entry)
		{
			entries.Add(entry);

			return await Task.FromResult(true);
		}

		public async Task<bool> UpdateAsync(EntryBase entry)
		{
			var oldEntry = entries.Where((EntryBase arg) => arg.Id == entry.Id).FirstOrDefault();
			entries.Remove(oldEntry);
			entries.Add(entry);

			return await Task.FromResult(true);
		}

		public async Task<bool> DeleteAsync(EntryBase entry)
		{
			var oldEntry = entries.Where((EntryBase arg) => arg.Id == entry.Id).FirstOrDefault();
			entries.Remove(oldEntry);

			return await Task.FromResult(true);
		}

		public async Task<EntryBase?> GetAsync(string id, bool ignoreBody = false)
		{
			return await Task.FromResult(entries.FirstOrDefault(entry => entry.Id == id));
		}

		public async Task<IEnumerable<EntryBase>> GetAllAsync(bool forceRefresh = false)
		{
			return await Task.FromResult(entries);
		}
	}
}