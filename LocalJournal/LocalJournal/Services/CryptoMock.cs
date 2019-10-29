using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public class CryptoMock : ICrypto
	{
		public string Key { get; private set; } = null;

		public Task<string> Decrypt(string str)
		{
			return Task.FromResult(str + "<DECRYPTED>");
		}

		public Task<string> Encrypt(string str)
		{
			return Task.FromResult(str + "<ENCRYPTED>");
		}

		public Task<bool> HasKey()
		{
			return Task.FromResult(Key != null);
		}

		public Task StoreKey(string password)
		{
			Key = password;
			return Task.CompletedTask;
		}
	}
}
