using System.Drawing;
using System.Windows.Forms;

namespace Dmon
{
    internal class TableTabControl : TabControl
    {
        public TableTabControl(Control[] tableViews)
        {
            Controls.AddRange(tableViews);
        }

        public void Initialize()
        {
            Cursor = Cursors.Hand;
            Dock = DockStyle.Fill;
            ItemSize = new Size(58, 18);
            Location = new Point(0, 0);
        }

        public new void SuspendLayout()
        {
            foreach (Control control in Controls)
            {
                control.SuspendLayout();
            }

            base.SuspendLayout();
        }

        public new void ResumeLayout(bool performLayout)
        {
            foreach (Control control in Controls)
            {
                control.ResumeLayout(performLayout);
            }

            base.ResumeLayout(performLayout);
        }
    }
}
