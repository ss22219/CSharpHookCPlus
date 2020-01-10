using System;
using System.Runtime.InteropServices;

namespace InjectSharpLib
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct LoadClrLibraryParam
    {
        public IntPtr AssemblyPath;
        public IntPtr ClassName;
        public IntPtr StaticMethodName;
        public IntPtr Argument;
    }
}
