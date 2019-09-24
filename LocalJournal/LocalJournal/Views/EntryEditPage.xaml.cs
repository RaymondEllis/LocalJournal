using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LocalJournal.Models;
using LocalJournal.Services;

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

			if (entry != null)
				Entry = entry;
			else
			{
				var now = MyDate.Now();
				Entry = new TextEntry
				{
					Id = null,

					CreationTime = now,
					LastModified = now,

					Title = "",
					Body = "",
				};
			}

			BindingContext = this;

			// Disable encrypted toggle when we are already encrypting.
			Encrypted.IsEnabled = !Encrypted.IsToggled;
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

		private async void Encrypted_Toggled(object sender, ToggledEventArgs e)
		{
			// Before enabling encryption, check to make sure it's setup.
			if (e.Value)
			{
				var cryptro = DependencyService.Get<ICrypto>();

				if (!await cryptro.Unlock())
				{
					await DisplayAlert("Unable to encrypt", "Please make sure encryption is setup.", "OK");
					Encrypted.IsToggled = false;
					await Navigation.PushModalAsync(new NavigationPage(new CryptoSetupPage()));
				}
			}
		}
	}
}