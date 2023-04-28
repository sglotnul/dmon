using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Dmon.Model
{
    internal class InsertQueryProvider : IExecutable<int>
    {
        private readonly SqlConnection _connection;
        private readonly string _command;

        private List<string[]> _whereConditions = new List<string[]>();

        public InsertQueryProvider(SqlConnection connection, string table, Dictionary<string, object> values)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            _connection = connection;
            _command = $"INSERT INTO {table} ({string.Join(",", values.Keys)}) VALUES ({string.Join(",", values.Values.Select(v => v == null ? "NULL" : $"N'{v}'"))})";
            Console.WriteLine(_command);
        }

        public async Task<int> ExecuteAsync()
        {
            _connection.Open();

            var command = _connection.CreateCommand();
            command.CommandText = _command;

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
    }
}
