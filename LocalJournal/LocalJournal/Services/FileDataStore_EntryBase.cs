using LocalJournal.Models;
using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LocalJournal.Services
{
	public class FileDataStore_EntryBase : FileDataStore<EntryBase>
	{
		public FileDataStore_EntryBase()
			: base(DependencyService.Get<IFileSystem>(), DependencyService.Get<IDataSerializer<EntryBase>>())
		{
		}

		protected override async Task AssignUniqueId(EntryBase item)
		{
			item.Id =await VerifyUniqueId(item.CreationTime.ToIdString());
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
