using System.Windows.Forms;

namespace Dmon
{
    internal class GroupedControl : GroupBox
    {
        public Control Control { get; }

        public GroupedControl(Control control)
        {
            Control = control;

            Initialize();
        }

        protected virtual void Initialize()
        {
            Control.Dock = DockStyle.Top;

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            Controls.Add(Control);
        }
    }
}
