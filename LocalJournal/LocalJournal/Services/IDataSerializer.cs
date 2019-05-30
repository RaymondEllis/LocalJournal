using System.IO;
using System.Threading.Tasks;

namespace LocalJournal.Services
{
	public interface IDataSerializer<T>
	{
		Task<T> ReadAsync(StreamReader sr, string id);

		Task<bool> WriteAsync(StreamWriter sw, T t);
	}
}
