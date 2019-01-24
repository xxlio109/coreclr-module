using System;
using System.Runtime.InteropServices;

namespace AltV.Net.Native
{
    internal static partial class Alt
    {
        internal static class Player
        {
            [DllImport(_dllName, CharSet = CharSet.Ansi, CallingConvention = _callingConvention)]
            [return: MarshalAs(UnmanagedType.AnsiBStr)]
            internal static extern String Player_GetName(IntPtr playerPointer);

            [DllImport(_dllName, CharSet = CharSet.Ansi, CallingConvention = _callingConvention)]
            internal static extern void Player_SetName(IntPtr playerPointer, [MarshalAs(UnmanagedType.AnsiBStr)] String name);
        }
    }
}