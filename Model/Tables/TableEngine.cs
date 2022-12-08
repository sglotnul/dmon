using System.Collections.Generic;
using System.Globalization;
using System.Data.SqlClient;

namespace Dmon.Model
{
    internal class TableEngine : ITableEngine
    {
        private readonly SqlConnection _connection;
        private readonly string _table;

        public TableEngine(SqlConnection connection, string table)
        {
            _connection = connection;
            _table = table;
        }

        public IDeleteQueryProvider Delete()
        {
            return new DeleteQueryProvider(_connection, _table);
        }

        public ISelectQueryProvider Select(params string[] fields)
        {
            return new SelectQueryProvider(_connection, _table, fields);
        }

        public IExecutable<int> Insert(Dictionary<string, object> values)
        {
            return new InsertQueryProvider(_connection, _table, values);
        }

        public IUpdateQueryProvider Update(Dictionary<string, object> values)
        {
            return new UpdateQueryProvider(_connection, _table, values);
        }
    }
}
