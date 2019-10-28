using System;
using System.Threading.Tasks;
using LocalJournal.Models;
using LocalJournal.Services;
using Xamarin.Forms;

namespace LocalJournal.ViewModels
{
	public class EntryEditViewModel : BaseViewModel
	{
		public TextEntry Entry { get; }

		public EntryEditViewModel(TextEntry entry = null)
		{
			if (entry != null)
				Entry = entry;
			else
			{
				var now = MyDate.Now();
				Entry = new TextEntry
				{
					Id = null,

					CreationTime = now,
					LastModified = now,

					Title = "",
					Body = "",
				};
			}

			Entry = entry;
			Title = Entry.Title;
		}

	}
}
