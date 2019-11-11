using System;

namespace LocalJournal.Models
{
	public class TextEntry : EntryBase, IEquatable<TextEntry>
	{
		/// <summary></summary>
		/// <remarks>Can be null when <see cref="IDataStore"/> is only returning meta data.</remarks>
		public string? Body { get; set; }

		public bool Equals(TextEntry other)
		{
			return other != null &&
				base.Equals(other) &&
				Body == other.Body;
		}

		public override bool Equals(EntryBase other)
		{
			if (other is TextEntry textEntry)
				return Equals(textEntry);
			return false;
		}
	}
}