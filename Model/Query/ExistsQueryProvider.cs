using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Dmon.Model
{
    internal class ExistsQueryProvider : IExistsQueryProvider
    {
        private readonly SqlConnection _connection;
        private StringBuilder _queryBuilder;
        private bool _constructMode = true;

        public ExistsQueryProvider(SqlConnection connection, string table)
        {
            CultureInfo newCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = newCulture;

            _connection = connection;
            _queryBuilder = new StringBuilder($"SELECT COUNT(*) FROM {table}");
        }

        public IExistsQueryProvider Where(string column, object value)
        {
            if (_constructMode)
            {
                _queryBuilder.Append("\nWHERE ");
                _constructMode = false;
            }
            else
                _queryBuilder.Append(" AND ");

            _queryBuilder.Append($"{column}=N'{value}'");

            return this;
        }

        public async Task<bool> ExecuteAsync()
        {
            _connection.Open();

            var command = _connection.CreateCommand();
            command.CommandText = _queryBuilder.ToString();

            bool exists = false;
            try
            {
                var reader = await command.ExecuteReaderAsync();

                await reader.ReadAsync();

                exists = reader.GetInt32(0) > 0;

                reader.Close();
            }
            finally
            {
                _connection.Close();
            }
            
            return exists;
        }
    }
}
