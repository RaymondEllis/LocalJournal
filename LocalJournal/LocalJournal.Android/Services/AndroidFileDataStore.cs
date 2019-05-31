using Android.Content;
using Android.OS;
using LocalJournal.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LocalJournal.Services
{
	public class AndroidFileDataStore : FileDataStore
	{
		public AndroidFileDataStore()
			: base()
		{
			DataPath = Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, "journal");
		}

		protected override bool CheckPermission()
		{
			// ToDo: Check external permissions.
			return true;
		}
	}
}