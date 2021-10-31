using LocalJournal.Models;
using MvvmHelpers;
using System;

namespace LocalJournal.ViewModels
{
	public class TemplateEditViewModel : BaseViewModel
	{
		public Template Template { get; }

		public TemplateEditViewModel(Template? template)
		{
			if (template != null)
				Template = template;
			else
			{
				Template = new Template("", new TextEntry());
			}
		}
	}
}
