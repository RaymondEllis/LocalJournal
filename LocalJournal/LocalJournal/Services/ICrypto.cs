using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	interface ICrypto
	{
		/// <summary>
		/// Fetch the key.
		/// If key is encrypted, pop-up biometric, or ask user for password/key.
		/// If never used, pop-up biometric, ask user to generate or enter key.
		/// </summary>
		Task<bool> Unlock();

		Task<string> Encrypt(string str);

		Task<string> Decrypt(string str);
	}
}
