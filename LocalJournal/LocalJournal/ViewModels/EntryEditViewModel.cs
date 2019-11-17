using LocalJournal.Models;
using MvvmHelpers;

namespace LocalJournal.ViewModels
{
	public class EntryEditViewModel : BaseViewModel
	{
		public TextEntry Entry { get; }

		public EntryEditViewModel(TextEntry? entry, bool fromTemplate)
		{
			var now = MyDate.Now();
			if (entry != null)
			{
				Entry = entry;
				if(fromTemplate)
				{
					Entry.Id = null;
					Entry.CreationTime = now;
					Entry.LastModified = now;
				}
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
