using System.Collections.Generic;

namespace Dmon.Model
{
    public interface ITable
    {
        IReadOnlyDictionary<string, ColumnConfiguration> ColumnsConfiguration { get; }
        ITableEngine GetEngine();
    }
}