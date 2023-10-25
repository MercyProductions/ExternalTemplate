using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CSharp_Memory_Class
{
    public class Memory
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public struct MODULEENTRY32
        {
            internal uint dwSize;
            internal uint th32ModuleID;
            internal uint th32ProcessID;
            internal uint GlblcntUsage;
            internal uint ProccntUsage;
            internal IntPtr modBaseAddr;
            internal uint modBaseSize;
            internal IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szExePath;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);
        public static IntPtr OpenProcess(Process proc, uint flags)
        {
            return OpenProcess(flags, false, (uint)proc.Id);
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);

        public static byte ReadByte(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(byte)];
            ReadProcessMemory(hProcess, address, buffer, sizeof(byte), out _);
            return buffer[0];
        }

        public static byte[] ReadBytes(IntPtr hProcess, IntPtr address, int size)
        {
            byte[] buffer = new byte[size];
            ReadProcessMemory(hProcess, address, buffer, size, out _);
            return buffer;
        }

        public static short ReadInt16(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(short)];
            ReadProcessMemory(hProcess, address, buffer, sizeof(short), out _);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static int ReadInt32(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(int)];
            ReadProcessMemory(hProcess, address, buffer, sizeof(int), out _);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static long ReadInt64(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(long)];
            ReadProcessMemory(hProcess, address, buffer, sizeof(long), out _);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static float ReadFloat(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(float)];
            ReadProcessMemory(hProcess, address, buffer, sizeof(float), out _);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static double ReadDouble(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[sizeof(double)];
            ReadProcessMemory(hProcess, address, buffer, sizeof(double), out _);
            return BitConverter.ToDouble(buffer, 0);
        }

        public static string ReadString(IntPtr hProcess, IntPtr address, int maxLength)
        {
            byte[] buffer = new byte[maxLength];
            ReadProcessMemory(hProcess, address, buffer, maxLength, out _);
            int len = Array.IndexOf(buffer, (byte)0); // Find the first null terminator (end of string)
            if (len == -1)
                len = maxLength;
            return Encoding.Unicode.GetString(buffer, 0, len);
        }

        public static string ReadAsciiString(IntPtr hProcess, IntPtr address, int maxLength)
        {
            byte[] buffer = new byte[maxLength];
            ReadProcessMemory(hProcess, address, buffer, maxLength, out _);
            int len = Array.IndexOf(buffer, (byte)0); // Find the first null terminator (end of string)
            if (len == -1)
                len = maxLength;
            return Encoding.ASCII.GetString(buffer, 0, len);
        }

        public static bool WriteByte(IntPtr hProcess, IntPtr address, byte value)
        {
            byte[] buffer = { value };
            return WriteProcessMemory(hProcess, address, buffer, sizeof(byte), out _);
        }

        public static bool WriteBytes(IntPtr hProcess, IntPtr address, byte[] buffer)
        {
            return WriteProcessMemory(hProcess, address, buffer, buffer.Length, out _);
        }

        public static bool WriteInt16(IntPtr hProcess, IntPtr address, short value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(hProcess, address, buffer, sizeof(short), out _);
        }

        public static bool WriteInt32(IntPtr hProcess, IntPtr address, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(hProcess, address, buffer, sizeof(int), out _);
        }

        public static bool WriteInt64(IntPtr hProcess, IntPtr address, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(hProcess, address, buffer, sizeof(long), out _);
        }

        public static bool WriteFloat(IntPtr hProcess, IntPtr address, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(hProcess, address, buffer, sizeof(float), out _);
        }

        public static bool WriteDouble(IntPtr hProcess, IntPtr address, double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(hProcess, address, buffer, sizeof(double), out _);
        }

        public static bool WriteString(IntPtr hProcess, IntPtr address, string value)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(value);
            return WriteProcessMemory(hProcess, address, buffer, buffer.Length, out _);
        }

        public static bool WriteAsciiString(IntPtr hProcess, IntPtr address, string value)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(value);
            return WriteProcessMemory(hProcess, address, buffer, buffer.Length, out _);
        }

        public static IntPtr FindPattern(Process process, byte[] pattern)
        {
            if (process == null || process.HasExited)
                return IntPtr.Zero;

            IntPtr baseAddress = process.MainModule.BaseAddress;
            IntPtr maxAddress = IntPtr.Add(baseAddress, process.MainModule.ModuleMemorySize);

            int patternLength = pattern.Length;
            byte[] buffer = new byte[patternLength];

            for (IntPtr address = baseAddress; address.ToInt64() < maxAddress.ToInt64() - patternLength; address = IntPtr.Add(address, 1))
            {
                buffer = ReadBytes(process.Handle, address, patternLength);
                if (PatternMatches(buffer, pattern))
                    return address;
            }

            return IntPtr.Zero;
        }

        private static bool PatternMatches(byte[] buffer, byte[] pattern)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] != 0x00 && buffer[i] != pattern[i])
                    return false;
            }
            return true;
        }

        public static void GetGameModuleInfo(string gameProcessName, out IntPtr baseAddress, out int moduleSize)
        {
            Process[] processes = Process.GetProcessesByName(gameProcessName);
            baseAddress = IntPtr.Zero;
            moduleSize = 0;

            if (processes.Length == 0)
            {
                Console.WriteLine("Game process not found.");
                return;
            }

            Process gameProcess = processes[0];
            baseAddress = gameProcess.MainModule.BaseAddress;
            moduleSize = gameProcess.MainModule.ModuleMemorySize;

            string moduleName = gameProcess.MainModule.ModuleName;
            string moduleFileName = gameProcess.MainModule.FileName;
            Console.WriteLine($"Game module name: {moduleName}");
            Console.WriteLine($"Game module file path: {moduleFileName}");
        }

        public static IntPtr GetModuleBaseAddress(Process proc, string modName)
        {
            IntPtr addr = IntPtr.Zero;
            foreach (ProcessModule m in proc.Modules)
            {
                if (m.ModuleName == modName)
                {
                    addr = m.BaseAddress;
                    break; ;
                }
            }

            return addr;
        }

        const int INVALID_HANDLE_VALUE = -1;
        public static IntPtr GetModuleBaseAddress(int procId, string modName)
        {
            IntPtr modBaseAddr = IntPtr.Zero;
            IntPtr hSnap = CreateToolhelp32Snapshot(SnapshotFlags.Module | SnapshotFlags.Module32, procId);

            if (hSnap.ToInt64() != INVALID_HANDLE_VALUE)
            {
                MODULEENTRY32 modEntry = new MODULEENTRY32();
                modEntry.dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32));

                if (Module32First(hSnap, ref modEntry))
                {
                    do
                    {
                        if (modEntry.szModule.Equals(modName))
                        {
                            modBaseAddr = modEntry.modBaseAddr;
                            break;
                        }
                    } 
                    
                    while (Module32Next(hSnap, ref modEntry));
                }
            }

            CloseHandle(hSnap); 
            return modBaseAddr;
        }

        public static IntPtr FindDMAAddy(IntPtr hProc, IntPtr ptr, int[] offsets)
        {
            var buffer = new byte[IntPtr.Size];

            foreach (int i in offsets)
            {
                ReadProcessMemory(hProc, ptr, buffer, buffer.Length, out var read);
                ptr = (IntPtr.Size == 4)
                    ? IntPtr.Add(new IntPtr(BitConverter.ToInt32(buffer, 0)), i)
                    : ptr = IntPtr.Add(new IntPtr(BitConverter.ToInt64(buffer, 0)), i);
            }

            return ptr;
        }

    }
}
