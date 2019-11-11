using LocalJournal.Models;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public abstract class FileDataStore : IDataStore<EntryBase>
	{
		protected readonly IDataSerializer<EntryBase> dataSerializer;

		protected FileDataStore(IDataSerializer<EntryBase> dataSerializer)
		{
			this.dataSerializer = dataSerializer;
		}

		public async Task<bool> AddEntryAsync(EntryBase entry)
		{
			if (!await CheckPermission())
				return false;

			if (entry.Id != null)
				throw new ArgumentException($"Expected a null 'Id' for a new entry, but got Id of '{entry.Id}!", nameof(entry));

			entry.Id = await CreateUniqueID(entry.CreationTime.ToIdString());

			using var stream = await GetStreamAsync(entry.Id, FileAccess.Write);
			if (stream == null)
				return false;
			using var sw = new StreamWriter(stream);
			return await dataSerializer.WriteAsync(sw, entry);
		}

		public async Task<bool> UpdateEntryAsync(EntryBase entry)
		{
			if (!await CheckPermission())
				return false;

			if (entry.Id == null)
				throw new ArgumentException($"Expected a 'Id' for updating a entry, but got a null 'Id'!", nameof(entry));

			entry.LastModified = MyDate.Now();

			using var stream = await GetStreamAsync(entry.Id, FileAccess.Write);
			if (stream == null)
				return false;
			using var sw = new StreamWriter(stream);
			return await dataSerializer.WriteAsync(sw, entry);
		}

		public virtual async Task<bool> DeleteEntryAsync(string id)
		{
			if (!await CheckPermission())
				return false;

			await DeleteFile(FileFromId(id));

			return true;
		}

		public async Task<EntryBase?> GetEntryAsync(string id, bool ignoreBody = false)
		{
			if (!await CheckPermission())
				return null;

			using var stream = await GetStreamAsync(id, FileAccess.Read);
			if (stream == null)
				return null;
			using var sr = new StreamReader(stream);
			return await dataSerializer.ReadAsync(sr, id, ignoreBody);
		}

		public async Task<IEnumerable<EntryBase>> GetEntriesAsync(bool forceRefresh = false)
		{
			if (!await CheckPermission())
				return new List<EntryBase>(0);

			var files = await GetFiles();
			var entries = new List<EntryBase>(files.Length);
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

		protected abstract Task<Stream?> GetStreamAsync(string id, FileAccess access);
	}
}