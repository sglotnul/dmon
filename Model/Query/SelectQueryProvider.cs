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

        private List<string[]> _whereConditions = new List<string[]>();
        private List<string[]> _inRangeConditions = new List<string[]>();
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
            _whereConditions.Add(new string[] { field, value.ToString() });

            return this;
        }

        public ISelectQueryProvider InRange(string field, object from, object to)
        {
            _inRangeConditions.Add(new string[] { field, from.ToString(), to.ToString() });

            return this;
        }

        public ISelectQueryProvider Contains(string field, string substring)
        {
            _containsConditions.Add(new string[] { field, substring });

            return this;
        }

        public IExecutable<DataTable> OrderBy(string field, bool ascending = true)
        {
            MergeWhereConditions();

            _queryBuilder.Append($"\nORDER BY {field}");
            if (!ascending)
                _queryBuilder.Append(" DESC");

            return this;
        }

        public async Task<DataTable> ExecuteAsync()
        {
            MergeWhereConditions();
            MergeInRangeConditions();
            MergeContainsConditions();

            var sql = _queryBuilder.ToString();

            var adaper = new SqlDataAdapter(sql, _connection);
            var ds = new DataSet();

            await Task.Run(() => adaper.Fill(ds));

            return ds.Tables[0];
        }

        private void MergeWhereConditions()
        {
            if (_whereConditions.Count == 0) return;

            _queryBuilder.Append($"\nWHERE {_whereConditions[0][0]} = '{_whereConditions[0][1]}'");

            for (int i = 1; i < _whereConditions.Count; i++)
            {
                _queryBuilder.Append($" AND {_whereConditions[i][0]} = '{_whereConditions[i][1]}'");
            }
        }

        private void MergeInRangeConditions()
        {
            if (_inRangeConditions.Count == 0) return;

            _queryBuilder.Append($"\nWHERE {_inRangeConditions[0][0]} >= '{_inRangeConditions[0][1]}'"
                + $" AND {_inRangeConditions[0][0]} <= '{_inRangeConditions[0][2]}'");

            for (int i = 1; i < _inRangeConditions.Count; i++)
            {
                _queryBuilder.Append($" AND {_inRangeConditions[i][0]} >= '{_inRangeConditions[i][1]}' "
                    + $"AND {_inRangeConditions[i][0]} <= '{_inRangeConditions[i][2]}'");
            }
        }

        private void MergeContainsConditions()
        {
            if (_containsConditions.Count == 0) return;

            _queryBuilder.Append($"\nWHERE {_containsConditions[0][0]} LIKE '%{_containsConditions[0][1]}%'");

            for (int i = 1; i < _containsConditions.Count; i++)
            {
                _queryBuilder.Append($" AND {_containsConditions[0][0]} LIKE '%{_containsConditions[0][1]}%'");
            }
        }
    }
}
