using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsApi
{
    public static class RemoteExcuteAPI
    {
        /// <summary>
        /// 注入模块到远程进程
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="modulePath"></param>
        /// <returns></returns>
        public static bool InjectDLL(int processId, string modulePath)
        {
            return ExcuteRemoteSystemFunction(processId, "kernel32.dll", "LoadLibraryA", Encoding.ASCII.GetBytes(modulePath));
        }

        /// <summary>
        /// 执行远程进程上的系统函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processId">进程id</param>
        /// <param name="moduleName">系统模块名称</param>
        /// <param name="functionName">函数名称</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ExcuteRemoteSystemFunction<T>(int processId, string moduleName, string functionName, T param)
        {
            return ExcuteRemoteSystemFunction(processId, moduleName, functionName, StructToBytes(param, Marshal.SizeOf<T>()));
        }

        /// <summary>
        /// 执行远程进程上的系统函数
        /// </summary>
        /// <param name="processId">进程id</param>
        /// <param name="moduleName">系统模块名称</param>
        /// <param name="functionName">函数名称</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ExcuteRemoteSystemFunction(int processId, string moduleName, string functionName, byte[] param)
        {
            var hndProc = OpenProcessWithRemoteExcute(processId);
            if (hndProc == IntPtr.Zero)
                return false;
            try
            {
                //查找当前应用系统函数地址，本机上所有应用的系统函数地址都是相同的
                var lpFuncAddress = ProcessAPI.GetProcAddress(ProcessAPI.GetModuleHandle(moduleName), functionName);
                if (lpFuncAddress == IntPtr.Zero)
                    return false;
                var lpAddress = CopyToRemoteMemory(hndProc, param);
                if (lpAddress == IntPtr.Zero)
                    return false;
                return ExcuteRemoteFunction(hndProc, lpFuncAddress, lpAddress);
            }
            finally
            {
                ProcessAPI.CloseHandle(hndProc);
            }
        }

        /// <summary>
        /// 执行远程进程上的函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processId">进程id</param>
        /// <param name="lpFuncAddress">函数地址</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ExcuteRemoteFunction<T>(int processId, IntPtr lpFuncAddress, T param)
        {
            return ExcuteRemoteFunction(processId, lpFuncAddress, StructToBytes(param, Marshal.SizeOf<T>()));
        }

        /// <summary>
        /// 执行远程进程上的函数
        /// </summary>
        /// <param name="processId">进程id</param>
        /// <param name="lpFuncAddress">函数地址</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ExcuteRemoteFunction(int processId, IntPtr lpFuncAddress, byte[] param)
        {
            IntPtr hndProc = OpenProcessWithRemoteExcute(processId);
            try
            {
                if (hndProc == IntPtr.Zero)
                    return false;
                var lpAddress = CopyToRemoteMemory(hndProc, param);
                if (lpAddress == IntPtr.Zero)
                    return false;
                return ExcuteRemoteFunction(hndProc, lpFuncAddress, lpAddress);
            }
            finally
            {
                ProcessAPI.CloseHandle(hndProc);
            }
        }

        public static IntPtr CopyToRemoteMemory<T>(IntPtr hndProc, T param)
        {
            return CopyToRemoteMemory(hndProc, StructToBytes(param, Marshal.SizeOf<T>()));
        }

        public static IntPtr OpenProcessWithRemoteExcute(int processId)
        {
            return ProcessAPI.OpenProcess(
                                    ProcessAPI.ProcessAccessFlags.CreateThread | ProcessAPI.ProcessAccessFlags.VirtualMemoryOperation |
                                    ProcessAPI.ProcessAccessFlags.VirtualMemoryRead | ProcessAPI.ProcessAccessFlags.VirtualMemoryWrite
                                    | ProcessAPI.ProcessAccessFlags.QueryInformation
                                    , true, processId);
        }

        public static IntPtr CopyToRemoteMemory(IntPtr hndProc, byte[] data)
        {
            var lpAddress = MemoryAPI.VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)data.Length, (0x1000 | 0x2000), 0X40);
            if (lpAddress == IntPtr.Zero)
                return IntPtr.Zero;
            if (MemoryAPI.WriteProcessMemory(hndProc, lpAddress, data, (uint)data.Length, 0) == 0)
                return IntPtr.Zero;
            return lpAddress;
        }

        /// <summary>
        /// 执行远程进程上的函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processId">线程ID</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="lpFuncAddress">远程函数地址</param>
        /// <param name="GetParamAddress">传入进程句柄，返回参数地址委托</param>
        /// <returns></returns>
        public static bool ExcuteRemoteFunction(int processId, IntPtr lpFuncAddress, Func<IntPtr, IntPtr> GetParamAddress)
        {
            IntPtr hndProc = OpenProcessWithRemoteExcute(processId);
            try
            {
                if (hndProc == IntPtr.Zero)
                    return false;
                if (lpFuncAddress == IntPtr.Zero)
                    return false;
                return ExcuteRemoteFunction(hndProc, lpFuncAddress, GetParamAddress(hndProc));
            }
            finally
            {
                ProcessAPI.CloseHandle(hndProc);
            }
        }

        /// <summary>
        /// 执行远程进程上的函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hndProc">进程句柄</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="lpFuncAddress">远程函数地址</param>
        /// <param name="lpParamAddress">远程参数地址</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ExcuteRemoteFunction(IntPtr hndProc, IntPtr lpFuncAddress, IntPtr lpParamAddress)
        {
            if (hndProc == IntPtr.Zero)
                return false;
            if (lpFuncAddress == IntPtr.Zero)
                return false;
            return ProcessAPI.CreateRemoteThread(hndProc, (IntPtr)null, IntPtr.Zero, lpFuncAddress, lpParamAddress, 0, (IntPtr)null) != IntPtr.Zero;
        }

        /// <summary>
        /// 将struct类型转换为byte[]
        /// </summary>
        public static byte[] StructToBytes(object structObj, int size)
        {
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try//struct_bytes转换
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in StructToBytes ! " + ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
