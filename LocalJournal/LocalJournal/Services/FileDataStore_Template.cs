using LocalJournal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LocalJournal.Services
{
	public class FileDataStore_Template : FileDataStore<Template>
	{
		public FileDataStore_Template()
			: base(new FolderQuery(DependencyService.Get<IFileSystem>(), "templates", ".yaml"),
				  DependencyService.Get<IDataSerializer<Template>>())
		{
		}

		protected override async Task AssignUniqueId(Template item)
		{
			item.Id = await VerifyUniqueId(item.Description);
		}

		protected override IOrderedEnumerable<Template> Order(List<Template> items)
		{
			return items.OrderByDescending(i => i.Description);
		}

		protected override void OnUpdated(Template item)
		{

		}
	}
}
