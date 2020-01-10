using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using WindowsApi;

namespace SharpLib.Hook
{
    class CallWndProcHook
    {
        private static IntPtr _nextHook;
        public static void SetHook()
        {
            _nextHook = WindowAPI.SetWindowsHookExA(WinHookType.WH_CALLWNDPROC, CallWndProc, IntPtr.Zero, Process.GetCurrentProcess().Threads[0].Id);
        }

        public static void SendDataMessage(IntPtr hwnd, byte[] messageData)
        {
            var copyData = new CopyDataStruct
            {
                cbData = 0,
                dwData = 0,
                lpData = IntPtr.Zero
            };
            copyData.cbData = messageData.Length;
            copyData.lpData = Marshal.AllocHGlobal(messageData.Length);

            Marshal.Copy(messageData, 0, copyData.lpData, messageData.Length);
            WindowAPI.SendMessage(hwnd, WinMsgType.WM_COPYDATA, 0, ref copyData);
            Marshal.FreeHGlobal(copyData.lpData);
        }

        public static int CallWndProc(int nCode, int wParam, IntPtr lParam)
        {
            var cwpStruct = Marshal.PtrToStructure<CWPStruct>(lParam);
            if (cwpStruct.message == WinMsgType.WM_COPYDATA)
            {
                var msgKind = cwpStruct.wParam;
                var copyData = Marshal.PtrToStructure<CopyDataStruct>(cwpStruct.lParam);
                CopyDataHandler(copyData);
            }
            return WindowAPI.CallNextHookEx(_nextHook, nCode, wParam, lParam);
        }

        private static void CopyDataHandler(CopyDataStruct copyData)
        {
            if (copyData.cbData > 0 && copyData.lpData != IntPtr.Zero)
            {
                var messageData = new byte[copyData.cbData];
                Marshal.Copy(copyData.lpData, messageData, 0, copyData.cbData);
                Console.WriteLine(Encoding.Default.GetString(messageData));
            }
        }
    }
}
