using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Dmon
{
    internal class NullableControlWithValue : GroupedControlWithValue
    {
        private CheckBox _checkBox;

        public NullableControlWithValue(Control control, Func<Control, object> getValue) : base(control, getValue) 
        {
            GetValue = c => _checkBox.Checked ? getValue(c) : null;
        }

        protected override void Initialize()
        {
            _checkBox = new CheckBox();
            InitializeCheckbox();

            base.Initialize();

            Controls.Add(_checkBox);
        }

        private void InitializeCheckbox()
        {
            _checkBox.Dock = DockStyle.Top;
            _checkBox.Text = "Установить вручную";
            _checkBox.Checked = true;
            _checkBox.CheckedChanged += OnCheckedChanged;
        }

        private void OnCheckedChanged(object sender, EventArgs args)
        {
            Control.Enabled = _checkBox.Checked;
        }
    }
}
