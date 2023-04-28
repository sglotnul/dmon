using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Dmon
{
    public class MainForm : Form
    {
        private readonly TableTabControl _tabControl;

        public MainForm(SqlConnection connection, string username)
        {
            var tableBuilder = new TableBuilder(connection);
            var roleRepository = new RolesRepository(username, connection);

            var tableViews = new TableView[]
            {
                new TableView("Категории", tableBuilder.BuildCategoriesTable(), roleRepository),
                new TableView("Продукты", tableBuilder.BuildProductsTable(), roleRepository),
                new TableView("Магазины", tableBuilder.BuildStoresTable(), roleRepository),
                new TableView("Отношения: продукт - магазин", tableBuilder.BuildProductToStoreTable(), roleRepository),
                new TableView("Пользователи", tableBuilder.BuildUsersTable(), roleRepository),
                new TableView("Роли", tableBuilder.BuildRolesTable(), roleRepository),
                new TableView("Отношения: роль - пользователь", tableBuilder.BuildRoleToUserTable(), roleRepository),
                new TableView("Отношения: роль - таблица", tableBuilder.BuildRoleToTableTable(), roleRepository),
            };

            _tabControl = new TableTabControl(tableViews);

            Initialize();
        }

        private void Initialize()
        {
            SuspendLayout();

            _tabControl.Initialize();

            Controls.Add(_tabControl);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1085, 597);

            ResumeLayout(false);
        }

        private new void SuspendLayout()
        {
            _tabControl.SuspendLayout();
            base.SuspendLayout();
        }

        private new void ResumeLayout(bool performLayout)
        {
            _tabControl.ResumeLayout(performLayout);
            base.ResumeLayout(performLayout);
        }
    }
}
