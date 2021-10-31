using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LocalJournal.Services
{
	/// <summary>
	/// Uses reflection to cache all types that <see cref="T.IsAssignableFrom"/>.
	/// The cache is then used later to create or get types from the full type name.
	/// </summary>
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

		/// <summary>
		/// Gets a new list of all the full names for type <typeparamref name="T"/>.
		/// </summary>
		public static IList<string> GetNames()
		{
			if (Cache is null)
				Cache = BuildCache();

			return Cache.Keys.ToList();
		}

		/// <summary>
		/// Tries to get type of <paramref name="fullName"/>.
		/// </summary>
		public static bool TryGetType(string fullName, out Type result)
		{
			if (Cache is null)
				Cache = BuildCache();

			return Cache.TryGetValue(fullName, out result);
		}

		/// <summary>
		/// Tries to create instance of the type from <paramref name="fullName"/> and casts to type of <typeparamref name="T"/>.
		/// </summary>
		public static bool TryCreate(string fullName, out T? result)
		{
			if (TryGetType(fullName, out var type))
				result = Activator.CreateInstance(type) as T;
			else
				result = null;

			return result != null;
		}
	}
}
