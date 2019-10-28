using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Windows.Input;
using Xamarin.Essentials;

namespace LocalJournal.ViewModels
{
	public class AboutViewModel : BaseViewModel
	{
		public AboutViewModel()
		{
			Title = "About";

			OpenGitHubCommand = new AsyncCommand(() => Launcher.OpenAsync(new Uri("https://github.com/RaymondEllis/LocalJournal")));
		}

		public ICommand OpenGitHubCommand { get; }
	}
}