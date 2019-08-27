using LocalJournal.Models;
using NodaTime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LocalJournal.Services
{
	public class FileDataStore : IDataStore<TextEntry>
	{
		public string DataPath { get; protected set; }

		private readonly IDataSerializer<TextEntry> dataSerializer;

		public FileDataStore()
		{
			dataSerializer = DependencyService.Get<IDataSerializer<TextEntry>>();
			DataPath = Path.Combine(FileSystem.AppDataDirectory, "journal");
		}

		public async Task<bool> AddEntryAsync(TextEntry entry)
		{
			if (!CheckPermission())
				return false;

			Directory.CreateDirectory(DataPath);
			var file = FileFromId(entry.Id);
			using (var sw = new StreamWriter(file))
				return await dataSerializer.WriteAsync(sw, entry);
		}

		public async Task<bool> UpdateEntryAsync(TextEntry entry)
		{
			if (!CheckPermission())
				return false;

			entry.LastModified = MyDate.Now();

			using (var sw = new StreamWriter(FileFromId(entry.Id)))
				return await dataSerializer.WriteAsync(sw, entry);
		}

		public async Task<bool> DeleteEntryAsync(string id)
		{
			if (!CheckPermission())
				return false;

			File.Delete(FileFromId(id));

			return await Task.FromResult(true);
		}

		public async Task<TextEntry> GetEntryAsync(string id, bool ignoreBody = false)
		{
			if (!CheckPermission())
				return null;

			using (var sr = new StreamReader(FileFromId(id)))
				return await dataSerializer.ReadAsync(sr, id, ignoreBody);
		}

		public async Task<IEnumerable<TextEntry>> GetEntriesAsync(bool forceRefresh = false)
		{
			if (!CheckPermission())
				return new List<TextEntry>(0);

			var files = Directory.GetFiles(DataPath, "*.md");
			var entries = new List<TextEntry>(files.Length);
			foreach (var file in files)
			{
				var entry = await GetEntryAsync(Path.GetFileNameWithoutExtension(file), true);
				if (entry != null)
					entries.Add(entry);
			}
			return entries.OrderByDescending(e => e.CreationTime, OffsetDateTime.Comparer.Instant);
		}

		protected virtual string FileFromId(string id)
		{
			return Path.Combine(DataPath, $"{id}.md");
		}

		protected virtual bool CheckPermission()
		{
			return true;
		}
	}
}