using LocalJournal.Models;
using NodaTime;
using NodaTime.Text;
using System;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LocalJournal.Services
{
	public class TemplateYamlSerializer : IDataSerializer<Template>
	{
		private readonly YamlNodaTimeConverter<OffsetDateTime> NodaTimeConverter =
			new YamlNodaTimeConverter<OffsetDateTime>(OffsetDateTimePattern.CreateWithInvariantCulture("G"));

		public async Task<Template> ReadAsync(StreamReader sr, string id, bool ignoreBody)
		{
			var description = sr.ReadLine();
			description = description.Split(':')[1].Trim();

			var typeStr = sr.ReadLine();
			typeStr = typeStr.Split(':')[1].Trim();

			sr.ReadLine();

			if (ServiceLocatorByType<EntryBase>.TryGetType(typeStr, out var type))
			{
				var deserializer = new DeserializerBuilder()
					.WithNamingConvention(PascalCaseNamingConvention.Instance)
					.WithTypeConverter(NodaTimeConverter)
					.Build();

				var data = await sr.ReadToEndAsync();
				var entryObj = deserializer.Deserialize(data, type);

				if (entryObj is EntryBase entry)
					return new Template(description, entry);

				throw new Exception($"Unable to deserialize template id:{id}, Entry '{entryObj?.GetType().FullName}' is not {nameof(EntryBase)}.");
			}
			throw new Exception($"Unable to deserialize template id:{id}, Type '{typeStr}' unknown.");
		}

		public async Task<bool> WriteAsync(StreamWriter sw, Template template)
		{
			var serializer = new SerializerBuilder()
				.WithNamingConvention(PascalCaseNamingConvention.Instance)
				.WithTypeConverter(NodaTimeConverter)
				.Build();

			sw.Write($"Description: {template.Description}\n");
			sw.Write($"Type: {template.Type.FullName}\n");
			sw.Write($"Entry:\n");

			if (template.Entry != null)
				await sw.WriteAsync(serializer.Serialize(template.Entry).ToCrossPlatformEOL());

			return true;
		}
	}
}
