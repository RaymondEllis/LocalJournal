using LocalJournal.Models;
using LocalJournal.Services;
using LocalJournal.Views;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LocalJournal.ViewModels
{
	public class TemplatesViewModel : BaseViewModel
	{
		IDataStore<Template> DataStore { get; }

		public ObservableCollection<Template> Items { get; }

		public AsyncCommand LoadCommand => new AsyncCommand(ExecuteLoadCommand);
		public AsyncCommand<Template> DeleteCommand => new AsyncCommand<Template>(ExecuteDeleteCommand);

		public TemplatesViewModel(IDataStore<Template> dataStore)
		{
			DataStore = dataStore;
			Items = new ObservableCollection<Template>();

			MessagingCenter.Subscribe<TemplateEditPage, Template>(this, "Update", ExecuteUpdateMessage);
		}

		private async void ExecuteUpdateMessage(TemplateEditPage sender, Template item)
		{
			await DataStore.UpdateAsync(item);
			await LoadCommand.ExecuteAsync();
		}

		private async Task ExecuteLoadCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				Items.Clear();
				var templates = await DataStore.GetAllAsync(true);
				foreach (var item in templates)
				{
					Items.Add(item);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task ExecuteDeleteCommand(Template item)
		{
			await DataStore.DeleteAsync(item);
		}

	}
}
