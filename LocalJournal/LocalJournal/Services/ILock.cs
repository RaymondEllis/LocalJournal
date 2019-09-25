using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	interface ILock
	{
		void Lock();
		Task<bool> UnlockAsync();
	}
}
