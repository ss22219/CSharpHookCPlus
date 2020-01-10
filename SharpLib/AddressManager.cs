using SharpLib.Dto;
using SharpLib.Memory;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsApi;

namespace SharpLib
{
    class AddressManager
    {
        private const string DemoClass_SendMsgByteFlags = "C7 46 08 00 00 00 00 56 66 89 4E 08 E8 ?? ?? ?? ?? 8B C8 E8 ?? ?? ?? ??";
        private const string ModuleName = "DemoNativeLib.dll";
        private static NativeAddress? _nAddr = null;
        public static NativeAddress? GetAddress()
        {
            if (_nAddr == null)
            {
                var module = ProcessAPI.GetProcessModule(Process.GetCurrentProcess().Id).FirstOrDefault(m => m.ModuleName == ModuleName);
                if (module == null)
                    return null;
                var addr = new NativeAddress();
                var positions = MemorySearch.FindPositions(DemoClass_SendMsgByteFlags, module.BaseAddress, module.ModuleSize);
                if (positions != null)
                {
                    addr.DemoClass_GetInstance = (IntPtr)unchecked((uint)positions[0] + Marshal.PtrToStructure<uint>(positions[1]) + 4);
                    addr.DemoClass_SendMsg = (IntPtr)unchecked((uint)positions[1] + Marshal.PtrToStructure<uint>(positions[1]) + 4);
                    _nAddr = addr;
                }
                else
                    return null;
            }
            return _nAddr.Value;
        }
    }
}
