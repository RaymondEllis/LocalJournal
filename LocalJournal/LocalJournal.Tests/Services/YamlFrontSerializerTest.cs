using LocalJournal.Models;
using LocalJournal.Tests.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace LocalJournal.Services.Tests
{
	public class YamlFrontSerializerTest
	{
		public static IEnumerable<object[]> ReadData
		{
			get
			{
				return new List<object[]> {
NewReadTest(
@"---
title: Test
---
Test Body",
new TextEntry() {
	Id = "test",
	Title = "Test",
	Body = "Test Body"
}),

NewReadTest(
@"---
title: Test
encrypted: true
---",
new TextEntry() {
	Id = "test",
	Title = "Test",
	Body = "<DECRYPTED>",
	Encrypted = true
}),

NewReadTest(
@"---
title: Test
body: this does not exist.
---
Test Body",
new TextEntry() {
	Id = "test",
	Title = "Test",
	Body = "Test Body"
}),

NewReadTest(
@"Test Body",
new TextEntry() {
	Id = "test",
	Title = "test",
	Body = "Test Body"
}),
			};
			}
		}

		[Theory]
		[MemberData(nameof(ReadData))]
		public async void Read(string input, TextEntry output)
		{
			var serializer = new YamlFrontSerializer(new CryptoMock());
			using var inputStream = input.ToStreamReader();
			var result = await serializer.ReadAsync(inputStream, "test", false);

			Assert.NotNull(result);
			Assert.Equal(output, result);
		}

		private static object[] NewReadTest(string input, TextEntry expected)
			=> new object[] { input, expected };


		public static IEnumerable<object[]> WriteData
		{
			get
			{
				return new List<object[]>
				{
NewWriteTest(
new TextEntry()
{
	Title = "test",
	Body = "Test Body"
},
@"---
id: 
creationTime: 0001-01-01T00:00:00Z
lastModified: 0001-01-01T00:00:00Z
title: test
encrypted: false
---
Test Body"
),

NewWriteTest(
new TextEntry()
{
	Id = "testID",
	Title = "test",
	Body = "Test Body",
	Encrypted = true,
},
@"---
id: testID
creationTime: 0001-01-01T00:00:00Z
lastModified: 0001-01-01T00:00:00Z
title: test
encrypted: true
---
Test Body<ENCRYPTED>"
),
				};
			}
		}

		[Theory]
		[MemberData(nameof(WriteData))]
		public async void Write(TextEntry input, string expected)
		{
			ICrypto cryptro = new CryptoMock();
			await cryptro.StoreKey("some key");

			var serializer = new YamlFrontSerializer(cryptro);

			string result;
			using var ms = new MemoryStream(50);
			{
				using var sw = new StreamWriter(ms);
				{
					Assert.True(await serializer.WriteAsync(sw, input));
					sw.Flush();
				}
				result = Encoding.UTF8.GetString(ms.ToArray());
			}

			Assert.NotEmpty(result);
			Assert.Equal(expected, result);
		}
		private static object[] NewWriteTest(TextEntry input, string expected)
			=> new object[] { input, expected.ToCrossPlatformEOL() };
	}
}
