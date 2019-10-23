using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LocalJournal.Models;
using LocalJournal.Views;
using LocalJournal.ViewModels;
using LocalJournal.Services;

namespace LocalJournal.Views
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class EntriesPage : ContentPage
	{
		readonly EntriesViewModel viewModel;

		public EntriesPage()
		{
			InitializeComponent();

			BindingContext = viewModel = new EntriesViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.Entries.Count == 0)
				viewModel.LoadEntriesCommand.Execute(null);
		}

		async void OnEntrySelected(object sender, SelectedItemChangedEventArgs args)
		{
			var entry = args.SelectedItem as TextEntry;
			if (entry == null)
				return;

			bool userHasAccess = true;

			// If encrypted, check if locked.
			if (entry.Encrypted)
			{
				var UILock = DependencyService.Get<ILock>();
				userHasAccess = await UILock.UnlockAsync();
			}

			if (userHasAccess)
			{
				entry = await viewModel.DataStore.GetEntryAsync(entry.Id);

				if (entry.Encrypted && entry.Body == null)
					await DisplayAlert("Unable to decrypt", "Invalid password", "OK");
				else
					await Navigation.PushModalAsync(new NavigationPage(new EntryEditPage(entry)));
			}

			// Manually deselect entry.
			EntriesListView.SelectedItem = null;
		}

		async void AddEntry_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new EntryEditPage(null)));
		}

		async void DeleteEntry_Clicked(object sender, EventArgs e)
		{
			if ((sender as Button).BindingContext is TextEntry entry)
			{
				if (await DisplayAlert("Delete confirmation",
					$"Are you sure you want to delete\n" +
					$"Id: {entry.Id}\n" +
					$"Title: {entry.Title}", "YES", "NO"))
				{
					await viewModel.DataStore.DeleteEntryAsync(entry.Id);
					viewModel.LoadEntriesCommand.Execute(null);
				}
			}
			else
				await DisplayAlert("Nothing selected", "Unable to find entry to delete.", "OK");
		}

		async void SetupCryptro_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new CryptoSetupPage()));
		}
	}
}