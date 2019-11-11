using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Xamarin.Forms;
#nullable enable

namespace LocalJournal.Services
{
	public class FileDataStore_Platform : FileDataStore
	{
		private StorageFolder? folder;

		public FileDataStore_Platform()
			: base(DependencyService.Get<IDataSerializer<LocalJournal.Models.EntryBase>>())
		{
		}

		protected override async Task<bool> CheckPermission()
		{
			// check if we already have access to the folder.
			if (folder != null)
				return true;
			if (StorageApplicationPermissions.FutureAccessList.ContainsItem("journal_path"))
			{
				folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("journal_path");
				return true;
			}

			// ask user to pick folder.
			var folderPicker = new FolderPicker
			{
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};
			folderPicker.FileTypeFilter.Add("*");

			folder = await folderPicker.PickSingleFolderAsync();
			if (folder != null)
			{
				StorageApplicationPermissions.FutureAccessList.AddOrReplace("journal_path", folder);
				return true;
			}

			return false;
		}

		protected override string FileFromId(string id)
		{
			return $"{id}.md";
		}

		protected override async Task DeleteFile(string filename)
		{
			var file = await folder!.GetFileAsync(filename);
			await file.DeleteAsync();
		}

		protected override async Task<string[]> GetFiles()
		{
			var files = await folder!.GetFilesAsync();
			var result = new string[files.Count];
			for (int i = 0; i < result.Length; ++i)
				result[i] = files[i].Name;
			return result;
		}

		protected override async Task<Stream?> GetStreamAsync(string id, FileAccess access)
		{
			switch (access)
			{
				case FileAccess.Read:
					return await folder.OpenStreamForReadAsync(FileFromId(id));
				case FileAccess.Write:
					return await folder.OpenStreamForWriteAsync(FileFromId(id), CreationCollisionOption.ReplaceExisting);
				default:
					return null;
			}
		}

		protected override async Task<bool> FileExists(string id)
		{
			try
			{
				return (await folder!.GetFileAsync(FileFromId(id))) != null;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
		}
	}
}