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

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (ViewModel.Items.Count == 0)
				await ViewModel.LoadCommand.ExecuteAsync();
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
			if (e.SelectedItem is Template template)
				await Navigation.PushModalAsync(new NavigationPage(new EntryEditPage(template.Entry, true)));

			TemplatesListView.SelectedItem = null;
		}

		private async void Cancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		private async void EditTemplate_Clicked(object sender, EventArgs e)
		{
			if ((sender as BindableObject)?.BindingContext is Template template)
				await Navigation.PushModalAsync(new NavigationPage(new TemplateEditPage(template)));
		}
	}
}