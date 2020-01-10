using System;
using System.Runtime.InteropServices;

namespace SharpLib.Hook
{
    class Native
    {
        [DllImport("CoreClr.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void Hook(ref IntPtr sourceMethod, IntPtr targetMethod);
        [DllImport("CoreClr.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void UnHook(ref IntPtr sourceMethod, IntPtr targetMethod);
    }

    public class MethodHook<T>
    {
        private readonly IntPtr _targetMethodPtr;
        private IntPtr _sourceMethodPtr;
        public T SourceMethod { get; private set; }

        public MethodHook(IntPtr sourceMethodPtr, T targetMethod)
        {
            _targetMethodPtr = Marshal.GetFunctionPointerForDelegate(targetMethod);
            _sourceMethodPtr = sourceMethodPtr;
        }

        public void Enable()
        {
            var sourceMethodPtr = _sourceMethodPtr;
            Native.Hook(ref sourceMethodPtr, _targetMethodPtr);
            if (sourceMethodPtr == IntPtr.Zero)
            {
                Console.WriteLine("Hook Faild!");
                return;
            }   
            _sourceMethodPtr = sourceMethodPtr;
            SourceMethod = Marshal.GetDelegateForFunctionPointer<T>(sourceMethodPtr);
        }

        public void Disable()
        {
            var sourceMethodPtr = _sourceMethodPtr;
            Native.UnHook(ref sourceMethodPtr, _targetMethodPtr);
            if (sourceMethodPtr == IntPtr.Zero)
            {
                Console.WriteLine("Disable Hook Faild!");
                return;
            }
            _sourceMethodPtr = sourceMethodPtr;
        }
    }
}
