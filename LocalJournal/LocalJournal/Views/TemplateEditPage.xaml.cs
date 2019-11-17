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
		TemplateEditViewModel ViewModel { get; }
		private bool loaded = false;

		public TemplateEditPage(Template? template)
		{
			InitializeComponent();

			var types = ServiceLocatorByType<EntryBase>.GetNames();
			foreach (var t in types)
				TypePicker.Items.Add(t);

			BindingContext = ViewModel = new TemplateEditViewModel(template);

			AssignEntry(ViewModel.Template.Entry);
			TypePicker.SelectedItem = ViewModel.Template.Entry?.GetType().FullName;
			loaded = true;
		}

		private void TypePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!loaded)
				return;

			// ToDo: Ask user if they want to change. It will reset the template.

			if (ServiceLocatorByType<EntryBase>.TryGet((string)TypePicker.SelectedItem, out var entry))
				AssignEntry(entry);
			else
				AssignEntry(null);
		}

		private void AssignEntry(EntryBase? entry)
		{
			View? newView;
			if (entry is EntryMeta)
				newView = null;
			else
			{
				ViewModel.Template.Entry = entry;
				newView = new TextEntryView()
				{
					BindingContext = new EntryEditViewModel(entry as TextEntry, false)
				};
			}

			EntryFrame.Content = newView;
			EntryFrame.IsVisible = EntryFrame.Content != null;
		}

		private void Save_Clicked(object sender, EventArgs e)
		{
			MessagingCenter.Send(this, "Update", ViewModel.Template);
		}

		private async void Cancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}