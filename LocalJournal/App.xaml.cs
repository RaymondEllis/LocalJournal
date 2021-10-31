using LocalJournal.Services;
using Microsoft.Maui.Controls;
using Application = Microsoft.Maui.Controls.Application;

namespace LocalJournal
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			// TODO: File picker currently broken:
			// https://github.com/dotnet/maui/issues/2194
			// https://github.com/dotnet/maui/issues/2102
			//DependencyService.Register<FileSystem_Platform>();
			//DependencyService.Register<FileDataStore_EntryBase>();
			//DependencyService.Register<FileDataStore_Template>();
			DependencyService.Register<MockDataStore>();

			//DependencyService.Register<RawSerializer>();
			DependencyService.Register<YamlFrontSerializer>();
			DependencyService.Register<TemplateYamlSerializer>();
			DependencyService.Register<AesBasicCrypto>();

			DependencyService.Register<LockBiometricPin>();


			MainPage = new AppShell();
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
