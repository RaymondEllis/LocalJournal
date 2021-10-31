using LocalJournal.Models;
using LocalJournal.Services;
using LocalJournal.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace LocalJournal.Views
{
	public partial class EntriesPage : ContentPage
	{
		readonly EntriesViewModel viewModel;

		public EntriesPage()
		{
			InitializeComponent();

			BindingContext = viewModel = new EntriesViewModel(DependencyService.Get<IDataStore<EntryBase>>());
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.Entries.Count == 0)
				await viewModel.LoadEntriesCommand.ExecuteAsync();
		}

		async void OnEntrySelected(object sender, SelectedItemChangedEventArgs args)
		{
			if (args.SelectedItem is EntryMeta entry)
				await viewModel.EditEntryCommand.ExecuteAsync(entry);

			// Manually deselect entry.
			EntriesListView.SelectedItem = null;
		}

		async void DeleteEntry_Clicked(object sender, EventArgs e)
		{
			if ((sender as BindableObject)?.BindingContext is EntryMeta entry)
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
	}
}