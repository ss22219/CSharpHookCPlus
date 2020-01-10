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
        static readonly DemoClass_SendMsgDelegate _msgDelegate = DemoClass_SendMsg;
        public static void DemoClass_SendMsg(IntPtr @this, IntPtr lpNetPacket)
        {
            Console.WriteLine($"Msg Send");
            _methodHook.SourceMethod(@this, lpNetPacket);
        }

        public static void SetHook(IntPtr sendMsgFuncPtr)
        {
            _methodHook = new MethodHook<DemoClass_SendMsgDelegate>(sendMsgFuncPtr, _msgDelegate);
            _methodHook.Enabel();
        }
    }
}
