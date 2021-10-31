using LocalJournal.Models;
using LocalJournal.Services;
using LocalJournal.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace LocalJournal.Views
{
	public partial class EntryEditPage : ContentPage
	{
		readonly EntryEditViewModel viewModel;

		TextEntry Entry => viewModel.Entry;

		public EntryEditPage(EntryBase? entry)
		{
			InitializeComponent();

			if (entry is TextEntry textEntry)
				BindingContext = viewModel = new EntryEditViewModel(textEntry);
			else if (entry is null)
				BindingContext = viewModel = new EntryEditViewModel(null);
			else
				throw new NotSupportedException(
					$"Currently only TextEntry is supported in the EntryEditPage, but was passed {entry?.GetType().Name}");

			EntryEditView.Encrypted.Toggled += Encrypted_Toggled;
			EntryEditView.Encrypted.IsToggled = Entry.Encrypted;

			MessagingCenter.Subscribe<App>(this, "OnResume", OnResume);
		}

		protected EntryEditPage(EntryEditViewModel viewModel)
		{
			InitializeComponent();

			this.viewModel = viewModel;
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
			MessagingCenter.Send(this, "UpdateEntry", (EntryBase)Entry);
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
					await Navigation.PushModalAsync(new NavigationPage(new SettingsPage()));
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
			EntryEditView.Encrypted.IsToggled = Entry.Encrypted;
		}
	}
}