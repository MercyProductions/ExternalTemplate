using CSharp_Memory_Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MercyGlobal.Global
{
    public class Threads
    {
        public static void AttachToGame()
        {
            while (MercyGlobal.Global.Bools.isRunning == true)
            {
                Process proc = Process.GetProcessesByName("game")[0];
                var hProc = Memory.OpenProcess(proc, (uint)Memory.ProcessAccessFlags.All);

                var modBase = Memory.GetModuleBaseAddress(proc, "game");
                var ModBase2 = Memory.GetModuleBaseAddress(proc.Id, "game");

                Console.WriteLine("Game is running...");

                Thread.Sleep(1000);
            }

            while (MercyGlobal.Global.Bools.isRunning == false)
            {
                Process proc = Process.GetProcessesByName("game")[0];
                var hProc = Memory.OpenProcess(proc, (uint)Memory.ProcessAccessFlags.All);

                Console.WriteLine("Game is Closed...");

                Thread.Sleep(1000);
            }
        }
    }
}
