using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharp_Memory_Class;
using MercyGlobal;

namespace CSharp_Memory_Class
{
    class Program
    {
        static void Main(string[] args)
        {
            CSharp_Memory_Class.Memory m = new CSharp_Memory_Class.Memory();

            Thread thread = new Thread(MercyGlobal.Global.Threads.AttachToGame);
            thread.Start();

            // Code Goes Here

            System.Console.WriteLine("Last Error: " + Marshal.GetLastWin32Error());
        }
    }
}
