using LocalJournal.Models;
using LocalJournal.Services;
using LocalJournal.ViewModels;
using LocalJournal.Views.EntryTypes;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocalJournal.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TemplateEditPage : ContentPage
	{
		TemplateViewModel ViewModel { get; }

		public TemplateEditPage()
		{
			InitializeComponent();


			Type initialType = typeof(TextEntry);


			var types = ServiceLocatorByType<EntryBase>.GetNames();
			foreach (var t in types)
				TypePicker.Items.Add(t);

			BindingContext = ViewModel = new TemplateViewModel(initialType);
		}

		private void TypePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			// ToDo: Ask user if they want to change. It will reset the template.

			View? newView;
			if (ServiceLocatorByType<EntryBase>.TryGet((string)TypePicker.SelectedItem, out var entry))
			{
				if (entry is EntryMeta)
					newView = null;
				else
				{
					newView = new TextEntryView()
					{
						BindingContext = new EntryEditViewModel(entry as TextEntry)
					};
				}
			}
			else
				newView = null;
			EntryFrame.Content = newView;
			EntryFrame.IsVisible = newView != null;
		}

		private void Save_Clicked(object sender, EventArgs e)
		{

		}

		private async void Cancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}