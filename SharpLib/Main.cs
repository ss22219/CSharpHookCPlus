using SharpLib.Hook;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace SharpLib
{
    public class Main
    {
        public static int Start(string infomartion)
        {
            try
            {
                Console.WriteLine(infomartion);
                LoadDlls();
                var mainHwnd = Process.GetCurrentProcess().MainWindowHandle;
                CallWndProcHook.SetHook();
                CallWndProcHook.SendDataMessage(mainHwnd, Encoding.Default.GetBytes("CallWndProc Hooked!"));

                var address = AddressManager.GetAddress();
                if (address == null)
                    Console.WriteLine("Addr NotFound!");
                else
                {
                    var nAddr = address.Value;
                    Console.WriteLine($"DemoClass_SendMsg Addr: {nAddr.DemoClass_SendMsg.ToString("X")}");
                    DemoClass_SendMsgHook.SetHook(nAddr.DemoClass_SendMsg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 1;
        }

        private static void LoadDlls()
        {
            var dllDir = Path.GetDirectoryName(typeof(Main).Assembly.Location);
            var dlls = Directory.GetFiles(dllDir, "*.dll");
            foreach (var dll in dlls)
            {
                try
                {
                    Assembly.Load(File.ReadAllBytes(dll));
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
