using NodaTime;
using System;

namespace LocalJournal.Models
{
	public class TextEntry : IEntry
	{
		public string Id { get; set; }

		public OffsetDateTime CreationTime { get; set; }
		public OffsetDateTime LastModified { get; set; }

		public string Title { get; set; }
		public string Body { get; set; }
	}
}