using Android.OS;
using System.IO;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public class FileDataStorePlatform : FileDataStore
	{
		public string DataPath { get; protected set; }

		public FileDataStorePlatform()
			: base()
		{
			DataPath = Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, "journal");
			Directory.CreateDirectory(DataPath);
		}
		protected override Task<bool> CheckPermission()
		{
			// ToDo: Check external permissions.
			return Task.FromResult(true);
		}

		protected override string FileFromId(string id)
		{
			return Path.Combine(DataPath, $"{id}.md");
		}
		protected override Task DeleteFile(string filename)
		{
			File.Delete(filename);
			return null;
		}

		protected override Task<string[]> GetFiles()
		{
			return Task.FromResult(Directory.GetFiles(DataPath, "*.md"));
		}

		protected override Task<Stream> GetStreamAsync(string id, FileAccess access)
		{
			return Task.FromResult((Stream)new FileStream(FileFromId(id), FileMode.OpenOrCreate, access));
		}
	}
}