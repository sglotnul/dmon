using System.Collections.Generic;

namespace Dmon.Model
{
    public interface ITable
    {
        IReadOnlyDictionary<string, ColumnConfiguration> ColumnsConfiguration { get; }
        IDeleteQueryProvider Delete();
        ISelectQueryProvider Select(params string[] fields);
        IUpdateQueryProvider Update(Dictionary<string, object> values);
        IExecutable<int> Insert(Dictionary<string, object> values);
        IExistsQueryProvider Exists();
    }
}