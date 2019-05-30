using LocalJournal.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LocalJournal.Services
{
	public class FileDataStore : IDataStore<TextEntry>
	{
		public static string DataPath = Path.Combine(FileSystem.AppDataDirectory, "journal");

		private readonly IDataSerializer<TextEntry> dataSerializer;

		public FileDataStore()
		{
			dataSerializer = DependencyService.Get<IDataSerializer<TextEntry>>();
		}

		public async Task<bool> AddEntryAsync(TextEntry entry)
		{
			var file = FileFromId(entry.Id);
			Directory.CreateDirectory(file);
			using (var sw = new StreamWriter(file))
				return await dataSerializer.WriteAsync(sw, entry);
		}

		public async Task<bool> UpdateEntryAsync(TextEntry entry)
		{
			using (var sw = new StreamWriter(FileFromId(entry.Id)))
				return await dataSerializer.WriteAsync(sw, entry);
		}

		public async Task<bool> DeleteEntryAsync(string id)
		{
			File.Delete(FileFromId(id));

			return await Task.FromResult(true);
		}

		public async Task<TextEntry> GetEntryAsync(string id)
		{
			using (var sr = new StreamReader(FileFromId(id)))
				return await dataSerializer.ReadAsync(sr, id);
		}

		public async Task<IEnumerable<TextEntry>> GetEntriesAsync(bool forceRefresh = false)
		{
			var files = Directory.GetFiles(DataPath, "*.md");
			var entries = new List<TextEntry>(files.Length);
			foreach (var file in files)
			{
				var entry = await GetEntryAsync(Path.GetFileNameWithoutExtension(file));
				if (entry != null)
					entries.Add(entry);
			}
			return await Task.FromResult(entries);
		}

		private string FileFromId(string id)
		{
			return Path.Combine(DataPath, $"{id}.md");
		}
	}
}