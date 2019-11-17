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
		protected IDataStore<EntryBase> DataStore { get; }

		public ObservableCollection<EntryMeta> Entries { get; }

		public AsyncCommand LoadEntriesCommand => new AsyncCommand(ExecuteLoadEntriesCommand);

		public AsyncCommand<EntryMeta> EditEntryCommand => new AsyncCommand<EntryMeta>(ExecuteEditEntryCommand);

		public AsyncCommand<EntryMeta> DeleteEntryCommand => new AsyncCommand<EntryMeta>(ExecuteDeleteEntryCommand);

		public EntriesViewModel(IDataStore<EntryBase>? dataStore)
		{
			Title = "Browse";
			Entries = new ObservableCollection<EntryMeta>();

			DataStore = dataStore ?? new MockDataStore();

			MessagingCenter.Subscribe<EntryEditPage, EntryBase>(this, "UpdateEntry", async (obj, entry) =>
			{
				if (entry is EntryMeta)
					throw new ArgumentException($"{nameof(EntryMeta)} is a invalid type to call the message \"UpdateEntry\"", nameof(entry));

				var index = EntryIndex(entry.Id);

				if (index == -1)
				{
					if (await DataStore.AddAsync(entry))
						Entries.Add(entry.AsMeta());
				}
				else
				{
					if (await DataStore.UpdateAsync(entry))
						Entries[index] = entry.AsMeta(); // Refresh list
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
				var entries = await DataStore.GetAllAsync(true);
				foreach (var entry in entries)
				{
					Entries.Add(entry.AsMeta());
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

		private async Task ExecuteEditEntryCommand(EntryMeta entryMeta)
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
				try
				{
					var entry = await DataStore.GetAsync(entryMeta.Id);
					await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new EntryEditPage(entry)));
				}
				catch (InvalidPasswordException)
				{
					await Application.Current.MainPage.DisplayAlert("Unable to decrypt", "Invalid password", "OK");
				}
			}
		}

		private async Task ExecuteDeleteEntryCommand(EntryMeta entryMeta)
		{
			await DataStore.DeleteAsync(entryMeta);
			await LoadEntriesCommand.ExecuteAsync();
		}
	}
}