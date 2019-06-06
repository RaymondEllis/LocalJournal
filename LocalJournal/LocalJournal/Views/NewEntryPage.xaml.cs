using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LocalJournal.Models;
using NodaTime;
using NodaTime.Extensions;

namespace LocalJournal.Views
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class NewEntryPage : ContentPage
	{
		public TextEntry Entry { get; set; }

		public NewEntryPage()
		{
			InitializeComponent();


			var now = SystemClock.Instance.InTzdbSystemDefaultZone().GetCurrentOffsetDateTime();

			Entry = new TextEntry
			{
				Id = now.ToString("uuuu'-'MM'-'dd' T'HH''mm''o<G>", null),

				CreationTime = now,
				LastModified = now,

				Title = "New Entry",
				Body = "Entry body",
			};

			BindingContext = this;
		}

		async void Save_Clicked(object sender, EventArgs e)
		{
			MessagingCenter.Send(this, "AddEntry", Entry);
			await Navigation.PopModalAsync();
		}

		async void Cancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}