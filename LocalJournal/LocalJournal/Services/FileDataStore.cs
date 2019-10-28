using LocalJournal.Models;
using NodaTime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LocalJournal.Services
{
	public abstract class FileDataStore : IDataStore<TextEntry>
	{
		protected readonly IDataSerializer<TextEntry> dataSerializer;

		protected FileDataStore()
		{
			dataSerializer = DependencyService.Get<IDataSerializer<TextEntry>>();
		}

		public async Task<bool> AddEntryAsync(TextEntry entry)
		{
			if (!await CheckPermission())
				return false;

			entry.Id = await CreateUniqueID(entry.CreationTime.ToIdString());

			using (var stream = await GetStreamAsync(entry.Id, FileAccess.Write))
			using (var sw = new StreamWriter(stream))
				return await dataSerializer.WriteAsync(sw, entry);
		}

		public async Task<bool> UpdateEntryAsync(TextEntry entry)
		{
			if (!await CheckPermission())
				return false;

			entry.LastModified = MyDate.Now();

			using (var stream = await GetStreamAsync(entry.Id, FileAccess.Write))
			using (var sw = new StreamWriter(stream))
				return await dataSerializer.WriteAsync(sw, entry);
		}

		public virtual async Task<bool> DeleteEntryAsync(string id)
		{
			if (!await CheckPermission())
				return false;

			await DeleteFile(FileFromId(id));

			return true;
		}

		public async Task<TextEntry> GetEntryAsync(string id, bool ignoreBody = false)
		{
			if (!await CheckPermission())
				return null;

			using (var stream = await GetStreamAsync(id, FileAccess.Read))
			using (var sr = new StreamReader(stream))
				return await dataSerializer.ReadAsync(sr, id, ignoreBody);
		}

		public async Task<IEnumerable<TextEntry>> GetEntriesAsync(bool forceRefresh = false)
		{
			if (!await CheckPermission())
				return new List<TextEntry>(0);

			var files = await GetFiles();
			var entries = new List<TextEntry>(files.Length);
			foreach (var file in files)
			{
				var entry = await GetEntryAsync(Path.GetFileNameWithoutExtension(file), true);
				if (entry != null)
					entries.Add(entry);
			}
			return entries.OrderByDescending(e => e.CreationTime, OffsetDateTime.Comparer.Instant);
		}

		protected async Task<string> CreateUniqueID(string requestedId)
		{
			string id = requestedId;

			int i = 0;
			while (await FileExists(id))
			{
				id = $"{requestedId}_{++i}";
			}
			return id;
		}

		protected abstract Task<bool> CheckPermission();

		protected abstract string FileFromId(string id);

		protected abstract Task<bool> FileExists(string id);

		protected abstract Task DeleteFile(string filename);

		protected abstract Task<string[]> GetFiles();

		protected abstract Task<Stream> GetStreamAsync(string id, FileAccess access);
	}
}