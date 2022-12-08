using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Dmon
{
    public class MainForm : Form
    {
        private readonly TableTabControl _tabControl;

        public MainForm(SqlConnection connection)
        {
            var tableBuilder = new TableBuilder(connection);

            var tableViews = new TableView[]
            {
                new TableView("Чемпионаты", tableBuilder.BuildChampionshipsTable()),
                new TableView("Стадии", tableBuilder.BuildChampionshipStagesTable()),
                new TableView("Участники", tableBuilder.BuildMembersTable()),
                new TableView("Игры", tableBuilder.BuildPlaysTable()),
                new TableView("Отношения: участник - игра", tableBuilder.BuildMemberToPlayTable())
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
