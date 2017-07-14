// Guids.cs
// MUST match guids.h
using System;

namespace Company.HideTitleBar
{
    static class GuidList
    {
        public const string guidHideTitleBarPkgString = "5fa47fb4-6814-4058-a2ac-ca3157d9b654";
        public const string guidHideTitleBarCmdSetString = "0f74c51d-0aae-43c2-9730-0ff6bd141541";

        public static readonly Guid guidHideTitleBarCmdSet = new Guid(guidHideTitleBarCmdSetString);
    };
}