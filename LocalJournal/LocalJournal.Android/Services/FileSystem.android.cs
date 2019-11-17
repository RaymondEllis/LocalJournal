using Android.OS;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.IO;
using System.Threading.Tasks;
#nullable enable

namespace LocalJournal.Services
{
	public class FileSystem_Platform : IFileSystem
	{
		public string DataPath { get; protected set; }

		public FileSystem_Platform()
		{
			DataPath = Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, "journal");
			Directory.CreateDirectory(DataPath);
		}

		public async Task<bool> CheckPermission()
		{
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

				if (status != PermissionStatus.Granted)
					status = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();

				return status == PermissionStatus.Granted;
			}
			catch
			{
				return false;
			}
		}

		public string FileFromId(string id)
		{
			return Path.Combine(DataPath, $"{id}.md");
		}
		public Task DeleteFile(string filename)
		{
			return Task.Run(() => File.Delete(filename));
		}

		public Task<string[]> GetFiles()
		{
			return Task.FromResult(Directory.GetFiles(DataPath, "*.md"));
		}

		public Task<Stream?> GetStreamAsync(string id, FileAccess access)
		{
			if (access == FileAccess.Read)
				return Task.FromResult((Stream?)new FileStream(FileFromId(id), FileMode.Open, access));
			else
				return Task.FromResult((Stream?)new FileStream(FileFromId(id), FileMode.Create, access));
		}

		public Task<bool> FileExists(string id)
		{
			return Task.FromResult(File.Exists(FileFromId(id)));
		}
	}
}