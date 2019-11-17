using System.IO;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public class FolderQuery
	{
		readonly IFileSystem fileSystem;
		public readonly string? SubFolder;
		public readonly string Extension;

		public FolderQuery(IFileSystem fileSystem, string? subFolder, string extension)
		{
			this.fileSystem = fileSystem;
			SubFolder = subFolder;
			Extension = extension;
		}

		private string IdToFileName(string id) => $"{id}{Extension}";

		public Task<bool> CheckPermission()
			=> fileSystem.CheckPermission();

		public Task<bool> FileExists(string id)
			=> fileSystem.FileExists(this, IdToFileName(id));

		public Task DeleteFile(string id)
			=> fileSystem.DeleteFile(this, IdToFileName(id));

		public Task<string[]> GetFiles()
			=> fileSystem.GetFiles(this);

		public Task<Stream?> GetStreamAsync(string id, FileAccess access)
			=> fileSystem.GetStreamAsync(this, IdToFileName(id), access);
	}
}
