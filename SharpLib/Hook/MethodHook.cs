using System;
using System.Diagnostics;
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
        private readonly T _targetMethod;
        private IntPtr _sourceMethodPtr;
        public T SourceMethod { get; private set; }

        public MethodHook(IntPtr sourceMethodPtr, T targetMethod)
        {
            _targetMethod = targetMethod;
            _targetMethodPtr = Marshal.GetFunctionPointerForDelegate(_targetMethod);
            _sourceMethodPtr = sourceMethodPtr;
        }

        public void Enable()
        {
            var sourceMethodPtr = _sourceMethodPtr;
            Native.Hook(ref sourceMethodPtr, _targetMethodPtr);
            if (sourceMethodPtr == IntPtr.Zero)
            {
                Debug.WriteLine("Hook Faild!");
                return;
            }   
            _sourceMethodPtr = sourceMethodPtr;
            SourceMethod = Marshal.GetDelegateForFunctionPointer<T>(_sourceMethodPtr);
        }

        public void Disable()
        {
            var sourceMethodPtr = _sourceMethodPtr;
            Native.UnHook(ref sourceMethodPtr, _targetMethodPtr);
            if (sourceMethodPtr == IntPtr.Zero)
            {
                Debug.WriteLine("Disable Hook Faild!");
                return;
            }
            _sourceMethodPtr = sourceMethodPtr;
            SourceMethod = default(T);
        }
    }
}
