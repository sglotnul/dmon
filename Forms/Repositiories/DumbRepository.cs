using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dmon
{
    public class DumbRepository
    {
        private readonly SqlConnection _connection;

        public DumbRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<DataTable> GetProductsFromLocation(string location, string field, bool ascending)
        {
            var sql = $@"SELECT p.id, p.name, p.price, p.categoryName FROM 
            (
	            SELECT DISTINCT pts.productId FROM ProductToStore pts
	            LEFT JOIN Stores s ON s.Id = pts.storeId
	            WHERE s.location LIKE N'%{location}%') t
            LEFT JOIN Products p ON p.Id = t.productId
            ORDER BY p.{field} {(ascending ? "ASC" : "DESC")}";

            Console.WriteLine(sql);

            var adaper = new SqlDataAdapter(sql, _connection);
            var ds = new DataSet();

            await Task.Run(() => adaper.Fill(ds));

            return ds.Tables[0];
        }
    }
}