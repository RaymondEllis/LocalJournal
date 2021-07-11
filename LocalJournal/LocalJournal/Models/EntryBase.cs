using LocalJournal.Services;
using NodaTime;
using System;

namespace LocalJournal.Models
{
	public abstract class EntryBase : IEquatable<EntryBase>, IItem
	{
		/// <summary>
		/// How is this entry known by the <see cref="IDataStore"/>?
		/// </summary>
		/// <remarks>Should be null when not stored in a <see cref="IDataStore"/></remarks>
		public string? Id { get; set; }

		public OffsetDateTime CreationTime { get; set; }
		public OffsetDateTime LastModified { get; set; }

		public string Title { get; set; } = "";

		public bool Encrypted { get; set; }

		/// <summary>
		/// Returns this or does a copy to a new <see cref="EntryMeta"/>
		/// </summary>
		/// <returns></returns>
		public virtual EntryMeta AsMeta()
		{
			if (this is EntryMeta meta)
				return meta;
			return new EntryMeta()
			{
				Id = Id,
				CreationTime = CreationTime,
				LastModified = LastModified,
				Title = Title,
				Encrypted = Encrypted,
			};
		}

		public virtual bool Equals(EntryBase other)
		{
			return Id == other.Id &&
				CreationTime.Equals(other.CreationTime) &&
				LastModified.Equals(other.LastModified) &&
				Title == other.Title &&
				Encrypted == other.Encrypted;
		}


		/// <summary>
		/// Creates a Clone with no Id, and CreationTime and LastModified set to Now.
		/// </summary>
		public virtual EntryBase Clone()
		{
			var now = MyDate.Now();

			var clone = CloneInternal();

			clone.Id = null;
			clone.CreationTime = now;
			clone.LastModified = now;
			clone.Title = Title;
			clone.Encrypted = Encrypted;

			return clone;
		}

		protected internal abstract EntryBase CloneInternal();
	}

	public class EntryMeta : EntryBase
	{
		protected internal override EntryBase CloneInternal()
		{
			return new EntryMeta();
		}
	}
}
