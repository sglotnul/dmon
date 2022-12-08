using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Dmon
{
    internal class GroupedControlWithValue : GroupedControl
    {
        public object Value => GetValue(Control);

        protected Func<Control, object> GetValue { get; set; }

        public GroupedControlWithValue(string name, Control control, Func<Control, object> getValue) : base(control) 
        {
            Name = name;
            GetValue = getValue;
        }
    }
}
