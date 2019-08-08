using LocalJournal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Helpers;
using NodaTime.Text;
using NodaTime;
using Xamarin.Forms;

namespace LocalJournal.Services
{
	class YamlFrontSerializer : IDataSerializer<TextEntry>
	{
		private const string Identifier = "---";

		private readonly YamlNodaTimeConverter<OffsetDateTime> NodaTimeConverter =
			new YamlNodaTimeConverter<OffsetDateTime>(OffsetDateTimePattern.CreateWithInvariantCulture("G"));

		public async Task<TextEntry> ReadAsync(StreamReader sr, string id, bool ignoreBody)
		{
			var crypto = DependencyService.Get<ICrypto>();

			string tmp = sr.ReadLine();
			// Yaml
			if (tmp == Identifier)
			{
				var yamlSB = new StringBuilder();

				while (!sr.EndOfStream)
				{
					tmp = sr.ReadLine();
					if (tmp == Identifier)
						break;

					yamlSB.AppendLine(tmp);
				}

				var deserializer = new DeserializerBuilder()
					.WithNamingConvention(new CamelCaseNamingConvention())
					.WithTypeConverter(NodaTimeConverter)
					.Build();

				var entry = deserializer.Deserialize<TextEntry>(yamlSB.ToString());

				entry.Body = ignoreBody ? "" : await sr.ReadToEndAsync();

				if (entry.Encrypted && !ignoreBody)
				{
					await crypto.Unlock();
					entry.Body = await crypto.Decrypt(entry.Body);
				}

				return entry;
			}
			// No Yaml
			else
			{
				var entry = new TextEntry()
				{
					Id = id,
					Title = "Title not supported, " + id,
					Body = ignoreBody ? "" : tmp + await sr.ReadToEndAsync(),
				};

				if (entry.Encrypted && !ignoreBody)
				{
					await crypto.Unlock();
					entry.Body = await crypto.Decrypt(entry.Body);
				}

				return entry;
			}
		}

		public async Task<bool> WriteAsync(StreamWriter sw, TextEntry entry)
		{
			var crypto = DependencyService.Get<ICrypto>();
			if (entry.Encrypted)
			{
				if (!await crypto.Unlock())
					return false;
			}

			var serializer = new SerializerBuilder()
				.WithNamingConvention(new CamelCaseNamingConvention())
				.WithTypeConverter(NodaTimeConverter)
				.WithAttributeOverride<TextEntry>(m => m.Body, new YamlIgnoreAttribute())
				.Build();

			await sw.WriteLineAsync(Identifier);
			await sw.WriteAsync(serializer.Serialize(entry));
			//serializer.Serialize(sw, entry);
			await sw.WriteLineAsync(Identifier);

			if (entry.Encrypted)
				await sw.WriteAsync(await crypto.Encrypt(entry.Body));
			else
				await sw.WriteAsync(entry.Body);

			return true;
		}
	}
}
