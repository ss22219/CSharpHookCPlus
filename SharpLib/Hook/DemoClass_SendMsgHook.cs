using System;
using System.Runtime.InteropServices;

namespace SharpLib.Hook
{
    //struct NetPacket
    //{
    //    public int length;
    //    public uint timestamp;
    //    public ushort identifier;
    //};

    class DemoClass_SendMsgHook
    {

        static MethodHook<DemoClass_SendMsgDelegate> _methodHook;
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void DemoClass_SendMsgDelegate(IntPtr @this, IntPtr lpNetPacket);

        public static void DemoClass_SendMsg(IntPtr @this, IntPtr lpNetPacket)
        {
            Console.WriteLine($"Msg Send");
            _methodHook.SourceMethod(@this, lpNetPacket);
        }

        public static void Hook(IntPtr sendMsgFuncPtr)
        {
            if (_methodHook == null)
                _methodHook = new MethodHook<DemoClass_SendMsgDelegate>(sendMsgFuncPtr, DemoClass_SendMsg);
            _methodHook.Enable();
        }

        public static void UnHook()
        {
            _methodHook.Disable();
        }
    }
}
