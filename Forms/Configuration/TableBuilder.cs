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

        public Table BuildChampionshipsTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "id", new ColumnConfiguration(ColumnDataType.Int, "Id", isPrimaryKey: true, isAutoField: true) },
                { "name", new ColumnConfiguration(ColumnDataType.String, "Имя") },
                { "startsAt", new ColumnConfiguration(ColumnDataType.Date, "Дата начала") },
                { "endsAt", new ColumnConfiguration(ColumnDataType.Date, "Дата окончания", isRequired: false) }
            };

            return new Table(_connection, "Championships", configuration);
        }

        public Table BuildChampionshipStagesTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "championshipId", new ColumnConfiguration(ColumnDataType.Int, "Id чемпионата", isPrimaryKey: true) },
                { "name", new ColumnConfiguration(ColumnDataType.String, "Название стадии", isPrimaryKey: true) },
                { "startsAt", new ColumnConfiguration(ColumnDataType.Date, "Дата начала стадии") },
                { "endsAt", new ColumnConfiguration(ColumnDataType.Date, "Дата окончания стадии", isRequired: false) }
            };

            return new Table(_connection, "ChampionshipStages", configuration);
        }

        public Table BuildMembersTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "id", new ColumnConfiguration(ColumnDataType.Int, "Id участника", isPrimaryKey: true, isAutoField: true) },
                { "name", new ColumnConfiguration(ColumnDataType.String, "Имя участника", isPrimaryKey: true) },
                { "country", new ColumnConfiguration(ColumnDataType.String, "Страна участника") },
            };

            return new Table(_connection, "Members", configuration);
        }

        public Table BuildPlaysTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "id", new ColumnConfiguration(ColumnDataType.Int, "Id", isPrimaryKey: true, isAutoField: true) },
                { "championshipId", new ColumnConfiguration(ColumnDataType.Int, "Id Чемпионата") },
                { "stageName", new ColumnConfiguration(ColumnDataType.String, "Название стадии") },
                { "location", new ColumnConfiguration(ColumnDataType.String, "Место проведения") },
                { "startsAt", new ColumnConfiguration(ColumnDataType.DateTime, "Дата и время начала") },
                { "endsAt", new ColumnConfiguration(ColumnDataType.DateTime, "Дата и время окончания", isRequired: false) },
            };

            return new Table(_connection, "Plays", configuration);
        }

        public Table BuildMemberToPlayTable()
        {
            var configuration = new Dictionary<string, ColumnConfiguration>()
            {
                { "memberId", new ColumnConfiguration(ColumnDataType.Int, "Id игры", isPrimaryKey: true) },
                { "playId", new ColumnConfiguration(ColumnDataType.Int, "Id Чемпионата", isPrimaryKey: true) },
                { "command", new ColumnConfiguration(ColumnDataType.String, "Команда") },
                { "iswinner", new ColumnConfiguration(ColumnDataType.Bool, "Победитель", isRequired: false) }
            };

            return new Table(_connection, "MemberToPlay", configuration);
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
