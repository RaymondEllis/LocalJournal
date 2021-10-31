using NodaTime.Text;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LocalJournal.Services
{
	/// <summary>
	/// Allows reading and writing NodaTime with YamlDotNet.
	/// </summary>
	public class YamlNodaTimeConverter<T> : IYamlTypeConverter
	{
		private readonly IPattern<T> DateTimePattern;

		public YamlNodaTimeConverter(IPattern<T> pattern)
		{
			DateTimePattern = pattern;
		}

		public bool Accepts(Type type)
		{
			return type == typeof(T);
		}

		public object? ReadYaml(IParser parser, Type type)
		{
			if (!(parser.Current is Scalar parseEvent))
				return null;
			var value = parseEvent.Value;

			var result = DateTimePattern.Parse(value);

			if (!result.Success)
				throw result.Exception;

			parser.MoveNext();

			return result.Value;
		}

		public void WriteYaml(IEmitter emitter, object? value, Type type)
		{
			if (value is T typedValue)
				emitter.Emit(new Scalar(null, null, DateTimePattern.Format(typedValue), ScalarStyle.Any, true, false));
			else
				throw new ArgumentException($"Expecting type {typeof(T)}, but got {value?.GetType()}", nameof(value));
		}
	}
}
