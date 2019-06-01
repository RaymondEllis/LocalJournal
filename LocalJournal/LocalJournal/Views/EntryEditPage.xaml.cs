using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LocalJournal.Models;

namespace LocalJournal.Views
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class EntryEditPage : ContentPage
	{
		public TextEntry Entry { get; set; }

		public EntryEditPage(TextEntry entry)
		{
			InitializeComponent();

			Entry = entry;
			//Entry = new TextEntry
			//{
			//	Title = "",
			//	Body = "",
			//};

			BindingContext = this;
		}

		async void Save_Clicked(object sender, EventArgs e)
		{
			MessagingCenter.Send(this, "UpdateEntry", Entry);
			await Navigation.PopModalAsync();
		}

		async void Cancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}