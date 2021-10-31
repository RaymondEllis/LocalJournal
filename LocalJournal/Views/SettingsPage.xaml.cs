using LocalJournal.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Collections.Generic;

namespace LocalJournal.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		private static ILock Lock => DependencyService.Get<ILock>();
		private static ICrypto Crypto => DependencyService.Get<ICrypto>();

		public SettingsPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (await Crypto.HasKey())
				PasswordButton.Text = "Change Password";
			else
				PasswordButton.Text = "Set Password";
		}

		private async void PasswordButton_Clicked(object sender, EventArgs e)
		{
			if (!await Lock.UnlockAsync())
			{
				await DisplayAlert("Locked", "Unable to change the password, App is locked.", "OK");
				return;
			}

			string password;
			if (PasswordEntry.IsVisible)
				password = PasswordEntry.Text;
			else
				password = await DisplayPromptAsync("Encryption Password", "Set password");
			if (!string.IsNullOrWhiteSpace(password))
			{
				if (WeakPassword(password, out var message))
					if (!await DisplayAlert("Password is weak", message + "\n\nContinue anyway?", "Yes", "No"))
						return;

				PasswordEntry.Text = "";
				await Crypto.StoreKey(password);
			}
		}

		private bool WeakPassword(string password, out string message)
		{
			var messages = new List<string>();

			var length1 = 8;
			var length2 = 16;
			var en_alphabet = "abcdefghijklmnopqrstuvwxyz";
			var en_numbers = "0123456789";
			var en_special = "`~!@#$%^&*()_-+={}[]\\|:;\"'<>,.?/";

			if (password.Length < length1)
				messages.Add($"Less then {length1} characters.");
			if (password.Length < length2)
				messages.Add($"Less then {length2} characters.");
			if (password.IndexOfAny(en_alphabet.ToLower().ToCharArray()) == -1)
				messages.Add($"Does not contain any lower case characters.");
			if (password.IndexOfAny(en_alphabet.ToUpper().ToCharArray()) == -1)
				messages.Add($"Does not contain any upper case characters.");
			if (password.IndexOfAny(en_numbers.ToCharArray()) == -1)
				messages.Add($"Does not contain any numbers.");
			if (password.IndexOfAny(en_special.ToCharArray()) == -1)
				messages.Add($"Does not contain any special characters.");

			message = string.Join("\n", messages);
			return messages.Count > 0;
		}
	}
}