using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Flux.Helpers
{
    public static class M
    {
        public static T Read<T>(IntPtr address) where T : struct
        {
            return H.DriveRead.Read<T>(address);
        }

        public static byte[] Read(IntPtr address, int nLength)
        {
            return H.DriveRead.Read(address, nLength);
        }

        public static void Write<T>(T value, IntPtr address) where T : struct
        {
            H.DriveRead.Write(value, address);
        }

        public static string ReadStr(IntPtr address, int nLength)
        {
            var buf = H.DriveRead.Read(address, nLength);
            var str = Encoding.Default.GetString(buf);
            return str.Substring(0, Math.Max(str.IndexOf('\0'), 0));
        }
    }

    public class MemoryManager
    {
        private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

        public MemoryManager(int pid)
        {
            Pid = pid;
            Handle = Win32.CreateFile("\\\\.\\AudioService", FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero,
                FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            if (Handle == InvalidHandleValue) return;
            Console.WriteLine(@"Linked");
            Thread.Sleep(500);
        }

        private IntPtr Handle { get; }
        private int Pid { get; }

        private static T GetStructure<T>(byte[] bytes)
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var structure = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }

        public T Read<T>(IntPtr address)
        {
            var size = Marshal.SizeOf(typeof(T));
            var data = Read(address, size);
            return GetStructure<T>(data);
        }

        public void Write<T>(T input, IntPtr address)
        {
            var size = Marshal.SizeOf(input);
            var arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(input, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            WriteMem(arr, address);
        }

        private void WriteMem(IReadOnlyCollection<byte> bytes, IntPtr address)
        {
            var info = new CopyMem();
            var buf = new byte[bytes.Count];
            info.SourcePid = Process.GetCurrentProcess().Id;
            info.TargetPid = Pid;
            info.Size = bytes.Count;
            info.Read = 3; //todo: add another ctl code instead of passing different read value
            info.Addr = address;
            var pinnedByteArr = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var addr = pinnedByteArr.AddrOfPinnedObject();
            info.Value = addr;
            var bytes1 = 0;
            if (info.Read != 0 && address != IntPtr.Zero && (long) address > 0)
            {
                var res = Win32.DeviceIoControl(Handle, CtlCode(0x00000022, 0x905, 2, 0), info, Marshal.SizeOf(info),
                    buf, (uint) buf.Length, ref bytes1, IntPtr.Zero);
            }
        }

        public byte[] Read(IntPtr address, int length)
        {
            var info = new CopyMem();
            var buf = new byte[length];
            info.SourcePid = 0;
            info.TargetPid = Pid;
            info.Size = buf.Length;

            //todo: make param to specify if base is needed
            if (address == H.GameManager || address == H.Renderer || address == H.Interface || address == H.WeaponManager)
                info.Read = 2; //tell driver to get base address
            else
                info.Read = 1;
            info.Addr = address;
            info.Value = IntPtr.Zero;
            var bytes = 0;
            if (info.Read != 0 && address != IntPtr.Zero && (long) address > 0)
            {
                var res = Win32.DeviceIoControl(Handle, CtlCode(0x00000022, 0x905, 2, 0), info, Marshal.SizeOf(info),
                    buf, (uint) length, ref bytes, IntPtr.Zero);
            }

            return buf;
        }

        private static uint CtlCode(uint deviceType, uint function, uint method, uint access)
        {
            return (deviceType << 16) | (access << 14) | (function << 2) | method;
        }

        private struct CopyMem
        {
            public int SourcePid;
            public int TargetPid;
            public int Read;
            public IntPtr Addr;
            public IntPtr Value;
            public int Size;
        }
    }
}