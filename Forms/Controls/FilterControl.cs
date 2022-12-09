using System;
using System.Windows.Forms;

namespace Dmon
{
    internal class FilterControl : GroupedControl
    {
        private readonly Action<Control> _setFiler;
        private CheckBox _checkBox;

        public FilterControl(Control control, Action<Control> setFilter) : base(control)
        {
            _setFiler = setFilter;
        }

        public void SetFilter()
        {
            if (_checkBox.Checked)
                _setFiler.Invoke(Control);
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
            _checkBox.Text = "Использовать фильтр";
            _checkBox.Checked = false;
            _checkBox.CheckedChanged += OnCheckedChanged;

            Control.Enabled = false;
        }

        private void OnCheckedChanged(object sender, EventArgs args)
        {
            Control.Enabled = _checkBox.Checked;
        }
    }
}
