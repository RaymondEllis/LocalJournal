using LocalJournal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
			await sw.WriteAsync(entry.Body);
			return true;
		}
	}
}
