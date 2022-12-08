using System.Data;

namespace Dmon.Model
{
    public interface ISelectQueryProvider : IExecutable<DataTable>
    {
        ISelectQueryProvider Where(string field, object value);
        IExecutable<DataTable> OrderBy(string field, bool ascending = true);
        ISelectQueryProvider InRange(string field, object from, object to);
        ISelectQueryProvider Contains(string field, string substring);
    }
}
