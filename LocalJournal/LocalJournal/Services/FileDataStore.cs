using NodaTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public abstract class FileDataStore<T> : IDataStore<T>
		where T : class, IItem
	{
		protected readonly IDataSerializer<T> dataSerializer;
		FolderQuery Folder { get; }

		protected FileDataStore(FolderQuery folder, IDataSerializer<T> dataSerializer)
		{
			Folder = folder;
			this.dataSerializer = dataSerializer;
		}

		public async Task<bool> AddEntryAsync(T entry)
		{
			if (!await Folder.CheckPermission())
				return false;

			if (entry.Id != null)
				throw new ArgumentException($"Expected a null 'Id' for a new entry, but got Id of '{entry.Id}!", nameof(entry));

			await AssignUniqueId(entry);

			using var stream = await Folder.GetStreamAsync(entry.Id!, FileAccess.Write);
			if (stream == null)
				return false;
			using var sw = new StreamWriter(stream);
			return await dataSerializer.WriteAsync(sw, entry);
		}

		public async Task<bool> UpdateEntryAsync(T entry)
		{
			if (!await Folder.CheckPermission())
				return false;

			if (entry.Id == null)
				throw new ArgumentException($"Expected a 'Id' for updating a entry, but got a null 'Id'!", nameof(entry));


			using var stream = await Folder.GetStreamAsync(entry.Id, FileAccess.Write);
			if (stream == null)
				return false;
			using var sw = new StreamWriter(stream);
			return await dataSerializer.WriteAsync(sw, entry);
		}

		public virtual async Task<bool> DeleteEntryAsync(string id)
		{
			if (!await Folder.CheckPermission())
				return false;

			await Folder.DeleteFile(id);

			return true;
		}

		public async Task<T?> GetEntryAsync(string id, bool ignoreBody = false)
		{
			if (!await Folder.CheckPermission())
				return null;

			using var stream = await Folder.GetStreamAsync(id, FileAccess.Read);
			if (stream == null)
				return null;
			using var sr = new StreamReader(stream);
			return await dataSerializer.ReadAsync(sr, id, ignoreBody);
		}

		public async Task<IEnumerable<T>> GetEntriesAsync(bool forceRefresh = false)
		{
			if (!await Folder.CheckPermission())
				return new List<T>(0);

			var files = await Folder.GetFiles();
			var entries = new List<T>(files.Length);
			foreach (var file in files)
			{
				var entry = await GetEntryAsync(Path.GetFileNameWithoutExtension(file), true);
				if (entry != null)
					entries.Add(entry);
			}
			return Order(entries);
		}

		protected abstract Task AssignUniqueId(T item);
		protected async Task<string> VerifyUniqueId(string requestedId)
		{
			string id = requestedId;

			int i = 0;
			while (await Folder.FileExists(id))
			{
				id = $"{requestedId}_{++i}";
			}
			return id;
		}

		protected virtual void OnUpdated(T item)
		{ }

		protected abstract IOrderedEnumerable<T> Order(List<T> items);
	}
}