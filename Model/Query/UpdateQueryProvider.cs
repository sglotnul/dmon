using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Dmon.Model
{
    internal class UpdateQueryProvider : IUpdateQueryProvider
    {
        private readonly SqlConnection _connection;
        private StringBuilder _queryBuilder;

        private List<string[]> _whereConditions = new List<string[]>();

        public UpdateQueryProvider(SqlConnection connection, string table, Dictionary<string, object> values)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var pairs = values.Select(pair => $"{pair.Key}={(pair.Value == null ? "NULL" : $"N'{pair.Value}'")}");

            _connection = connection;
            _queryBuilder = new StringBuilder($"UPDATE {table} SET {string.Join(",", pairs)}");
        }

        public IUpdateQueryProvider Where(string field, object value)
        {
            _whereConditions.Add(new string[] { field, value.ToString() });

            return this;
        }

        public async Task<int> ExecuteAsync()
        {
            MergeWhereConditions();

            _connection.Open();

            var command = _connection.CreateCommand();
            command.CommandText = _queryBuilder.ToString();
            Console.WriteLine(command.CommandText);

            int res;

            try
            {
                res = await command.ExecuteNonQueryAsync();
            }
            finally
            {
                _connection.Close();
            }

            return res;
        }

        private void MergeWhereConditions()
        {
            if (_whereConditions.Count == 0) return;

            _queryBuilder.Append($"\nWHERE {_whereConditions[0][0]} = N'{_whereConditions[0][1]}'");

            for (int i = 1; i < _whereConditions.Count; i++)
            {
                _queryBuilder.Append($" AND {_whereConditions[i][0]} = N'{_whereConditions[i][1]}'");
            }
        }
    }
}
