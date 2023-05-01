using System.Collections.Generic;
using System.Data.SqlClient;

using Dmon.Model;

namespace Dmon
{
    internal class TableBuilder
    {
        private readonly SqlConnection _connection;

        public TableBuilder(SqlConnection connection)
        {
            _connection = connection;
        }

        public Table BuildCategoriesTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "name", new ColumnConfiguration(ColumnDataType.String, "Название", isPrimaryKey: true) },
            };

            return new Table(_connection, "Categories", configuration);
        }

        public Table BuildProductsTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "id", new ColumnConfiguration(ColumnDataType.Int, "Артикул", isPrimaryKey: true, isAutoField: true) },
                { "name", new ColumnConfiguration(ColumnDataType.String, "Название") },
                { "price", new ColumnConfiguration(ColumnDataType.Double, "Цена") },
                { "categoryName", new ColumnConfiguration(ColumnDataType.String, "Категория") }
            };

            return new Table(_connection, "Products", configuration);
        }

        public Table BuildStoresTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "id", new ColumnConfiguration(ColumnDataType.Int, "Id", isPrimaryKey: true, isAutoField: true) },
                { "location", new ColumnConfiguration(ColumnDataType.String, "Местоположение") },
                { "manager", new ColumnConfiguration(ColumnDataType.String, "Менеджер") },
            };

            return new Table(_connection, "Stores", configuration);
        }

        public Table BuildProductToStoreTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "storeId", new ColumnConfiguration(ColumnDataType.Int, "Id магазина", isPrimaryKey: true) },
                { "productId", new ColumnConfiguration(ColumnDataType.Int, "Id продукта", isPrimaryKey: true) },
                { "productCount", new ColumnConfiguration(ColumnDataType.Int, "Количество") }
            };

            return new Table(_connection, "ProductToStore", configuration);
        }

        public Table BuildUsersTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "username", new ColumnConfiguration(ColumnDataType.String, "Имя", isPrimaryKey: true) },
                { "password", new ColumnConfiguration(ColumnDataType.String, "Пароль") },
            };

            return new Table(_connection, "Users", configuration);
        }

        public Table BuildRolesTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "name", new ColumnConfiguration(ColumnDataType.String, "Роль", isPrimaryKey: true) },
                { "permissions", new ColumnConfiguration(ColumnDataType.Int, "Права(bit flags)") },
            };

            return new Table(_connection, "Roles", configuration);
        }

        public Table BuildRoleToUserTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "userName", new ColumnConfiguration(ColumnDataType.String, "Пользователь", isPrimaryKey: true) },
                { "roleName", new ColumnConfiguration(ColumnDataType.String, "Роль", isPrimaryKey: true) },
            };

            return new Table(_connection, "RoleToUser", configuration);
        }

        public Table BuildRoleToTableTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "roleName", new ColumnConfiguration(ColumnDataType.String, "Роль", isPrimaryKey: true) },
                { "tableName", new ColumnConfiguration(ColumnDataType.String, "Таблица", isPrimaryKey: true) },
                { "permissions", new ColumnConfiguration(ColumnDataType.Int, "Права(bit flags)") },
            };

            return new Table(_connection, "RoleToTable", configuration);
        }
    }
}
