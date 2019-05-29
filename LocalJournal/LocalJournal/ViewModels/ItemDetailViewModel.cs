using System;

using LocalJournal.Models;

namespace LocalJournal.ViewModels
{
	public class ItemDetailViewModel : BaseViewModel
	{
		public Item Item { get; set; }
		public ItemDetailViewModel(Item item = null)
		{
			Title = item?.Title;
			Item = item;
		}
	}
}
