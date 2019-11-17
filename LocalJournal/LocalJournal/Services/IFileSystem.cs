using System.IO;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public interface IFileSystem
	{
		Task<bool> CheckPermission();

		Task<bool> FileExists(FolderQuery query, string filename);

		Task DeleteFile(FolderQuery query, string filename);

		Task<string[]> GetFiles(FolderQuery query);

		Task<Stream?> GetStreamAsync(FolderQuery query, string filename, FileAccess access);
	}
}
