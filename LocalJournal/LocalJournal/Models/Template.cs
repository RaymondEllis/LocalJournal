using LocalJournal.Services;
using System;

namespace LocalJournal.Models
{
	public class Template : IItem
	{
		public string? Id { get => Description; set => Description = value ?? ""; }
		public string Description { get; set; }
		public Type Type { get; set; }
		public EntryBase? Entry { get; set; }

		public Template(string description, EntryBase entry)
		{
			Description = description;
			Type = entry.GetType();
			Entry = entry;
		}
	}
}
