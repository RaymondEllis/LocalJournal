using LocalJournal.Models;
using System.IO;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	class RawSerializer : IDataSerializer<TextEntry>
	{
		public async Task<TextEntry> ReadAsync(StreamReader sr, string id, bool ignoreBody)
		{
			var entry = new TextEntry()
			{
				Id = id,
				Title = "Title not supported, " + id,
				Body = ignoreBody ? "" : await sr.ReadToEndAsync(),
			};

			return entry;
		}

		public async Task<bool> WriteAsync(StreamWriter sw, TextEntry entry)
		{
			if (entry.Body != null)
				await sw.WriteAsync(entry.Body.ToCrossPlatformEOL());
			return true;
		}
	}
}
