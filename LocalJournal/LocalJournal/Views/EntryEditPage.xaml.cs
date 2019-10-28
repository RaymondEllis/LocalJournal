using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LocalJournal.Models;
using LocalJournal.ViewModels;
using LocalJournal.Services;

namespace LocalJournal.Views
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class EntryEditPage : ContentPage
	{
		readonly EntryEditViewModel viewModel;

		TextEntry Entry => viewModel.Entry;

		public EntryEditPage(TextEntry entry)
		{
			InitializeComponent();

			BindingContext = viewModel = new EntryEditViewModel(entry);

			Encrypted.IsToggled = Entry.Encrypted;

			MessagingCenter.Subscribe<App>(this, "OnResume", OnResume);
		}

		protected async void OnResume(App sender)
		{
			if (Entry.Encrypted)
			{
				var UILock = DependencyService.Get<ILock>();
				if (!await UILock.UnlockAsync())
				{
					await Navigation.PopModalAsync();
				}
			}
		}

		protected override void OnDisappearing()
		{
			MessagingCenter.Unsubscribe<App>(this, "OnResume");
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
			if (e.Value == Entry.Encrypted)
				return;

			// Before enabling encryption, check to make sure it's setup.
			if (e.Value)
			{
				var cryptro = DependencyService.Get<ICrypto>();

				if (!await cryptro.HasKey())
				{
					await DisplayAlert("Password not found!", "Please make sure encryption is setup.", "OK");
					await Navigation.PushModalAsync(new NavigationPage(new CryptoSetupPage()));
				}
				else
					Entry.Encrypted = true;
			}
			// Alert the user when they are disabling encryption.
			else
			{
				if (await DisplayAlert("Disable encryption", "Are you sure you want to disable encryption?", "YES", "NO"))
				{
					Entry.Encrypted = false;
				}
			}
			Encrypted.IsToggled = Entry.Encrypted;
		}
	}
}