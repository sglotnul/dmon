using System.Collections.Generic;

namespace Dmon
{
    internal class ConditionsComponent
    {
        public Dictionary<string, object[]> RangeConditions { get; } = new Dictionary<string, object[]>();
        public Dictionary<string, string> ContainsConditions { get; } = new Dictionary<string, string>();
        public Dictionary<string, object> EqualsConditions { get; } = new Dictionary<string, object>();
    }
}
