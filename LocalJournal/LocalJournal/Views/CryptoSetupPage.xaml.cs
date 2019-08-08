using LocalJournal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocalJournal.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CryptoSetupPage : ContentPage
	{
		public CryptoSetupPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			try
			{
				Password.Text = await SecureStorage.GetAsync("encryption_key");
			}
			catch
			{ }
		}

		private async void Cancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		private async void Done_Clicked(object sender, EventArgs e)
		{
			try
			{
				await SecureStorage.SetAsync("encryption_key", Password.Text);
			}
			catch
			{ }
			await Navigation.PopModalAsync();
		}
	}
}