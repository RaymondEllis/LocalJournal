using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LocalJournal.Services
{
	class AesBasicCrypto : ICrypto
	{
		private const string StorageKey = "encryption_key";

		public async Task<string> Decrypt(string str)
		{
			var key = await GetKey();

			return await DecryptStringFromBase64String_Aes(str, key);
		}

		public async Task<string> Encrypt(string str)
		{
			var key = await GetKey();


			using (var aes = Aes.Create())
			{
				aes.Key = key;

				return await EncryptStringToBase64String_Aes(str, aes.Key, aes.IV);
			}

			throw new NotImplementedException();
		}

		public async Task<bool> Unlock()
		{
			return await Task.FromResult(false);

		}

		static async Task<byte[]> GetKey()
		{
			try
			{
				return Convert.FromBase64String(await SecureStorage.GetAsync(StorageKey));
			}
			catch
			{
				return null;
			}
		}

		static async Task<string> EncryptStringToBase64String_Aes(string plainText, byte[] key, byte[] IV)
		{
			var encrypted = await EncryptStringToBytes_Aes(plainText, key, IV);

			return $"{Convert.ToBase64String(encrypted)};{Convert.ToBase64String(IV)}";
		}
		static async Task<byte[]> EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] IV)
		{
			// Check arguments.
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");

			// Create an Aes object
			// with the specified key and IV.
			using (var aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.IV = IV;

				// Create an encryptor to perform the stream transform.
				var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption.
				using (var msEncrypt = new MemoryStream())
				{
					using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					using (var swEncrypt = new StreamWriter(csEncrypt))
						await swEncrypt.WriteAsync(plainText);

					return msEncrypt.ToArray();
				}
			}
		}

		static async Task<string> DecryptStringFromBase64String_Aes(string data, byte[] key)
		{
			var splitIndex = data.IndexOf(';');
			var encryptedString = data.Substring(0, splitIndex);
			var ivString = data.Substring(splitIndex + 1);

			return await DecryptStringFromBytes_Aes(Convert.FromBase64String(encryptedString), key, Convert.FromBase64String(ivString));
		}
		static async Task<string> DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] IV)
		{
			// Check arguments.
			if (cipherText == null || cipherText.Length <= 0)
				throw new ArgumentNullException("cipherText");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");

			// Create an Aes object
			// with the specified key and IV.
			using (var aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.IV = IV;

				// Create a decryptor to perform the stream transform.
				var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for decryption.
				using (var msDecrypt = new MemoryStream(cipherText))
				using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
				using (var srDecrypt = new StreamReader(csDecrypt))
					return await srDecrypt.ReadToEndAsync();
			}
		}
	}
}
