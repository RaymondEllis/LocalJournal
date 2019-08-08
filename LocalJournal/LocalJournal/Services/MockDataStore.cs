using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalJournal.Models;

namespace LocalJournal.Services
{
	public class MockDataStore : IDataStore<TextEntry>
	{
		readonly List<TextEntry> entries;

		public MockDataStore()
		{
			entries = new List<TextEntry>();
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

		public async Task<bool> AddEntryAsync(TextEntry entry)
		{
			entries.Add(entry);

			return await Task.FromResult(true);
		}

		public async Task<bool> UpdateEntryAsync(TextEntry entry)
		{
			var oldEntry = entries.Where((TextEntry arg) => arg.Id == entry.Id).FirstOrDefault();
			entries.Remove(oldEntry);
			entries.Add(entry);

			return await Task.FromResult(true);
		}

		public async Task<bool> DeleteEntryAsync(string id)
		{
			var oldEntry = entries.Where((TextEntry arg) => arg.Id == id).FirstOrDefault();
			entries.Remove(oldEntry);

			return await Task.FromResult(true);
		}

		public async Task<TextEntry> GetEntryAsync(string id, bool ignoreBody = false)
		{
			return await Task.FromResult(entries.FirstOrDefault(entry => entry.Id == id));
		}

		public async Task<IEnumerable<TextEntry>> GetEntriesAsync(bool forceRefresh = false)
		{
			return await Task.FromResult(entries);
		}
	}
}