using LocalJournal.Models;
using LocalJournal.Services;
using LocalJournal.Views;
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
		public ObservableCollection<TextEntry> Entries { get; }

		public AsyncCommand LoadEntriesCommand => new AsyncCommand(ExecuteLoadEntriesCommand);

		public AsyncCommand<TextEntry> LoadEntryCommand => new AsyncCommand<TextEntry>(ExecuteLoadEntryCommand);

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

		private int EntryIndex(string id)
		{
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

		private async Task ExecuteLoadEntryCommand(TextEntry entry)
		{
			bool userHasAccess = true;

			// If encrypted, check if locked.
			if (entry.Encrypted)
			{
				var UILock = DependencyService.Get<ILock>();
				userHasAccess = await UILock.UnlockAsync();
			}

			if (userHasAccess)
			{
				entry = await DataStore.GetEntryAsync(entry.Id);

				if (entry.Encrypted && entry.Body == null)
					await Application.Current.MainPage.DisplayAlert("Unable to decrypt", "Invalid password", "OK");
				else
					await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new EntryEditPage(entry)));
			}
		}

		private async Task ExecuteDeleteEntryCommand(TextEntry entry)
		{
			await DataStore.DeleteEntryAsync(entry.Id);
			await LoadEntriesCommand.ExecuteAsync();
		}
	}
}