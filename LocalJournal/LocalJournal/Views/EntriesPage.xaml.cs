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

		async void OnEntrieSelected(object sender, SelectedItemChangedEventArgs args)
		{
			var entry = args.SelectedItem as TextEntry;
			if (entry == null)
				return;

			await Navigation.PushModalAsync(new NavigationPage(new EntryEditPage(entry)));

			// Manually deselect entry.
			EntriesListView.SelectedItem = null;
		}

		async void AddEntry_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new NewEntryPage()));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.Entries.Count == 0)
				viewModel.LoadEntriesCommand.Execute(null);
		}
	}
}