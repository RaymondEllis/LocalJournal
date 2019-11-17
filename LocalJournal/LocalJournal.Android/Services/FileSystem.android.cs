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

		private string FullFileName(FolderQuery query, string filename)
		{
			if (string.IsNullOrEmpty(query.SubFolder))
				return Path.Combine(DataPath, filename);
			else
				return Path.Combine(DataPath, Path.Combine(query.SubFolder, filename));
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

		public Task DeleteFile(FolderQuery query, string filename)
		{
			return Task.Run(() => File.Delete(FullFileName(query, filename)));
		}

		public Task<string[]> GetFiles(FolderQuery query)
		{
			if (string.IsNullOrEmpty(query.SubFolder))
				return Task.FromResult(Directory.GetFiles(DataPath, $"*{query.Extension}", SearchOption.TopDirectoryOnly));
			else
				return Task.FromResult(Directory.GetFiles(Path.Combine(DataPath, query.SubFolder!), $"*{query.Extension}", SearchOption.TopDirectoryOnly));
		}

		public Task<Stream?> GetStreamAsync(FolderQuery query, string filename, FileAccess access)
		{
			if (access == FileAccess.Read)
				return Task.FromResult((Stream?)new FileStream(FullFileName(query, filename), FileMode.Open, access));
			else
				return Task.FromResult((Stream?)new FileStream(FullFileName(query, filename), FileMode.Create, access));
		}

		public Task<bool> FileExists(FolderQuery query, string filename)
		{
			return Task.FromResult(File.Exists(FullFileName(query, filename)));
		}
	}
}