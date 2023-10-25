using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Memory_Class.Anti_Debug
{
    public static class AntiVM
    {
        public static bool IsRunningInVirtualMachine()
        {
            // Check for virtualization-specific CPU instructions
            if (System.Environment.Is64BitOperatingSystem && System.Environment.Is64BitProcess)
            {
                // x64 mode
                byte[] vmCheckBytes = { 0x0F, 0x3F, 0xC1 }; // cpuid
                byte[] buffer = new byte[3];

                try
                {
                    using (FileStream fs = new FileStream(@"C:\pagefile.sys", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        fs.Seek(0x7FFD, SeekOrigin.Begin);
                        fs.Read(buffer, 0, buffer.Length);
                    }

                    if (buffer[0] == vmCheckBytes[0] && buffer[1] == vmCheckBytes[1] && buffer[2] == vmCheckBytes[2])
                    {
                        // VM detected
                        return true;
                    }
                }
                catch { }
            }
            else
            {
                // x86 mode
                byte[] vmCheckBytes = { 0x0F, 0xA2 }; // cpuid
                byte[] buffer = new byte[2];

                try
                {
                    using (FileStream fs = new FileStream(@"C:\pagefile.sys", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        fs.Seek(0x7FFB, SeekOrigin.Begin);
                        fs.Read(buffer, 0, buffer.Length);
                    }

                    if (buffer[0] == vmCheckBytes[0] && buffer[1] == vmCheckBytes[1])
                    {
                        // VM detected
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }
    }
}
