using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dmon.Model;

namespace Dmon
{
    internal class SideMenu : Panel
    {
        private static Dictionary<ColumnDataType, Func<Control>> ControlsMapping { get; }
        private static Dictionary<ColumnDataType, Action<ConditionsComponent, Control>> SetFilterMethodsMapping { get; }

        private readonly Button _button;
        private readonly IReadOnlyDictionary<string, ColumnConfiguration> _columnsConfiguration;
        private readonly List<FilterControl> _filterControls;

        private ConditionsComponent _cmp;

        static SideMenu()
        {
            ControlsMapping = new Dictionary<ColumnDataType, Func<Control>>
            {
                { ColumnDataType.DateTime, () => new RangeControl(new DateTimePicker(), new DateTimePicker(), c => ((DateTimePicker)c).Value) },
                { ColumnDataType.Date, () => new RangeControl(new DateTimePicker(), new DateTimePicker(), c => ((DateTimePicker)c).Value) },
                { ColumnDataType.Bool, () => new GroupedControlWithValue(new CheckBox(), c => ((CheckBox)c).Checked) },
                { ColumnDataType.Int, () => new RangeControl(new CustomNumericUpDown(), new CustomNumericUpDown(), c => ((CustomNumericUpDown)c).Value) },
                { ColumnDataType.Double, () => new RangeControl(new CustomNumericUpDown(2), new CustomNumericUpDown(2), c => ((CustomNumericUpDown)c).Value) },
                { ColumnDataType.String, () => new GroupedControlWithValue(new TextBox(), c => ((TextBox)c).Text) },
            };

            SetFilterMethodsMapping = new Dictionary<ColumnDataType, Action<ConditionsComponent, Control>>
            {
                { ColumnDataType.DateTime, (cmp, c) => cmp.RangeConditions[c.Name] = ((RangeControl)c).Range },
                { ColumnDataType.Date, (cmp, c) => cmp.RangeConditions[c.Name] = ((RangeControl)c).Range },
                { ColumnDataType.Bool, (cmp, c) => cmp.EqualsConditions[c.Name] = ((GroupedControlWithValue)c).Value },
                { ColumnDataType.Int, (cmp, c) => cmp.RangeConditions[c.Name] = ((RangeControl)c).Range },
                { ColumnDataType.Double, (cmp, c) => cmp.RangeConditions[c.Name] = ((RangeControl)c).Range },
                { ColumnDataType.String, (cmp, c) => cmp.ContainsConditions[c.Name] = ((GroupedControlWithValue)c).Value?.ToString() },
            };
        }

        public event EventHandler OnFilter
        {
            add => _button.Click += value;
            remove => _button.Click -= value;
        }

        public ConditionsComponent GetFilters()
        {
            _cmp = new ConditionsComponent();

            foreach (var c in _filterControls)
                c.SetFilter();

            return _cmp;
        }

        public SideMenu(IReadOnlyDictionary<string, ColumnConfiguration> columnsConfiguration)
        {
            _columnsConfiguration = columnsConfiguration;
            _filterControls = new List<FilterControl>();
            _button = new Button();

            Initialize();
        }

        private void Initialize()
        {
            Dock = DockStyle.Fill;
            AutoScroll = true;

            Controls.Add(_button);

            InitializeButton();
            InitializeFilters();
        }

        private void InitializeFilters()
        {
            foreach (var column in _columnsConfiguration.Keys)
            {
                var configuration = _columnsConfiguration[column];

                var control = ControlsMapping[configuration.ColumnDataType].Invoke();
                control.Name = column;

                var filterControl = new FilterControl(control, c => SetFilterMethodsMapping[configuration.ColumnDataType].Invoke(_cmp, c))
                {
                    Name = column,
                    Text = configuration.ColumnVerboseName,
                    Dock = DockStyle.Top
                };

                _filterControls.Add(filterControl);
            }

            Controls.AddRange(_filterControls.ToArray());
        }

        private void InitializeButton()
        {
            _button.Text = "Применить";
            _button.Dock = DockStyle.Bottom;
        }
    }
}
