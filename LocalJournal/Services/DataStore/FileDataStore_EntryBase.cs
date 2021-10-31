using LocalJournal.Models;
using Microsoft.Maui.Controls;
using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public class FileDataStore_EntryBase : FileDataStore<EntryBase>
	{
		public FileDataStore_EntryBase()
			: base(new FolderQuery(DependencyService.Get<IFileSystem>(), null, ".md"),
				  DependencyService.Get<IDataSerializer<EntryBase>>())
		{
		}

		protected override async Task AssignUniqueId(EntryBase item)
		{
			item.Id = await VerifyUniqueId(item.CreationTime.ToIdString());
		}

		protected override IOrderedEnumerable<EntryBase> Order(List<EntryBase> items)
		{
			return items.OrderByDescending(e => e.CreationTime, OffsetDateTime.Comparer.Instant);
		}

		protected override void OnUpdated(EntryBase item)
		{
			item.LastModified = MyDate.Now();
		}
	}
}
