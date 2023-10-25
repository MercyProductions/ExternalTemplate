using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class AntiDebug
{
    // Check if a debugger is attached using P/Invoke
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

    // Check for a debugger using P/Invoke
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    private delegate bool IsDebuggerPresentDelegate();

    public static bool IsDebuggerAttached()
    {
        bool isDebuggerPresent = false;
        CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);

        return isDebuggerPresent;
    }

    // Check for a debugger using P/Invoke and GetModuleHandle/GetProcAddress
    public static bool IsDebuggerPresent()
    {
        IntPtr kernel32Module = GetModuleHandle("kernel32.dll");
        IntPtr isDebuggerPresentPtr = GetProcAddress(kernel32Module, "IsDebuggerPresent");

        if (isDebuggerPresentPtr != IntPtr.Zero)
        {
            IsDebuggerPresentDelegate isDebuggerPresent = (IsDebuggerPresentDelegate)Marshal.GetDelegateForFunctionPointer(isDebuggerPresentPtr, typeof(IsDebuggerPresentDelegate));
            return isDebuggerPresent();
        }

        return false;
    }

    // Check for a debugger using Debugger.IsAttached property
    public static bool IsDebuggerAttachedManaged()
    {
        return Debugger.IsAttached;
    }

    // Check if the process is running under a debugger by trying to attach a new debugger
    public static bool IsDebuggerPresentManaged()
    {
        try
        {
            Debugger.Launch();
        }
        catch (NotSupportedException)
        {
            return true;
        }

        return false;
    }

    private static readonly string[] SuspiciousProcessNames = {
        "cheatengine",
        "engine",
        "ollydbg",
        "idaq",
        "idaq64",
        "idag",
        "idag64",
        "idaw",
        "idaw64",
        "ida",
        "ida64",
        "dnspy",
        "x64dbg",
        "x32dbg",
        "windbg",
        "scylla",
        "charles",
        "fiddler",
        // Add other process names here as needed
    };

    public static bool IsSuspiciousProcessRunning()
    {
        Process[] processes = Process.GetProcesses();
        foreach (Process process in processes)
        {
            if (Array.Exists(SuspiciousProcessNames, name => process.ProcessName.ToLower().Contains(name)))
            {
                return true;
            }
        }

        return false;
    }

    // Combine all checks to determine if debugging or suspicious processes are present
    public static bool IsDebuggingOrSuspiciousProcessPresent()
    {
        return IsDebuggerAttached() || IsDebuggerPresent() || IsDebuggerAttachedManaged()
            || IsDebuggerPresentManaged() || IsSuspiciousProcessRunning();
    }

    private static void OnDebuggingDetected(object sender, EventArgs e)
    {
        // Respond to debugger detection, e.g., exit the application or display a warning message.
        Console.WriteLine("Debugger detected! Exiting application.");
        Environment.Exit(0);
    }

    // Method to initialize the debugger detection event
    public static void InitializeDebuggerDetection()
    {
        AppDomain currentDomain = AppDomain.CurrentDomain;
        currentDomain.ProcessExit += OnDebuggingDetected;
        currentDomain.DomainUnload += OnDebuggingDetected;
    }

    // Combine all checks to determine if debugging or suspicious processes are present
}
