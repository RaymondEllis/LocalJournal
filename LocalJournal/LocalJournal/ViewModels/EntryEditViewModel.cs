using LocalJournal.Models;
using MvvmHelpers;

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
