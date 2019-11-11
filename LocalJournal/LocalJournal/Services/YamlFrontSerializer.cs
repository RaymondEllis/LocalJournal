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
	public class YamlFrontSerializer : IDataSerializer<EntryBase>
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

		public async Task<EntryBase> ReadAsync(StreamReader sr, string id, bool ignoreBody)
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

				if (ignoreBody)
				{
					return deserializer.Deserialize<EntryMeta>(yamlSB.ToString());
				}

				entry = deserializer.Deserialize<TextEntry>(yamlSB.ToString());

				// Read the body
				entry.Id = id;
				entry.Body = await sr.ReadToEndAsync();
			}
			// No YAML, read everything to body.
			else
			{
				if (ignoreBody)
				{
					return new EntryMeta()
					{
						Id = id,
						Title = id,
					};
				}
				else
				{
					entry = new TextEntry()
					{
						Id = id,
						Title = id,
						Body = tmp + await sr.ReadToEndAsync(),
					};
				}
			}

			// Decrypt the body
			if (entry.Encrypted)
			{
				try
				{
					entry.Body = await Crypto.Decrypt(entry.Body);
				}
				catch (InvalidPasswordException)
				{
					throw;
				}
			}

			return entry;
		}


		public async Task<bool> WriteAsync(StreamWriter sw, EntryBase entry)
		{
			if (entry.Encrypted)
			{
				if (!await Crypto.HasKey())
					return false;
			}

			var serializer = new SerializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.WithTypeConverter(NodaTimeConverter)
				.WithAttributeOverride(typeof(TextEntry), nameof(TextEntry.Body), new YamlIgnoreAttribute())
				.Build();

			await sw.WriteAsync(Identifier);
			await sw.WriteAsync("\n");
			await sw.WriteAsync(serializer.Serialize(entry).ToCrossPlatformEOL());
			await sw.WriteAsync(Identifier);
			await sw.WriteAsync("\n");

			if (entry is TextEntry textEntry && textEntry.Body != null)
			{
				if (textEntry.Encrypted)
				{
					try
					{
						await sw.WriteAsync(await Crypto.Encrypt(textEntry.Body.ToCrossPlatformEOL()));
					}
					catch (InvalidPasswordException)
					{
						throw;
					}
				}
				else
				{
					await sw.WriteAsync(textEntry.Body.ToCrossPlatformEOL());
				}
			}

			return true;
		}
	}
}
