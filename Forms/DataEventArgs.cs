using System;
using System.Collections.Generic;

namespace Dmon
{
    internal class DataEventArgs : EventArgs
    {
        public Dictionary<string, object> Data { get; }

        public DataEventArgs(Dictionary<string, object> data)
        {
            Data = data;
        }
    }
}
