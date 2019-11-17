using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
#nullable enable

namespace LocalJournal.Services
{
	public class FileSystem_Platform : IFileSystem
	{
		private StorageFolder? folder;

		private async Task<StorageFolder> GetFolder(FolderQuery query)
		{
			if (string.IsNullOrEmpty(query.SubFolder))
				return folder!;
			else
			{
				return await folder!.CreateFolderAsync(query.SubFolder, CreationCollisionOption.OpenIfExists);
			}
		}

		public async Task<bool> CheckPermission()
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

		public async Task DeleteFile(FolderQuery query, string filename)
		{
			var folder = await GetFolder(query);
			var file = await folder.GetFileAsync(filename);
			await file.DeleteAsync();
		}

		public async Task<string[]> GetFiles(FolderQuery query)
		{
			var folder = await GetFolder(query);


			var queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, new[] { query.Extension })
			{
				FolderDepth = FolderDepth.Shallow
			};
			var queryResult = folder.CreateFileQueryWithOptions(queryOptions);

			var files = await queryResult.GetFilesAsync();
			var result = new string[files.Count];
			for (int i = 0; i < result.Length; ++i)
				result[i] = files[i].Name;
			return result;
		}

		public async Task<Stream?> GetStreamAsync(FolderQuery query, string filename, FileAccess access)
		{
			var folder = await GetFolder(query);

			return access switch
			{
				FileAccess.Read => await folder.OpenStreamForReadAsync(filename),
				FileAccess.Write => await folder.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting),
				_ => null,
			};
		}

		public async Task<bool> FileExists(FolderQuery query, string filename)
		{
			var folder = await GetFolder(query);

			try
			{
				return (await folder.GetFileAsync(filename)) != null;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
		}
	}
}