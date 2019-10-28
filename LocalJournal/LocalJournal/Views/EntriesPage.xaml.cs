using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LocalJournal.Models;
using LocalJournal.ViewModels;

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

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.Entries.Count == 0)
				await viewModel.LoadEntriesCommand.ExecuteAsync();
		}

		async void OnEntrySelected(object sender, SelectedItemChangedEventArgs args)
		{
			if (args.SelectedItem is TextEntry entry)
				await viewModel.LoadEntryCommand.ExecuteAsync(entry);

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
					await viewModel.DeleteEntryCommand.ExecuteAsync(entry);
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