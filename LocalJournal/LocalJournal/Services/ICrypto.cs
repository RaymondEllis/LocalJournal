using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public interface ICrypto
	{
		/// <summary>
		/// Checks if a valid key is on this device.
		/// </summary>
		Task<bool> HasKey();

		/// <summary>
		/// Stores the given <paramref name="password"/> as a hashed key.
		/// </summary>
		Task StoreKey(string password);

		/// <summary>
		/// Encrypt string with stored key.
		/// </summary>
		Task<string> Encrypt(string str);

		/// <summary>
		/// Decrypt string with stored key.
		/// </summary>
		Task<string> Decrypt(string str);
	}
}
