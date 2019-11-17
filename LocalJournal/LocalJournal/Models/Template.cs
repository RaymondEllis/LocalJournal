using System;
using System.Collections.Generic;
using System.Text;

namespace LocalJournal.Models
{
	public class Template
	{
		public string Description { get; set; }
		public Type Type { get; set; }
		public EntryBase Entry { get; set; }

		public Template(string description, EntryBase entry)
		{
			Description = description;
			Type = entry.GetType();
			Entry = entry;
		}
	}
}
