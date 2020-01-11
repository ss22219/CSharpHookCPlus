using System;
using System.Runtime.InteropServices;

namespace WindowsApi
{
    public class WindowAPI
    {

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookExA(int idHook, SetWindowsHookDelegate lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern long SendMessage(IntPtr hwnd, int msg, int hwndFrom, ref CopyDataStruct  copyData);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string classname, string text);
    }

    /// <summary>
    /// CallWndProc回调函数
    /// </summary>
    /// <param name="nCode">指定挂钩过程是否必须处理消息。如果nCode为HC_ACTION，则挂钩过程必须处理该消息。如果nCode小于零，则挂钩过程必须将消息传递给CallNextHookEx函数，而无需进一步处理，并且必须返回CallNextHookEx返回的值</param>
    /// <param name="wParam">指定消息是否由当前线程发送。如果消息是由当前线程发送的，则它为非零；否则为0。否则为零</param>
    /// <param name="lParam">指向CWPSTRUCT结构的指针，该结构包含有关消息的详细信息</param>
    /// <returns></returns>
    public delegate int SetWindowsHookDelegate(int nCode, int wParam, IntPtr lParam);

    public class WinMsgType
    {
        public const int WM_COPYDATA = 0x004A;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CopyDataStruct
    {
        public uint dwData;
        public int cbData;
        public IntPtr lpData;
    }

    public class WinHookType
    {
        public const int WH_CALLWNDPROC = 4;
        public const int WH_CALLWNDPROCRET = 12;
        public const int WH_CBT = 5;
        public const int WH_DEBUG = 11;
        public const int WM_COPYDATA = 0x004A;
        public const int WH_FOREGROUNDIDLE = 11;
        public const int WH_GETMESSAGE = 3;
        public const int WH_JOURNALPLAYBACK = 1;
        public const int WH_JOURNALRECORD = 0;
        public const int WH_KEYBOARD = 2;
        public const int WH_KEYBOARD_LL = 13;
        public const int WH_MOUSE = 7;
        public const int WH_MOUSE_LL = 14;
        public const int WH_MSGFILTER = -1;
        public const int WH_SHELL = 10;
        public const int WH_SYSMSGFILTER = 160;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CWPStruct
    {
        public IntPtr lParam;
        public int wParam;
        public uint message;
        public IntPtr hwnd;
    }
}
