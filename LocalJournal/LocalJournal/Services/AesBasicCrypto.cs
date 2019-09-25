using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LocalJournal.Services
{
	class AesBasicCrypto : ICrypto
	{

		public async Task<string> Decrypt(string str)
		{
			if (string.IsNullOrEmpty(str))
				return str;

			var key = await GetKey();
			if (key == null)
				return null;

			try
			{
				return await DecryptStringFromBase64String_Aes(str, key);
			}
			catch (CryptographicException ex)
			{
				Debug.WriteLine(ex);
				return null;
			}
		}

		public async Task<string> Encrypt(string str)
		{
			if (string.IsNullOrEmpty(str))
				return str;

			var key = await GetKey();
			if (key == null)
				return null;


			using (var aes = Aes.Create())
			{
				aes.Key = key;

				return await EncryptStringToBase64String_Aes(str, aes.Key, aes.IV);
			}

			throw new NotImplementedException();
		}

		public async Task<bool> HasKey()
		{
			return !string.IsNullOrEmpty(await SecureStorage.GetAsync("encryption_key"));
		}

		public async Task StoreKey(string password)
		{
			var key = Convert.ToBase64String(CreateKey(password));
			await SecureStorage.SetAsync("encryption_key", key);
		}

		static async Task<byte[]> GetKey()
		{
			var key64 = await SecureStorage.GetAsync("encryption_key");
			if (string.IsNullOrEmpty(key64))
				return null;
			else
				return Convert.FromBase64String(key64);
		}

		private static byte[] CreateKey(string password, int keyBytes = 32)
		{
			byte[] salt = new byte[] { 82, 76, 64, 51, 48, 37, 25, 13 };
			int iterations = 1000;
			using (var keyGenerator = new Rfc2898DeriveBytes(password, salt, iterations))
				return keyGenerator.GetBytes(keyBytes);
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
				throw new ArgumentNullException("key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");

			// Create an Aes object with the specified key and IV.
			using (var aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.IV = IV;
				aesAlg.Padding = PaddingMode.PKCS7;

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
				throw new ArgumentNullException("key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");

			// Create an Aes object with the specified key and IV.
			using (var aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.IV = IV;
				aesAlg.Padding = PaddingMode.PKCS7;

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
