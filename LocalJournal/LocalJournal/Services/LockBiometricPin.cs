using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	class LockBiometricPin : ILock
	{
		private bool IsSetup = false;
		private bool IsLocked = true;


		public void Lock()
		{
			IsLocked = true;
		}

		public async Task<bool> UnlockAsync()
		{
			if (!IsSetup)
			{
				var result = await CrossFingerprint.Current.GetAvailabilityAsync();
				if (result == FingerprintAvailability.Available)
					IsSetup = true;
			}

			if (IsLocked)
			{
				var result = await CrossFingerprint.Current.AuthenticateAsync(new AuthenticationRequestConfiguration("Unlock", ""));
				IsLocked = !result.Authenticated;
			}

			return !IsLocked;
		}
	}
}
