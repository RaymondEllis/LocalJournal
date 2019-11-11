using System;
using System.Threading.Tasks;
using Xunit;

namespace LocalJournal.Services.Tests
{
	public class AesBasicCryptoTest
	{
		[Theory]
		[InlineData("v0d92TsCf69vX2A15vxr93AkRESD2C/SFo4H9A3O1aKkxIcUTdpVrxTms7+VD/kdARep5GhTtozWUHv17vWjwgBseseDwIx7k7rmh5VD4CB5tsF06pMW+NCDMgeLtamMV1hB4Lry4MRFFWSJCFimGLvD13ShWVXz71HjrDR6/jnzWFNi1Vo5evmJPJSRdpww;PC5JUG5ZMwNIpiduiqLefQ==")]
		[InlineData("6cyMPQo1hr4CRpNn23jFLw==;R6wVuaDJO8zeWLFoJOMXDA==")]
		public async void Encrypt_Test_Resulting_String_Format(string input)
		{
			var crypto = new AesBasicCrypto_StorageOverride("FakeApples");

			var result = await crypto.Encrypt(input);

			Assert.Equal(2, result?.Split(';').Length);
		}

		[Theory]
		[InlineData("SiyAPMx/t8UE/SW49fRuxg==;l4kXZCPQ3wiQx/02rktqxA==", "FakeApples", "a")]
		[InlineData("e+04+kRCZPDRkI/Diaehxw==;8otbT8KXQkUGrPuISRbDCg==", "FakeApples", "abc")]
		[InlineData("zy+Ev78VxCr6x5hULbH7q2con56pUiRQ3pfsVtgOcmo=;RnUSu9PJcpZ1sxKnNGOEzQ==", "FakeApples", "VfC,Qh7FP_D &% B5L")]
		public async void Decrypt(string encrypted, string password, string decrypted)
		{
			var crypto = new AesBasicCrypto_StorageOverride(password);

			// Used to build the inline data for this test.
			//var r = await crypto.Encrypt(decrypted);
			//System.Diagnostics.Debug.WriteLine($"[InlineData(\"{r}\", \"{password}\" ,\"{decrypted}\")]");

			var result = await crypto.Decrypt(encrypted);

			Assert.Equal(decrypted, result);
		}

		[Theory]
		[InlineData("FakeApples", "")]
		[InlineData("FakeApples", "a")]
		[InlineData("FakeApples", "abc")]
		[InlineData("FakeApples", "VfC,Qh7FP_D &% B5L")]
		[InlineData("FakeApples", "a\nb\rc")]
		[InlineData("FakeApples", "😊🍎🖤✔")]
		public async void Round_Trip(string password, string text)
		{
			var crypto = new AesBasicCrypto_StorageOverride(password);

			var encrypted = await crypto.Encrypt(text);
			if (!string.IsNullOrEmpty(encrypted))
				Assert.NotEqual(text, encrypted);

			var decrypted = await crypto.Decrypt(encrypted!);
			Assert.Equal(text, decrypted);
		}

		[Theory]
		[InlineData("Fakeapples", "FakeApples", "a")]
		[InlineData("fake🍎", "FakeApples", "abc")]
		[InlineData("", "FakeApples", "VfC,Qh7FP_D &% B5L")]
		public async void Decrypt_With_Invalid_Password_Throws_InvalidPasswordException(string decryptPassword, string encryptPassword, string text)
		{
			var crypto = new AesBasicCrypto_StorageOverride(encryptPassword);

			var encrypted = await crypto.Encrypt(text);
			Assert.NotNull(encrypted);
			Assert.False(string.IsNullOrEmpty(encrypted));

			crypto = new AesBasicCrypto_StorageOverride(decryptPassword);

			await Assert.ThrowsAsync<InvalidPasswordExecption>(async () => await crypto.Decrypt(encrypted!));
		}

		[Fact]
		public async void Encrypt_With_Null_Password_Throws_InvalidPasswordException()
		{
			var crypto = new AesBasicCrypto_StorageOverride((byte[]?)null);

			await Assert.ThrowsAsync<InvalidPasswordExecption>(async () => await crypto.Encrypt("abc"));
		}
		[Fact]
		public async void Encrypt_With_Empty_Password_Throws_InvalidPasswordException()
		{
			var crypto = new AesBasicCrypto_StorageOverride(Array.Empty<byte>());

			await Assert.ThrowsAsync<InvalidPasswordExecption>(async () => await crypto.Encrypt("abc"));
		}

		class AesBasicCrypto_StorageOverride : AesBasicCrypto
		{
			private readonly byte[]? key;
			public AesBasicCrypto_StorageOverride(byte[]? key)
				=> this.key = key;

			public AesBasicCrypto_StorageOverride(string password)
				=> key = CreateKey(password);

			public override Task<bool> HasKey()
				=> Task.FromResult(true);

			protected override Task<byte[]?> GetKey()
				=> Task.FromResult(key);
		}
	}
}
