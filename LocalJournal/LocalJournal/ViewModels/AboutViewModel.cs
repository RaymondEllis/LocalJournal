using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace LocalJournal.ViewModels
{
	public class AboutViewModel : BaseViewModel
	{
		public AboutViewModel()
		{
			Title = "About";

			OpenGitHubCommand = new Command(() => Device.OpenUri(new Uri("https://github.com/RaymondEllis/LocalJournal")));
		}

		public ICommand OpenGitHubCommand { get; }
	}
}