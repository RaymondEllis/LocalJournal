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
		private static ICrypto Crypto => DependencyService.Get<ICrypto>();

		public CryptoSetupPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (await Crypto.HasKey())
				Password.Placeholder = "Has Password";
			else
				Password.Placeholder = "Enter a new password";
		}

		private async void Cancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		private async void Done_Clicked(object sender, EventArgs e)
		{
			await Crypto.StoreKey(Password.Text);

			await Navigation.PopModalAsync();
		}
	}
}