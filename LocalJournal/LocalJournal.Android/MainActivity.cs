
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using LocalJournal.Services;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;
using Xamarin.Forms;
#nullable enable

namespace LocalJournal.Droid
{
	[Activity(Label = "LocalJournal", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);

			CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
			CrossCurrentActivity.Current.Init(this, savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			DependencyService.Register<FileSystem_Platform>();

			LoadApplication(new App());
		}

		public override void OnRequestPermissionsResult(int requestCode, string[]? permissions, [GeneratedEnum] Permission[]? grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}