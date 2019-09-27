using LocalJournal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet;
using YamlDotNet.Helpers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using NodaTime;
using NodaTime.Text;
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
			TextEntry entry;
			// YAML
			if (tmp == Identifier)
			{
				// Read YAML between the two identifiers.
				var yamlSB = new StringBuilder();
				while (!sr.EndOfStream)
				{
					tmp = sr.ReadLine();
					if (tmp == Identifier)
						break;

					yamlSB.AppendLine(tmp);
				}

				// Deserialize YAML
				var deserializer = new DeserializerBuilder()
					.WithNamingConvention(new CamelCaseNamingConvention())
					.WithTypeConverter(NodaTimeConverter)
					.Build();

				entry = deserializer.Deserialize<TextEntry>(yamlSB.ToString());

				// Read the body
				entry.Id = id;
				entry.Body = ignoreBody ? "" : await sr.ReadToEndAsync();
			}
			// No YAML, read everything to body.
			else
			{
				entry = new TextEntry()
				{
					Id = id,
					Title = "Title not supported, " + id,
					Body = ignoreBody ? "" : tmp + await sr.ReadToEndAsync(),
				};
			}

			// Decrypt the body
			if (entry.Encrypted && !ignoreBody)
			{
				entry.Body = await crypto.Decrypt(entry.Body);
			}

			return entry;
		}

		public async Task<bool> WriteAsync(StreamWriter sw, TextEntry entry)
		{
			var crypto = DependencyService.Get<ICrypto>();
			if (entry.Encrypted)
			{
				if (!await crypto.HasKey())
					return false;
			}

			var serializer = new SerializerBuilder()
				.WithNamingConvention(new CamelCaseNamingConvention())
				.WithTypeConverter(NodaTimeConverter)
				.WithAttributeOverride<TextEntry>(m => m.Body, new YamlIgnoreAttribute())
				.Build();

			await sw.WriteAsync(Identifier);
			await sw.WriteAsync("\n");
			await sw.WriteAsync(serializer.Serialize(entry).ToCrossPlatformEOL());
			await sw.WriteAsync(Identifier);
			await sw.WriteAsync("\n");

			if (entry.Encrypted)
				await sw.WriteAsync(await crypto.Encrypt(entry.Body.ToCrossPlatformEOL()));
			else
				await sw.WriteAsync(entry.Body.ToCrossPlatformEOL());

			return true;
		}
	}
}
