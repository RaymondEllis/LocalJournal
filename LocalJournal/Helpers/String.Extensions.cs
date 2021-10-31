using System;
using System.Collections.Generic;
using System.Text;

namespace LocalJournal.Services
{
	public static class StringExtensions
	{
		/// <summary>
		/// Converts platform specific new line, to LF.
		/// Replaces CRLF to LF, with an extra replace \r to \n to handle special cases.
		/// </summary>
		public static string ToCrossPlatformEOL(this string src)
		{
			return src.Replace("\r\n", "\n").Replace("\r", "\n");
		}
	}
}
