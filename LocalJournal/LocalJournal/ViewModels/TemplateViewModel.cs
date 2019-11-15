using System;
using LocalJournal.Models;
using MvvmHelpers;

namespace LocalJournal.ViewModels
{
	public class TemplateViewModel : BaseViewModel
	{
		public string Description { get; set; } = "";

		public string Type { get; set; }

		public EntryBase? Entry { get; set; }

		public TemplateViewModel(Type initialType)
		{
			Type = initialType.FullName;
		}
	}
}
