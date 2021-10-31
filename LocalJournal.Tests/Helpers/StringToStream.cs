using System;
using System.IO;
using System.Text;

namespace LocalJournal.Tests.Helpers
{
	public static class StringExtensions
	{
		public static StringToStreamReader ToStreamReader(this string value)
		{
			return new StringToStreamReader(value);
		}
	}

	public sealed class StringToStreamReader : IDisposable
	{
		public readonly MemoryStream MemoryStream;
		public readonly StreamReader StreamReader;

		public StringToStreamReader(string value)
		{
			MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value));
			StreamReader = new StreamReader(MemoryStream);
		}

		public void Dispose()
		{
			StreamReader?.Dispose();
			MemoryStream?.Dispose();
		}

		public static implicit operator StreamReader(StringToStreamReader v)
		{
			return v.StreamReader;
		}
	}
}
