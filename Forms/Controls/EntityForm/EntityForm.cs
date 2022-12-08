using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Dmon.Model;

namespace Dmon
{
    internal class EntityForm : Form
    {
        private static Dictionary<ColumnDataType, Func<object, Control>> ControlsMapping { get; }
        private static Dictionary<ColumnDataType, Func<Control, object>> ValueMethodsMapping { get; }

        private readonly CellData[] _columns;

        private readonly Button _button;
        private readonly List<GroupedControlWithValue> _valueControls;

        static EntityForm()
        {
            ControlsMapping = new Dictionary<ColumnDataType, Func<object, Control>>
            {
                { ColumnDataType.DateTime, v => new DateTimePicker() { Format = DateTimePickerFormat.Time, Value = v is DateTime val ? val : DateTime.Now } },
                { ColumnDataType.Date, v => new DateTimePicker() { Value = v is DateTime val ? val : DateTime.Now } },
                { ColumnDataType.Bool, v => new CheckBox() { Checked = v is bool val && val } },
                { ColumnDataType.Int, v => new CustomNumericUpDown() { Value = v is int val ? val : 0 } },
                { ColumnDataType.Double, v => new CustomNumericUpDown(2) { Value = v is int val ? val : .0m } },
                { ColumnDataType.String, v => new TextBox() { Text = v?.ToString() } },
            };

            ValueMethodsMapping = new Dictionary<ColumnDataType, Func<Control, object>>
            {
                { ColumnDataType.DateTime, c => ((DateTimePicker)c).Value },
                { ColumnDataType.Date, c => ((DateTimePicker)c).Value },
                { ColumnDataType.Bool, c => ((DateTimePicker)c).Checked },
                { ColumnDataType.Int, c => ((CustomNumericUpDown)c).Value },
                { ColumnDataType.Double, c => ((CustomNumericUpDown)c).Value },
                { ColumnDataType.String, c => ((TextBox)c).Text },
            };
        }

        public EntityForm(CellData[] columns)
        {
            _columns = columns;
            _button = new Button();
            _valueControls = new List<GroupedControlWithValue>(_columns.Length);

            Initialize();
        }

        public event DataSaveEventHandler OnSave
        {
            add => _button.Click += (sender, e) => value.Invoke(this, new DataEventArgs(GetValuesToSave()));
            remove => throw new NotImplementedException();
        }

        private Dictionary<string, object> GetValuesToSave()
        {
            var values = new Dictionary<string, object>();

            foreach (var control in _valueControls)
            {
                values.Add(control.Name, control.Value);
            }

            return values;
        }

        private void Initialize()
        {
            SuspendLayout();

            InitializeButton();
            InitializeColumns();

            Controls.Add(_button);

            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(260, 489);
            Padding = new Padding(10, 10, 10, 10);

            ResumeLayout(false);
        }

        private void InitializeColumns()
        {
            foreach(var column in _columns)
            {
                var configuration = column.Configuration;

                if (configuration.IsAutoField) continue;

                var control = ControlsMapping[configuration.ColumnDataType].Invoke(column.Value);

                var controlWithValue = new GroupedControlWithValue(column.ColumnName, control, ValueMethodsMapping[configuration.ColumnDataType]);
                if (!(configuration.IsRequired || configuration.IsPrimaryKey))
                    controlWithValue = new NullableControl(column.ColumnName, control, ValueMethodsMapping[configuration.ColumnDataType]);

                controlWithValue.Text = configuration.ColumnVerboseName;
                controlWithValue.Dock = DockStyle.Top;

                _valueControls.Add(controlWithValue);
            }

            Controls.AddRange(_valueControls.ToArray());
        }

        private void InitializeButton()
        {
            _button.Text = "Сохранить";
            _button.Dock = DockStyle.Bottom;
        }
    }
}
