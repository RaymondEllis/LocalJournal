using LocalJournal.Models;
using LocalJournal.Services;
using LocalJournal.Views;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LocalJournal.ViewModels
{
	public class EntriesViewModel : BaseViewModel
	{
		protected static IDataStore<TextEntry> DataStore => DependencyService.Get<IDataStore<TextEntry>>() ?? new MockDataStore();

		public ObservableCollection<TextEntry> Entries { get; }

		public AsyncCommand LoadEntriesCommand => new AsyncCommand(ExecuteLoadEntriesCommand);

		public AsyncCommand<TextEntry> EditEntryCommand => new AsyncCommand<TextEntry>(ExecuteEditEntryCommand);

		public AsyncCommand<TextEntry> DeleteEntryCommand => new AsyncCommand<TextEntry>(ExecuteDeleteEntryCommand);

		public EntriesViewModel()
		{
			Title = "Browse";
			Entries = new ObservableCollection<TextEntry>();

			MessagingCenter.Subscribe<EntryEditPage, TextEntry>(this, "UpdateEntry", async (obj, entry) =>
			 {
				 var index = EntryIndex(entry.Id);

				 if (index == -1)
				 {
					 if (await DataStore.AddEntryAsync(entry))
						 Entries.Add(entry);
				 }
				 else
				 {
					 if (await DataStore.UpdateEntryAsync(entry))
						 Entries[index] = entry; // Refresh list
				 }
			 });
		}

		private int EntryIndex(string? id)
		{
			if (id is null)
				return -1;

			for (int index = 0; index < Entries.Count; ++index)
				if (Entries[index].Id == id)
					return index;

			return -1;
		}

		private async Task ExecuteLoadEntriesCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				Entries.Clear();
				var entries = await DataStore.GetEntriesAsync(true);
				foreach (var entry in entries)
				{
					Entries.Add(entry);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task ExecuteEditEntryCommand(TextEntry entryMeta)
		{
			bool userHasAccess = true;

			if (entryMeta == null || entryMeta.Id == null)
			{

				return;
			}

			// If encrypted, check if locked.
			if (entryMeta.Encrypted)
			{
				var UILock = DependencyService.Get<ILock>();
				userHasAccess = await UILock.UnlockAsync();
			}

			if (userHasAccess)
			{
				var entry = await DataStore.GetEntryAsync(entryMeta.Id);

				if (entry == null || (entry.Encrypted && entry.Body == null))
					await Application.Current.MainPage.DisplayAlert("Unable to decrypt", "Invalid password", "OK");
				else
					await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new EntryEditPage(entry)));
			}
		}

		private async Task ExecuteDeleteEntryCommand(TextEntry entry)
		{
			if (entry?.Id is null)
				return;
			await DataStore.DeleteEntryAsync(entry.Id);
			await LoadEntriesCommand.ExecuteAsync();
		}
	}
}