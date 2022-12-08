using System.Windows.Forms;

namespace Dmon
{
    internal class CustomNumericUpDown : NumericUpDown
    {
        public CustomNumericUpDown(int from, int to, int digits = 0)
        {
            Minimum = from;
            Maximum = to;
            DecimalPlaces = digits;
        }

        public CustomNumericUpDown(int digits = 0) : this(int.MinValue, int.MaxValue, digits) { }
    }
}
