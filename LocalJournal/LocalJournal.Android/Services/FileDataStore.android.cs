using Android.OS;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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

		protected override async Task<bool> CheckPermission()
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

		protected override string FileFromId(string id)
		{
			return Path.Combine(DataPath, $"{id}.md");
		}
		protected override Task DeleteFile(string filename)
		{
			return Task.Run(() => File.Delete(filename));
		}

		protected override Task<string[]> GetFiles()
		{
			return Task.FromResult(Directory.GetFiles(DataPath, "*.md"));
		}

		protected override Task<Stream> GetStreamAsync(string id, FileAccess access)
		{
			return Task.FromResult((Stream)new FileStream(FileFromId(id), FileMode.OpenOrCreate, access));
		}

		protected override Task<bool> FileExists(string id)
		{
			return Task.FromResult(File.Exists(FileFromId(id)));
		}
	}
}