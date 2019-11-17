using System.IO;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public interface IDataSerializer<T>
	{
		Task<T> ReadAsync(StreamReader sr, string id, bool ignoreBody = false);

		Task<bool> WriteAsync(StreamWriter sw, T t);
	}
}
