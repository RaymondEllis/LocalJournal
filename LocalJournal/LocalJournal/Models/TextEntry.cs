using NodaTime;
using System;

namespace LocalJournal.Models
{
	public class TextEntry : IEquatable<TextEntry>
	{
		public string Id { get; set; }

		public OffsetDateTime CreationTime { get; set; }
		public OffsetDateTime LastModified { get; set; }

		public string Title { get; set; }
		public string Body { get; set; }

		public bool Encrypted { get; set; }

		public bool Equals(TextEntry other)
		{
			return other != null &&
				   Id == other.Id &&
				   CreationTime.Equals(other.CreationTime) &&
				   LastModified.Equals(other.LastModified) &&
				   Title == other.Title &&
				   Body == other.Body &&
				   Encrypted == other.Encrypted;
		}
	}
}