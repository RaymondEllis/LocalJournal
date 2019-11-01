using NodaTime;
using System;

namespace LocalJournal.Models
{
	public class TextEntry : IEquatable<TextEntry>
	{
		/// <summary>
		/// How is this entry known by the <see cref="IDataStore"/>?
		/// </summary>
		/// <remarks>Should be null when not stored in a <see cref="IDataStore"/></remarks>
		public string? Id { get; set; }

		public OffsetDateTime CreationTime { get; set; }
		public OffsetDateTime LastModified { get; set; }

		public string Title { get; set; } = "";

		/// <summary></summary>
		/// <remarks>Can be null when <see cref="IDataStore"/> is only returning meta data.</remarks>
		public string? Body { get; set; }

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