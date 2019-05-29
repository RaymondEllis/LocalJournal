using System;

using LocalJournal.Models;

namespace LocalJournal.ViewModels
{
	public class EntryDetailViewModel : BaseViewModel
	{
		public TextEntry Entry { get; set; }
		public EntryDetailViewModel(TextEntry entry = null)
		{
			Title = entry?.Title;
			Entry = entry;
		}
	}
}
