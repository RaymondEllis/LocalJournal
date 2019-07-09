﻿using LocalJournal.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.AccessCache;
using Xamarin.Essentials;
using Xamarin.Forms;
using System;
using Windows.Storage.Search;

namespace LocalJournal.Services
{
	public class UWPFileDataStore : IDataStore<TextEntry>
	{
		StorageFolder folder;
		private readonly IDataSerializer<TextEntry> dataSerializer;

		public UWPFileDataStore()
		{
			dataSerializer = DependencyService.Get<IDataSerializer<TextEntry>>();
		}

		public async Task<bool> AddEntryAsync(TextEntry entry)
		{
			if (!await CheckPermission())
				return false;

			using (var stream = await folder.OpenStreamForWriteAsync(FileFromId(entry.Id), CreationCollisionOption.ReplaceExisting))
			{
				using (var sw = new StreamWriter(stream))
					return await dataSerializer.WriteAsync(sw, entry);
			}
		}

		public async Task<bool> UpdateEntryAsync(TextEntry entry)
		{
			if (!await CheckPermission())
				return false;

			entry.LastModified = MyDate.Now();

			using (var stream = await folder.OpenStreamForWriteAsync(FileFromId(entry.Id), CreationCollisionOption.ReplaceExisting))
			{
				using (var sw = new StreamWriter(stream))
					return await dataSerializer.WriteAsync(sw, entry);
			}
		}

		public async Task<bool> DeleteEntryAsync(string id)
		{
			if (!await CheckPermission())
				return false;

			var file = await folder.GetFileAsync(FileFromId(id));
			await file.DeleteAsync();

			return true;
		}

		public async Task<TextEntry> GetEntryAsync(string id)
		{
			if (!await CheckPermission())
				return null;

			using (var stream = await folder.OpenStreamForReadAsync(FileFromId(id)))
				return await GetEntryAsync(id, stream);
		}
		private async Task<TextEntry> GetEntryAsync(string id, Stream stream)
		{
			using (var sr = new StreamReader(stream))
				return await dataSerializer.ReadAsync(sr, id);
		}

		public async Task<IEnumerable<TextEntry>> GetEntriesAsync(bool forceRefresh = false)
		{
			if (!await CheckPermission())
				return new List<TextEntry>(0);

			var files = await folder.GetFilesAsync();
			var entries = new List<TextEntry>(files.Count);
			foreach (var file in files)
			{
				if (Path.GetExtension(file.Name).Equals(".md", StringComparison.InvariantCultureIgnoreCase))
				{
					TextEntry entry;
					using (var stream = await file.OpenStreamForReadAsync())
						entry = await GetEntryAsync(Path.GetFileNameWithoutExtension(file.Name), stream);
					if (entry != null)
						entries.Add(entry);
				}
			}
			return await Task.FromResult(entries);
		}

		private string FileFromId(string id)
		{
			return $"{id}.md";
		}

		private async Task<bool> CheckPermission()
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
			var folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			folderPicker.FileTypeFilter.Add("*");

			folder = await folderPicker.PickSingleFolderAsync();
			if (folder != null)
			{
				StorageApplicationPermissions.FutureAccessList.AddOrReplace("journal_path", folder);
				return true;
			}

			return false;
		}
	}
}