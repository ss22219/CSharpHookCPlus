using System;
using System.Runtime.InteropServices;

namespace SharpLib.Dto
{
    [StructLayout(LayoutKind.Sequential)]
    struct NativeAddress
    {
        public IntPtr DemoClass_GetInstance;
        public IntPtr DemoClass_SendMsg;

    }
}
