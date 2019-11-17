using LocalJournal.Services;
using LocalJournal.Views;
using Xamarin.Forms;

namespace LocalJournal
{
	public partial class App : Application
	{

		public App()
		{
			InitializeComponent();


			DependencyService.Register<FileDataStore_EntryBase>();
			DependencyService.Register<FileDataStore_Template>();

			//DependencyService.Register<RawSerializer>();
			DependencyService.Register<YamlFrontSerializer>();
			DependencyService.Register<TemplateYamlSerializer>();
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
