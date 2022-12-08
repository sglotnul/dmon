using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmon.Model
{
    public class ColumnConfiguration
    {
        public ColumnDataType ColumnDataType { get; }
        public string ColumnVerboseName { get; }
        public bool IsPrimaryKey { get; }
        public bool IsAutoField { get; }
        public bool IsRequired { get; }

        public ColumnConfiguration(
            ColumnDataType dataType,
            string verboseName,
            bool isPrimaryKey = false,
            bool isAutoField = false,
            bool isRequired = true)
        {
            ColumnDataType = dataType;
            ColumnVerboseName = verboseName;
            IsPrimaryKey = isPrimaryKey;
            IsAutoField = isAutoField;
            IsRequired = isRequired;
        }
    }
}
