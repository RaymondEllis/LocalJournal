using System;
using System.Collections.Generic;
using System.Text;

namespace LocalJournal.Models
{
	public class Template
	{
		public string Description { get; set; }
		public string Type { get; set; }
		public EntryBase Entry { get; set; }
	}
}
