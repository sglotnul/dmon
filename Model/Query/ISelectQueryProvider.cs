using System.Data;

namespace Dmon.Model
{
    public interface ISelectQueryProvider : IExecutable<DataTable>
    {
        ISelectQueryProvider Where(string field, object value);
        IExecutable<DataTable> OrderBy(string field, bool ascending = true);
        ISelectQueryProvider Greater(string field, object value);
        ISelectQueryProvider Less(string field, object value);
        ISelectQueryProvider Contains(string field, string substring);
    }
}
