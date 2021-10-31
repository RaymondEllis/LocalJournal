using LocalJournal.Models;
using MvvmHelpers;

namespace LocalJournal.ViewModels
{
	public class EntryEditViewModel : BaseViewModel
	{
		public TextEntry Entry { get; }

		public EntryEditViewModel(TextEntry? entry)
		{
			var now = MyDate.Now();
			if (entry != null)
			{
				Entry = entry;
			}
			else
			{
				Entry = new TextEntry
				{
					Id = null,

					CreationTime = now,
					LastModified = now,

					Title = "",
					Body = "",
				};
			}

			Title = Entry.Title;
		}
	}
}
