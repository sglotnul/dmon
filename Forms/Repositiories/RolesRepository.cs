using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Dmon
{
    internal class RolesRepository
    {
        private readonly string _user;
        private readonly SqlConnection _connection;

        public RolesRepository(string user, SqlConnection connection)
        {
            _user = user;
            _connection = connection;
        }

        public async Task<UserPermissions> GetUserPermissionsAsync(string table)
        {
            _connection.Open();

            var permissions = 0;

            try
            {
                var command = _connection.CreateCommand();
                command.CommandText = $@"SELECT isnull(t.permissions, 0) & isnull(r.permissions, 0) FROM 
                (SELECT roleName, permissions FROM RoleToTable WHERE tableName = '{table}') t
                FULL JOIN
                    (SELECT Roles.name as name, Roles.permissions as permissions FROM RoleToUser
                    LEFT JOIN Roles ON Roles.name = RoleToUser.roleName
                    WHERE RoleToUser.userName = '{_user}') r
                ON t.roleName = r.name";

                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    permissions |= reader.GetInt32(0);
            }
            finally
            {
                _connection.Close();
            }

            return (UserPermissions)permissions;
        }
    }
}
