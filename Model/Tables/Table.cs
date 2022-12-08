using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace Dmon.Model
{
    public class Table : ITable
    {
        private readonly SqlConnection _connection;
        private readonly string _name;

        public IReadOnlyDictionary<string, ColumnConfiguration> ColumnsConfiguration { get; }

        public Table(SqlConnection connection, string name, Dictionary<string, ColumnConfiguration> configuration)
        {
            _connection = connection;
            _name = name;

            ColumnsConfiguration = new ReadOnlyDictionary<string, ColumnConfiguration>(configuration);
        }

        public ITableEngine GetEngine()
        {
            return new TableEngine(_connection, _name);
        }
    }
}
