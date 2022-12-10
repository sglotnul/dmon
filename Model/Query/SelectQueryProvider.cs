using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Dmon.Model
{
    internal class SelectQueryProvider : ISelectQueryProvider
    {
        private readonly SqlConnection _connection;
        private StringBuilder _queryBuilder;
        private string _orderBy = null;
        private string _orderDirection = null;

        private List<string[]> _whereConditions = new List<string[]>();
        private List<string[]> _greaterConditions = new List<string[]>();
        private List<string[]> _lessConditions = new List<string[]>();
        private List<string[]> _containsConditions = new List<string[]>();

        public SelectQueryProvider(SqlConnection connection, string table, string[] fields)
        {
            CultureInfo newCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = newCulture;

            _connection = connection;
            _queryBuilder = new StringBuilder($"SELECT {string.Join(",", fields)} FROM {table}");
        }

        public ISelectQueryProvider Where(string field, object value)
        {
            _whereConditions.Add(new string[] { field, value?.ToString() });

            return this;
        }

        public ISelectQueryProvider Greater(string field, object value)
        {
            _greaterConditions.Add(new string[] { field, value?.ToString() });

            return this;
        }

        public ISelectQueryProvider Less(string field, object value)
        {
            _lessConditions.Add(new string[] { field, value?.ToString() });

            return this;
        }

        public ISelectQueryProvider Contains(string field, string substring)
        {
            _containsConditions.Add(new string[] { field, substring });

            return this;
        }

        public IExecutable<DataTable> OrderBy(string field, bool ascending = true)
        {
            _orderBy = field;

            if (!ascending)
                _orderDirection = " DESC";

            return this;
        }

        public async Task<DataTable> ExecuteAsync()
        {
            MergeConditions();

            if (!string.IsNullOrEmpty(_orderBy))
            {
                _queryBuilder.Append($"\nORDER BY {_orderBy}{_orderDirection}");
            }

            var sql = _queryBuilder.ToString();

            var adaper = new SqlDataAdapter(sql, _connection);
            var ds = new DataSet();

            await Task.Run(() => adaper.Fill(ds));

            return ds.Tables[0];
        }

        private void MergeConditions()
        {
            var s = true;
            MergeWhereConditions(ref s);
            MergeGreaterConditions(ref s);
            MergeLessConditions(ref s);
            MergeContainsConditions(ref s);
        }

        private void MergeWhereConditions(ref bool s)
        {
            if (_whereConditions.Count == 0) return;
            if (s)
            {
                _queryBuilder.Append("\nWHERE ");
                s = false;
            }
            else
            {
                _queryBuilder.Append(" AND ");
            }

            _queryBuilder.Append($"{_whereConditions[0][0]} = '{_whereConditions[0][1]}'");

            for (int i = 1; i < _whereConditions.Count; i++)
            {
                _queryBuilder.Append($" AND {_whereConditions[i][0]} = '{_whereConditions[i][1]}'");
            }
        }

        private void MergeGreaterConditions(ref bool s)
        {
            if (_greaterConditions.Count == 0) return;

            if (s)
            {
                _queryBuilder.Append("\nWHERE ");
                s = false;
            }
            else
            {
                _queryBuilder.Append(" AND ");
            }

            _queryBuilder.Append($"{_greaterConditions[0][0]} >= '{_greaterConditions[0][1]}'");

            for (int i = 1; i < _greaterConditions.Count; i++)
            {
                _queryBuilder.Append($" AND {_greaterConditions[i][0]} >= '{_greaterConditions[i][1]}'");
            }
        }

        private void MergeLessConditions(ref bool s)
        {
            if (_lessConditions.Count == 0) return;

            if (s)
            {
                _queryBuilder.Append("\nWHERE ");
                s = false;
            }
            else
            {
                _queryBuilder.Append(" AND ");
            }

            _queryBuilder.Append($"{_lessConditions[0][0]} <= '{_lessConditions[0][1]}'");

            for (int i = 1; i < _lessConditions.Count; i++)
            {
                _queryBuilder.Append($" AND {_lessConditions[i][0]} <= '{_lessConditions[i][1]}'");
            }
        }

        private void MergeContainsConditions(ref bool s)
        {
            if (_containsConditions.Count == 0) return;
            if (s)
            {
                _queryBuilder.Append("\nWHERE ");
                s = false;
            }
            else
            {
                _queryBuilder.Append(" AND ");
            }

            _queryBuilder.Append($"{_containsConditions[0][0]} LIKE '%{_containsConditions[0][1]}%'");

            for (int i = 1; i < _containsConditions.Count; i++)
            {
                _queryBuilder.Append($" AND {_containsConditions[0][0]} LIKE '%{_containsConditions[0][1]}%'");
            }
        }
    }
}
