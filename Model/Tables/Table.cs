using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace Dmon.Model
{
    public class Table : ITable
    {
        private readonly SqlConnection _connection;

        public string Name { get; }
        public IReadOnlyDictionary<string, ColumnConfiguration> ColumnsConfiguration { get; }

        public Table(SqlConnection connection, string table, Dictionary<string, ColumnConfiguration> configuration)
        {
            _connection = connection;
            Name = table;

            ColumnsConfiguration = new ReadOnlyDictionary<string, ColumnConfiguration>(configuration);
        }

        public IDeleteQueryProvider Delete()
        {
            return new DeleteQueryProvider(_connection, Name);
        }

        public ISelectQueryProvider Select(params string[] fields)
        {
            return new SelectQueryProvider(_connection, Name, fields);
        }

        public IExecutable<int> Insert(Dictionary<string, object> values)
        {
            return new InsertQueryProvider(_connection, Name, values);
        }

        public IUpdateQueryProvider Update(Dictionary<string, object> values)
        {
            return new UpdateQueryProvider(_connection, Name, values);
        }

        public IExistsQueryProvider Exists()
        {
            return new ExistsQueryProvider(_connection, Name);
        }
    }
}
