using LocalJournal.Models;
using LocalJournal.Services;
using LocalJournal.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocalJournal.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TemplatesPage : ContentPage
	{
		readonly TemplatesViewModel ViewModel;

		public TemplatesPage()
		{
			InitializeComponent();

			BindingContext = ViewModel = new TemplatesViewModel(DependencyService.Get<IDataStore<Template>>());
		}

		private async void NewTemplate_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new TemplateEditPage(null)));
		}

		private async void DeleteTemplate_Clicked(object sender, EventArgs e)
		{
			if ((sender as BindableObject)?.BindingContext is Template template)
			{
				if (await DisplayAlert("Delete confirmation",
					   $"Are you sure you want to delete\n" +
					   $"the template: {template.Description}", "YES", "NO"))
				{
					await ViewModel.DeleteCommand.ExecuteAsync(template);
				}
			}
			else
				await DisplayAlert("Nothing selected", "Unable to find template to delete.", "OK");
		}

		private async void OnTemplateSelected(object sender, SelectedItemChangedEventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new TemplateEditPage(e.SelectedItem as Template)));
		}

		private async void Cancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}