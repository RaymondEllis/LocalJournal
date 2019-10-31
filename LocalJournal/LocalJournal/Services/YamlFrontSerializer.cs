using LocalJournal.Models;
using NodaTime;
using NodaTime.Text;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LocalJournal.Services
{
	public class YamlFrontSerializer : IDataSerializer<TextEntry>
	{
		private const string Identifier = "---";

		private readonly ICrypto Crypto;

		private readonly YamlNodaTimeConverter<OffsetDateTime> NodaTimeConverter =
			new YamlNodaTimeConverter<OffsetDateTime>(OffsetDateTimePattern.CreateWithInvariantCulture("G"));

		public YamlFrontSerializer()
			: this(Xamarin.Forms.DependencyService.Get<ICrypto>())
		{ }
		public YamlFrontSerializer(ICrypto crypto)
		{
			Crypto = crypto;
		}

		public async Task<TextEntry> ReadAsync(StreamReader sr, string id, bool ignoreBody)
		{
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
					.WithNamingConvention(CamelCaseNamingConvention.Instance)
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
					Title = id,
					Body = ignoreBody ? "" : tmp + await sr.ReadToEndAsync(),
				};
			}

			// Decrypt the body
			if (entry.Encrypted && !ignoreBody)
			{
				entry.Body = await Crypto.Decrypt(entry.Body);
			}

			return entry;
		}

		public async Task<bool> WriteAsync(StreamWriter sw, TextEntry entry)
		{
			if (entry.Encrypted)
			{
				if (!await Crypto.HasKey())
					return false;
			}

			var serializer = new SerializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.WithTypeConverter(NodaTimeConverter)
				.WithAttributeOverride(typeof(TextEntry), nameof(entry.Body), new YamlIgnoreAttribute())
				.Build();

			await sw.WriteAsync(Identifier);
			await sw.WriteAsync("\n");
			await sw.WriteAsync(serializer.Serialize(entry).ToCrossPlatformEOL());
			await sw.WriteAsync(Identifier);
			await sw.WriteAsync("\n");

			if (entry.Body != null)
			{
				if (entry.Encrypted)
					await sw.WriteAsync(await Crypto.Encrypt(entry.Body.ToCrossPlatformEOL()));
				else
					await sw.WriteAsync(entry.Body.ToCrossPlatformEOL());
			}

			return true;
		}
	}
}
