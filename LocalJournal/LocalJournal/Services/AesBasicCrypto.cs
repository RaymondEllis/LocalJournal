using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LocalJournal.Services
{
	public class AesBasicCrypto : ICrypto
	{

		public async Task<string> Decrypt(string str)
		{
			if (string.IsNullOrEmpty(str))
				return str;

			var key = await GetKey();
			if (key is null || key.Length <= 0)
				throw new InvalidPasswordExecption($"Cannot decrypt, key / password is empty.");

			try
			{
				return await DecryptStringFromBase64String_Aes(str, key);
			}
			catch (CryptographicException)
			{
				throw;
			}
		}

		public async Task<string> Encrypt(string str)
		{
			if (string.IsNullOrEmpty(str))
				return str;

			var key = await GetKey();
			if (key is null || key.Length <= 0)
				throw new InvalidPasswordExecption($"Cannot encrypt, key / password is empty.");

			using var aes = Aes.Create();
			aes.Key = key;

			try
			{
				return await EncryptStringToBase64String_Aes(str, aes.Key, aes.IV);
			}
			catch { throw; }
		}

		public virtual async Task<bool> HasKey()
		{
			return !string.IsNullOrEmpty(await SecureStorage.GetAsync("encryption_key"));
		}

		public async Task StoreKey(string password)
		{
			var key = Convert.ToBase64String(CreateKey(password));
			await SecureStorage.SetAsync("encryption_key", key);
		}

		protected virtual async Task<byte[]?> GetKey()
		{
			var key64 = await SecureStorage.GetAsync("encryption_key");
			if (string.IsNullOrEmpty(key64))
				return null;
			else
				return Convert.FromBase64String(key64);
		}

		protected static byte[] CreateKey(string password, int keyBytes = 32)
		{
			byte[] salt = new byte[] { 82, 76, 64, 51, 48, 37, 25, 13 };
			int iterations = 1000;
			using var keyGenerator = new Rfc2898DeriveBytes(password, salt, iterations);
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
			if (plainText.Length <= 0)
				throw new ArgumentNullException(nameof(plainText));
			if (key.Length <= 0)
				throw new InvalidPasswordExecption($"Cannot encrypt, key / password is empty.");
			if (IV.Length <= 0)
				throw new ArgumentNullException(nameof(IV));

			// Convert plain text into byte array.
			var bytes = Encoding.UTF8.GetBytes(plainText);

			// Create an AES object with the specified key and IV.
			using var aesAlg = Aes.Create();
			aesAlg.Key = key;
			aesAlg.IV = IV;
			aesAlg.Padding = PaddingMode.PKCS7;

			// Create an encryptor to perform the stream transform.
			var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for encryption.
			using var ms = new MemoryStream();
			using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

			await cs.WriteAsync(bytes, 0, bytes.Length);
			cs.FlushFinalBlock();

			return ms.ToArray();
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
			if (cipherText.Length <= 0)
				throw new ArgumentNullException(nameof(cipherText), $"Cannot decrypt, missing encrypted text to decrypt. Data may be corrupt!");
			if (key.Length <= 0)
				throw new InvalidPasswordExecption($"Cannot decrypt, key / password is empty.");
			if (IV.Length <= 0)
				throw new ArgumentNullException(nameof(IV), $"Cannot decrypt, missing {nameof(IV)}. Data may be corrupt!");

			// Create an AES object with the specified key and IV.
			using var aesAlg = Aes.Create();
			aesAlg.Key = key;
			aesAlg.IV = IV;
			aesAlg.Padding = PaddingMode.PKCS7;

			// Create a decryptor to perform the stream transform.
			var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

			try
			{
				// Create the streams used for decryption.
				using var mc = new MemoryStream(cipherText);
				using var cs = new CryptoStream(mc, decryptor, CryptoStreamMode.Read);
				using var sr = new StreamReader(cs);

				return await sr.ReadToEndAsync();
			}
			catch (Exception ex)
			{
				throw new InvalidPasswordExecption($"Password invalid or {ex.Message}", ex);
			}
		}
	}
}
