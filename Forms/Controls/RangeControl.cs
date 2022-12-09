using System;
using System.Windows.Forms;

namespace Dmon
{
    internal class RangeControl : GroupBox
    {
        private readonly NullableControlWithValue _fromControl, _toControl;

        public object[] Range => new object[] { _fromControl.Value, _toControl.Value };

        public RangeControl(Control fromControl, Control toControl, Func<Control, object> getValue)
        {
            _fromControl = new NullableControlWithValue(fromControl, getValue);
            _toControl = new NullableControlWithValue(toControl, getValue);

            Initialize();
        }

        void Initialize()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            _fromControl.Dock = DockStyle.Top;
            _fromControl.Text = "От";

            _toControl.Dock = DockStyle.Top;
            _toControl.Text = "До";

            Controls.Add(_toControl);
            Controls.Add(_fromControl);
        }
    }
}
