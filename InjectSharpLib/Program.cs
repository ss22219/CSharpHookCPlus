using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using WindowsApi;

namespace InjectSharpLib
{
    class Program
    {
        const string InjectNativeDll = "CoreClr.dll";
        const string InjectSharpDll = "SharpLib.dll";
        static void Main(string[] args)
        {
            var pid = Process.GetProcessesByName("DemoWinFormApp").First().Id;
            if (!File.Exists(InjectNativeDll))
            {
                Console.WriteLine($"{InjectNativeDll} not exists!");
                return;
            }
            ProcessAPI.LoadLibrary(InjectNativeDll);
            var module = ProcessAPI.GetProcessModule(Process.GetCurrentProcess().Id).FirstOrDefault(m => m.ModuleName == InjectNativeDll);
            if (module == null)
            {
                Console.WriteLine("locale native dll load failed!");
                return;
            }
            var startProc = ProcessAPI.GetProcAddress(module.BaseAddress, "LoadClrLibrary") - (int)module.BaseAddress;

            RemoteExcuteAPI.InjectDLL(pid, Directory.GetCurrentDirectory() + "\\" + InjectNativeDll);
            WindowsApi.ProcessModule remotModule = null;
            for (int i = 0; i < 10 && remotModule == null; i++)
            {
                 remotModule = ProcessAPI.GetProcessModule(pid).FirstOrDefault(m => m.ModuleName == InjectNativeDll);
                if (remotModule == null)
                    Thread.Sleep(100);
            }
            if (remotModule == null)
            {
                Console.WriteLine("remote native dll load failed!");
                return;
            }
            if (!RemoteExcuteAPI.ExcuteRemoteFunction(pid, remotModule.BaseAddress + (int)startProc, GetParamAddress))
                Console.WriteLine("excute remote function failed!");
        }

        static IntPtr GetParamAddress(IntPtr hndProc)
        {
            var assemblyPath = $"{Directory.GetCurrentDirectory()}\\{InjectSharpDll}";
            var className = "SharpLib.Main";
            var staticMethodName = "Start";
            var argument = "CLR Started!";
            var param = new LoadClrLibraryParam
            {
                AssemblyPath = RemoteExcuteAPI.CopyToRemoteMemory(hndProc, Encoding.Unicode.GetBytes(assemblyPath)),
                ClassName = RemoteExcuteAPI.CopyToRemoteMemory(hndProc, Encoding.Unicode.GetBytes(className)),
                StaticMethodName = RemoteExcuteAPI.CopyToRemoteMemory(hndProc, Encoding.Unicode.GetBytes(staticMethodName)),
                Argument = RemoteExcuteAPI.CopyToRemoteMemory(hndProc, Encoding.Unicode.GetBytes(argument))
            };
            return RemoteExcuteAPI.CopyToRemoteMemory(hndProc, param);
        }
    }
}
