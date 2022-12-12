using System;

namespace Dmon
{
    [Flags]
    internal enum UserPermissions : ushort
    {
        Read = 1,
        Write = 2,
    }
}
