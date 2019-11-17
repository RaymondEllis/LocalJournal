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

		public async Task<bool> AddAsync(T item)
		{
			if (!await Folder.CheckPermission())
				return false;

			if (item.Id != null)
				throw new ArgumentException($"Expected a null 'Id' for a new entry, but got Id of '{item.Id}!", nameof(item));

			await AssignUniqueId(item);

			using var stream = await Folder.GetStreamAsync(item.Id!, FileAccess.Write);
			if (stream == null)
				return false;
			using var sw = new StreamWriter(stream);
			return await dataSerializer.WriteAsync(sw, item);
		}

		public async Task<bool> UpdateAsync(T item)
		{
			if (!await Folder.CheckPermission())
				return false;

			if (item.Id == null)
				throw new ArgumentException($"Expected a 'Id' for updating a entry, but got a null 'Id'!", nameof(item));


			using var stream = await Folder.GetStreamAsync(item.Id, FileAccess.Write);
			if (stream == null)
				return false;
			using var sw = new StreamWriter(stream);
			return await dataSerializer.WriteAsync(sw, item);
		}

		public virtual async Task<bool> DeleteAsync(T item)
		{
			if (item.Id is null || !await Folder.CheckPermission())
				return false;

			await Folder.DeleteFile(item.Id);
			return true;
		}

		public async Task<T?> GetAsync(string id, bool ignoreBody = false)
		{
			if (!await Folder.CheckPermission())
				return null;

			using var stream = await Folder.GetStreamAsync(id, FileAccess.Read);
			if (stream == null)
				return null;
			using var sr = new StreamReader(stream);
			return await dataSerializer.ReadAsync(sr, id, ignoreBody);
		}

		public async Task<IEnumerable<T>> GetAllAsync(bool forceRefresh = false)
		{
			if (!await Folder.CheckPermission())
				return new List<T>(0);

			var files = await Folder.GetFiles();
			var items = new List<T>(files.Length);
			foreach (var file in files)
			{
				var item = await GetAsync(Path.GetFileNameWithoutExtension(file), true);
				if (item != null)
					items.Add(item);
			}
			return Order(items);
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