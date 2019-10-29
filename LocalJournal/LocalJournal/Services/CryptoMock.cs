using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public class CryptoMock : ICrypto
	{
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
			return Task.FromResult(true);
		}

		public Task StoreKey(string password)
		{
			return Task.CompletedTask;
		}
	}
}
