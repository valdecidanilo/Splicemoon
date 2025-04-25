using System;

namespace LenixSO.Logger
{
    [Flags]
    public enum LogFlags
    {
        LOG = 1,
        GET = 2,
        POST = 4,
        ERROR = 8,
    }
}