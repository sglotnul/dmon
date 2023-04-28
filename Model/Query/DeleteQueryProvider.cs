using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Dmon.Model
{
    internal class DeleteQueryProvider : IDeleteQueryProvider
    {
        private readonly SqlConnection _connection;
        private StringBuilder _queryBuilder;

        private List<string[]> _whereConditions = new List<string[]>();

        public DeleteQueryProvider(SqlConnection connection, string table)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            _connection = connection;
            _queryBuilder = new StringBuilder($"DELETE FROM {table}");
        }

        public IDeleteQueryProvider Where(string field, object value)
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
