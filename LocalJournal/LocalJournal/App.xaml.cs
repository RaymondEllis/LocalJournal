using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LocalJournal.Services;
using LocalJournal.Views;

namespace LocalJournal
{
	public partial class App : Application
	{

		public App()
		{
			InitializeComponent();

			//DependencyService.Register<RawSerializer>();
			DependencyService.Register<FileDataStore_EntryBase>();
			DependencyService.Register<YamlFrontSerializer>();
			DependencyService.Register<AesBasicCrypto>();
			DependencyService.Register<LockBiometricPin>();
			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts

			MessagingCenter.Send(this, "OnStart");
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps

			MessagingCenter.Send(this, "OnSleep");
			DependencyService.Get<ILock>()?.Lock();
		}

		protected override void OnResume()
		{
			// Handle when your app resumes

			MessagingCenter.Send(this, "OnResume");
		}
	}
}
