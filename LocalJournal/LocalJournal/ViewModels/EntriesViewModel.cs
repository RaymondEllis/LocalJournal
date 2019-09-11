using LocalJournal.Models;
using LocalJournal.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LocalJournal.ViewModels
{
	public class EntriesViewModel : BaseViewModel
	{
		public ObservableCollection<TextEntry> Entries { get; set; }
		public Command LoadEntriesCommand { get; set; }

		public EntriesViewModel()
		{
			Title = "Browse";
			Entries = new ObservableCollection<TextEntry>();
			LoadEntriesCommand = new Command(async () => await ExecuteLoadEntriesCommand());

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

		async Task ExecuteLoadEntriesCommand()
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
	}
}