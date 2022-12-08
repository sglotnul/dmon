using Dmon.Model;

namespace Dmon
{
    internal class CellData
    {
        public ColumnConfiguration Configuration { get; }
        public string ColumnName { get; }
        public object Value { get; }

        public CellData(string columnName, ColumnConfiguration configuration, object value) 
        { 
            ColumnName = columnName;
            Configuration = configuration;
            Value = value;
        }
    }
}
