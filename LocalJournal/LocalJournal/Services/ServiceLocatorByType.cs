using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LocalJournal.Services
{
	public static class ServiceLocatorByType<T> where T : class
	{
		private static Dictionary<string, Type>? Cache;

		private static Dictionary<string, Type> BuildCache()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var baseType = typeof(T);
			var types = assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));

			var result = new Dictionary<string, Type>();
			foreach (var t in types)
				result.Add(t.FullName, t);
			return result;
		}

		public static IList<string> GetNames()
		{
			if (Cache is null)
				Cache = BuildCache();

			return Cache.Keys.ToList();
		}

		public static bool TryGetType(string fullName, out Type result)
		{
			if (Cache is null)
				Cache = BuildCache();

			return Cache.TryGetValue(fullName, out result);
		}

		public static bool TryGet(string fullName, out T? result)
		{
			if (Cache is null)
				Cache = BuildCache();

			result = null;
			if (Cache.TryGetValue(fullName, out Type type))
				result = Activator.CreateInstance(type) as T;
			return result != null;
		}
	}
}
