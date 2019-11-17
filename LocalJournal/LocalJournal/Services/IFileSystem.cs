using System.IO;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public interface IFileSystem
	{
		Task<bool> CheckPermission();

		string FileFromId(string id);

		Task<bool> FileExists(string id);

		Task DeleteFile(string filename);

		Task<string[]> GetFiles();

		Task<Stream?> GetStreamAsync(string id, FileAccess access);
	}
}
