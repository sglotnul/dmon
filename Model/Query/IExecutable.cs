using System.Threading.Tasks;

namespace Dmon.Model
{
    public interface IExecutable<T>
    {
        Task<T> ExecuteAsync();
    }
}
