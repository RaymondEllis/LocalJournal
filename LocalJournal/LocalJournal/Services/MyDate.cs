using NodaTime;
using NodaTime.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalJournal
{
	/// <summary>
	/// Date helper methods and extensions.
	/// </summary>
	public static class MyDate
	{

		public static string ToUserString(this OffsetDateTime dt)
		{
			// ToDo: Change to users local time
			return dt.ToString("uuuu'-'MM'-'dd' T'HH''mm''o<G>", null);
		}

		public static OffsetDateTime Now()
		{
			return SystemClock.Instance.InTzdbSystemDefaultZone().GetCurrentOffsetDateTime();
		}
	}
}
