using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using LocalJournal.Models;
using LocalJournal.Views;

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

			MessagingCenter.Subscribe<NewEntryPage, TextEntry>(this, "AddEntry", async (obj, entry) =>
			{
				if (await DataStore.AddEntryAsync(entry))
					Entries.Add(entry);
			});
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