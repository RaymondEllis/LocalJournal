using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LocalJournal.Models;
using LocalJournal.ViewModels;

namespace LocalJournal.Views
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class EntryDetailPage : ContentPage
	{
		readonly EntryDetailViewModel viewModel;

		public EntryDetailPage(EntryDetailViewModel viewModel)
		{
			InitializeComponent();

			BindingContext = this.viewModel = viewModel;
		}

		public EntryDetailPage()
		{
			InitializeComponent();

			var entry = new TextEntry
			{
				Title = "",
				Body = ""
			};

			viewModel = new EntryDetailViewModel(entry);
			BindingContext = viewModel;
		}
	}
}