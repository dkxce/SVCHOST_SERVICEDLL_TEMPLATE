//
// C# 
// svchost.exe Service DLL Example
// v 0.1, 18.05.2024
// https://github.com/dkxce
// en,ru,1251,utf-8
//

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Microsoft.Win32;

namespace CSharpSvcHostDLL
{
    /// <summary>
    ///     Supported `rundll32` methods:
    ///     - rundll32.exe {file.dll},Help;
    ///     - rundll32.exe {file.dll},Install;
    ///     - rundll32.exe {file.dll},Install [/s] [/log="{file}]"  -- with log file name;
    ///     - rundll32.exe {file.dll},Uninstall [/s];
    ///     - rundll32.exe {file.dll},Start [/s];
    ///     - rundll32.exe {file.dll},Stop [/s];
    /// </summary>
    public static class DLLMAIN
    {

        #pragma warning disable format // @formatter:off
        #region DLL CONSTS
        public const string SVC_SERVICE_NAME  /*ASCII*/ = "SVCHOST_SVC_DLL";      // It will bee %System32%\{SVC_SERVICE_NAME}.dll
        public const string SVC_DISPLAY_NAME  /*ASCII*/ = "Svchost Svc DLL (C#)"; // Service Manager Display Name
        public const string SVC_GROUP_NAME    /*ASCII*/ = "LocalServiceMinor";    // DcomLaunch ... HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Svchost\@{SVC_GROUP_NAME} ... svchost.exe -k {SVC_GROUP_NAME}
        public const string SVC_DESCRIPTION   /*ASCII*/ = "C# Service DLL running with svchost.exe. Sample by dkxce https://github.com/dkxce";
        // See: sc create help
        public const string SVC_SERVICE_TYPE  /*ASCII*/ = "share";                // Service Type: own|share|interact|kernel|filesys|rec|userown|usershare
        public const string SVC_SERVICE_START /*ASCII*/ = "auto";                 // Service Start: boot|system|auto|demand|disabled|delayed-auto
        public const string SVC_SERVICE_ERROR /*ASCII*/ = "normal";               // Service Error: normal|severe|critical|ignore        
        public const string SVC_SERVICE_USER  /*ASCII*/ = "LocalSystem";          // Service Credintals   
        public const bool   SVC_WRITE_LOG              = true;
        #endregion DLL CONSTS
        #pragma warning restore format // @formatter:on


        #region PUBLIC PARAMETERS
        public enum STATE : byte { STOPPED = 0, RUNNING = 1, PAUSED = 2 }
        public static STATE CurrentState { private set; get; } = STATE.STOPPED;
        public static Thread mainThread { private set; get; } = null;
        
        #endregion PUBLIC PARAMETERS

        #region MAIN METHODS

        static DLLMAIN() { }
       
        internal static bool OnStart()
        {
            CurrentState = STATE.RUNNING;
            mainThread = new Thread(MainServiceThread);
            mainThread.Start();
            return true;
        }  
        
        internal static bool OnStop()
        {
            CurrentState = STATE.STOPPED;
            return true;
        }

        internal static bool OnPause()
        {
            CurrentState = STATE.PAUSED;
            return true;
        }

        internal static bool OnContinue()
        {
            CurrentState = STATE.RUNNING;
            return true;
        }

        internal static bool OnInterrogate()
        {
            // DO SOMETHING //
            return true;
        }

        internal static void MainServiceThread()
        {
            while (CurrentState != STATE.STOPPED)
            {
                // Do something //
                while (CurrentState == STATE.PAUSED) Thread.Sleep(100);
                Thread.Sleep(100);
            };
        }

        #endregion MAIN METHODS       

        private static class DLLServiceBasics
        {
            #region WinApi Structs
            [Flags]
            private enum SERVICE_CONTROLS_ACCEPTED : uint
            {
                SERVICE_ACCEPT_STOP = 0x0001,
                SERVICE_ACCEPT_PAUSE_CONTINUE = 0x0002,
                SERVICE_ACCEPT_SHUTDOWN = 0x0004,
                SERVICE_ACCEPT_PARAMCHANGE = 0x0008,
                SERVICE_ACCEPT_NETBINDCHANGE = 0x0010,
                SERVICE_ACCEPT_HARDWAREPROFILECHANGE = 0x0020,
                SERVICE_ACCEPT_POWEREVENT = 0x0040,
                SERVICE_ACCEPT_SESSIONCHANGE = 0x0080,
                SERVICE_ACCEPT_PRESHUTDOWN = 0x0100,
                SERVICE_ACCEPT_TIMECHANGE = 0x0200,
                SERVICE_ACCEPT_TRIGGEREVENT = 0x0400,
                SERVICE_ACCEPT_USER_LOGOFF = 0x0800,
                // reserved for internal use           = 0x1000,
                SERVICE_ACCEPT_LOWRESOURCES = 0x2000,
                SERVICE_ACCEPT_SYSTEMLOWRESOURCES = 0x4000
            }

            private enum SERVICE_STATUS_CURRENT_STATE : uint
            {
                SERVICE_CONTINUE_PENDING = 5U,
                SERVICE_PAUSE_PENDING = 6U,
                SERVICE_PAUSED = 7U,
                SERVICE_RUNNING = 4U,
                SERVICE_START_PENDING = 2U,
                SERVICE_STOP_PENDING = 3U,
                SERVICE_STOPPED = 1U,
            }

            [Flags]
            private enum ENUM_SERVICE_TYPE : uint
            {
                SERVICE_DRIVER = 0x0000000B,
                SERVICE_KERNEL_DRIVER = 0x00000001,
                SERVICE_WIN32 = 0x00000030,
                SERVICE_WIN32_SHARE_PROCESS = 0x00000020,
                SERVICE_ADAPTER = 0x00000004,
                SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,
                SERVICE_RECOGNIZER_DRIVER = 0x00000008,
                SERVICE_WIN32_OWN_PROCESS = 0x00000010,
                SERVICE_USER_OWN_PROCESS = 0x00000050,
                SERVICE_USER_SHARE_PROCESS = 0x00000060
            }

            private struct SERVICE_STATUS
            {
                public ENUM_SERVICE_TYPE dwServiceType;
                public SERVICE_STATUS_CURRENT_STATE dwCurrentState;
                public uint dwControlsAccepted;
                public uint dwWin32ExitCode;
                public uint dwServiceSpecificExitCode;
                public uint dwCheckPoint;
                public uint dwWaitHint;
            }
            #endregion WinApi Structs

            #region Service Control Values
            private const uint SERVICE_CONTROL_STOP = 0x00000001;
            private const uint SERVICE_CONTROL_PAUSE = 0x00000002;
            private const uint SERVICE_CONTROL_CONTINUE = 0x00000003;
            private const uint SERVICE_CONTROL_INTERROGATE = 0x00000004;
            private const uint SERVICE_CONTROL_SHUTDOWN = 0x00000005;
            private const uint SERVICE_CONTROL_PARAMCHANGE = 0x00000006;
            private const uint SERVICE_CONTROL_NETBINDADD = 0x00000007;
            private const uint SERVICE_CONTROL_NETBINDREMOVE = 0x00000008;
            private const uint SERVICE_CONTROL_NETBINDENABLE = 0x00000009;
            private const uint SERVICE_CONTROL_NETBINDDISABLE = 0x0000000A;
            private const uint SERVICE_CONTROL_DEVICEEVENT = 0x0000000B;
            private const uint SERVICE_CONTROL_HARDWAREPROFILECHANGE = 0x0000000C;
            private const uint SERVICE_CONTROL_POWEREVENT = 0x0000000D;
            private const uint SERVICE_CONTROL_SESSIONCHANGE = 0x0000000E;
            private const uint SERVICE_CONTROL_PRESHUTDOWN = 0x0000000F;
            private const uint SERVICE_CONTROL_TIMECHANGE = 0x00000010;
            #endregion Service Control Values                

            #region WinAPI            

            [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern IntPtr RegisterServiceCtrlHandlerExW(string lpServiceName, IntPtr lpHandlerProc, IntPtr lpContext);

            [DllImport("advapi32.dll", SetLastError = true)]
            private static extern bool SetServiceStatus(IntPtr hServiceStatus, ref SERVICE_STATUS lpServiceStatus);
            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool QueryServiceStatus(IntPtr hService, ref SERVICE_STATUS dwServiceStatus);
            #endregion WinAPI

            #region Inits
            public static string LOG_FILE_PATH { private set; get; } = null;
            static DLLServiceBasics() { Init(); }
            private static void Init()
            {
                if (LOG_FILE_PATH == null)
                {
                    try
                    {
                        using (RegistryKey rk = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\{SVC_SERVICE_NAME}\\Parameters"))
                        {
                            string fn = rk.GetValue("LogPath", "").ToString();
                            if (!string.IsNullOrEmpty(fn)) LOG_FILE_PATH = fn;
                        };
                    }
                    catch { };
                };
            }
            #endregion Inits

            #region SVCHOST.EXE METHODS
            private delegate uint ServiceHandlerProc(uint dwControl, uint dwEventType, IntPtr lpEventData, IntPtr lpContext);
            private static IntPtr serviceStatusHandle = IntPtr.Zero;
            private static SERVICE_STATUS serviceStatus = new SERVICE_STATUS()
            {
                dwServiceType = ENUM_SERVICE_TYPE.SERVICE_WIN32_SHARE_PROCESS,
                dwCurrentState = SERVICE_STATUS_CURRENT_STATE.SERVICE_START_PENDING,
                dwControlsAccepted = (uint)(SERVICE_CONTROLS_ACCEPTED.SERVICE_ACCEPT_STOP | SERVICE_CONTROLS_ACCEPTED.SERVICE_ACCEPT_SHUTDOWN | SERVICE_CONTROLS_ACCEPTED.SERVICE_ACCEPT_PAUSE_CONTINUE)
            };            

            private static uint ServiceHandler(uint dwControl, uint dwEventType, IntPtr lpEventData, IntPtr lpContext)
            {
                switch (dwControl)
                {
                    case SERVICE_CONTROL_STOP:
                    case SERVICE_CONTROL_SHUTDOWN:
                        if (OnStop())
                        {
                            serviceStatus.dwCurrentState = SERVICE_STATUS_CURRENT_STATE.SERVICE_STOPPED;
                            WriteToLog("STOPPED");
                        };
                        break;
                    case SERVICE_CONTROL_PAUSE:
                        if (OnPause())
                        {
                            serviceStatus.dwCurrentState = SERVICE_STATUS_CURRENT_STATE.SERVICE_PAUSED;
                            WriteToLog("PAUSED");
                        };
                        break;
                    case SERVICE_CONTROL_CONTINUE:
                        if (OnContinue())
                        {
                            serviceStatus.dwCurrentState = SERVICE_STATUS_CURRENT_STATE.SERVICE_RUNNING;
                            WriteToLog("CONTINUE");
                        };
                        break;
                    case SERVICE_CONTROL_INTERROGATE:
                        if (OnInterrogate())
                        {
                            WriteToLog("INTERROGATE");
                        };
                        break;
                    default:
                        break;
                };

                SetServiceStatus(serviceStatusHandle, ref serviceStatus);
                return 0; // NO_ERROR;
            }

            [DllExport(CallingConvention = CallingConvention.StdCall, ExportName = SVC_SERVICE_NAME)]
            private static void Initialization() { }

            [DllExport(CallingConvention = CallingConvention.Cdecl, ExportName = "ServiceMain")]
            private static void ServiceMain(uint dwArgc, [MarshalAs(UnmanagedType.LPWStr)] string lpszArgv)
            {
                IntPtr ServiceHandlerPtr = Marshal.GetFunctionPointerForDelegate(new ServiceHandlerProc(ServiceHandler));
                serviceStatusHandle = RegisterServiceCtrlHandlerExW(SVC_SERVICE_NAME, ServiceHandlerPtr, IntPtr.Zero);
                if (serviceStatusHandle == IntPtr.Zero) return;

                if (OnStart())
                {
                    serviceStatus.dwCurrentState = SERVICE_STATUS_CURRENT_STATE.SERVICE_RUNNING;
                    SetServiceStatus(serviceStatusHandle, ref serviceStatus);
                    WriteToLog("STARTED");
                }
                else
                {
                    WriteToLog("START ABORTED");
                    serviceStatus.dwCurrentState = SERVICE_STATUS_CURRENT_STATE.SERVICE_STOPPED;
                    SetServiceStatus(serviceStatusHandle, ref serviceStatus);                    
                };
            }            
            #endregion SVCHOST.EXE METHODS

            #region RUNDLL32 METHODS

            /// <summary>
            ///     rundll32.exe {dllname},Help
            /// </summary>
            [DllExport(CallingConvention = CallingConvention.StdCall, ExportName = "Help")]
            public static uint GetHelp(IntPtr hwnd, IntPtr hinst, [MarshalAs(UnmanagedType.LPStr)] string pszCmdLine, int nCmdShow)
            {
                string path = Assembly.GetExecutingAssembly().Location;
                string file = Path.GetFileName(path);
                string dllp = Environment.ExpandEnvironmentVariables($"%SystemRoot%\\system32\\{SVC_SERVICE_NAME}.dll");
                string txt =
                    $"--------------------------------\r\n" +
                    $"Main Info:\r\n--------------------------------\r\n" +                    
                    $"Service name: {SVC_SERVICE_NAME}\r\n" +
                    $"Display name: {SVC_DISPLAY_NAME} \r\n" +
                    $"Description: {SVC_DESCRIPTION}\r\n" +
                    $"Service type: {SVC_SERVICE_TYPE} \r\n" +
                    $"Display start: {SVC_SERVICE_START} \r\n" +
                    $"Display error: {SVC_SERVICE_ERROR} \r\n" +
                    $"Display user: {SVC_SERVICE_USER} \r\n" +
                    $"Group name: {SVC_GROUP_NAME}\r\n" +                    
                    $"Write Log: {SVC_WRITE_LOG}\r\n" +
                    $"Current File: {file}\r\n" +
                    $"Current Path: {path}\r\n" +
                    $"Dll Path: {dllp}\r\n--------------------------------\r\n" +
                    $"Supported `rundll32` methods:\r\n--------------------------------\r\n" +
                    $"- rundll32.exe {file},Help \r\n" +
                    $"- rundll32.exe {file},Install /? \r\n" +
                    $"- rundll32.exe {file},Install [/s] [/log=\"{{file}}\" \r\n" +
                    $"- rundll32.exe {file},Uninstall [/s] \r\n" +
                    $"- rundll32.exe {file},Start [/s] \r\n" +
                    $"- rundll32.exe {file},Stop [/s] \r\n";
                RunProcStdOutForm f = new RunProcStdOutForm(SVC_DISPLAY_NAME, "Help");
                f.StatusVisible = false;
                f.SetText(txt);
                f.ShowDialog();
                f.Dispose();
                return 0;
            }

            /// <summary>
            ///     rundll32.exe {dllname},Install
            /// </summary>
            [DllExport(CallingConvention = CallingConvention.StdCall, ExportName = "Install")]
            public static uint InstallService(IntPtr hwnd, IntPtr hinst, [MarshalAs(UnmanagedType.LPStr)] string pszCmdLine, int nCmdShow)
            {
                bool silent = (!string.IsNullOrEmpty(pszCmdLine)) && pszCmdLine.ToLower().Contains("/s");
                string LogPath = null;
                if ((!string.IsNullOrEmpty(pszCmdLine)) && pszCmdLine.Contains("/log="))
                {
                    int iFrom = pszCmdLine.IndexOf("/log=\"") + 6;
                    int iTill = pszCmdLine.IndexOf("\"", iFrom);
                    LogPath = pszCmdLine.Substring(iFrom, iTill - iFrom);
                };

                if (!string.IsNullOrEmpty(pszCmdLine) && pszCmdLine.Contains("?"))
                {
                    string path = Assembly.GetExecutingAssembly().Location;
                    string file = Path.GetFileName(path);
                    string txt =
                        $"rundll32.exe {file},Install  -- Install\r\n" +
                        $"rundll32.exe {file},Install /s  -- Silent Install\r\n" +
                        $"rundll32.exe {file},Install /log=\"{{fileName}}\"  -- With Log Path: `{LogPath}`";
                    RunProcStdOutForm f = new RunProcStdOutForm(SVC_DISPLAY_NAME, "Install Help");
                    f.StatusVisible = false;
                    f.SetText(txt);
                    f.ShowDialog();
                    f.Dispose();
                    return 0;
                };

                string result = "";
                if (silent)
                {
                    {   // COPY FILE TO System32 Folder
                        string path = Assembly.GetExecutingAssembly().Location;
                        string cmd = $"copy /Y \"{path}\" \"%SystemRoot%\\system32\\{SVC_SERVICE_NAME}.dll\"";
                        result += "Copy ServiceDll: " + RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' }) + "\r\n";
                    };
                    {   // Set DLL Path
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\\Parameters\" /v \"ServiceDll\" /t REG_EXPAND_SZ /d \"%SystemRoot%\\system32\\{SVC_SERVICE_NAME}.dll\" /f";
                        result += "Write ServiceDll: " + RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' }) + "\r\n";
                    };
                    {   // Set Display Name
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\" /v \"DisplayName\" /t REG_SZ /d \"{SVC_DISPLAY_NAME}\" /f";
                        result += "Write Display Name: " + RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' }) + "\r\n";
                    };
                    {   // Set Description
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\" /v \"Description\" /t REG_SZ /d \"{SVC_DESCRIPTION}\" /f";
                        result += "Write Description: " + RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' }) + "\r\n";
                    };
                    {   // Set Service Group
                        string cmd = $"reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\SvcHost\" /v \"{SVC_GROUP_NAME}\" /t REG_MULTI_SZ /d \"{SVC_SERVICE_NAME}\" /f";
                        result += "Write Service Group: " + RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' }) + "\r\n";
                    };
                    if (!string.IsNullOrEmpty(LogPath))
                    {   // Set Log Path
                        result += "Set LogPath: " + LogPath + "\r\n";
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\\Parameters\" /v \"LogPath\" /t REG_SZ /d \"{LogPath}\" /f";
                        result += "Write LogPath: " + RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' }) + "\r\n";
                    };
                    {   // Set FailureActions
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\" /v \"FailureActions\" /t REG_BINARY /d 00000000000000000000000003000000140000000100000060ea00000100000060ea00000100000060ea0000 /f";
                        result += "Set FailureActions: " + RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' }) + "\r\n";
                    };
                    {   // Install Service
                        string cmd = $"sc create {SVC_SERVICE_NAME} binPath=\"%SystemRoot%\\System32\\svchost.exe -k {SVC_GROUP_NAME}\" type={SVC_SERVICE_TYPE} start={SVC_SERVICE_START} error={SVC_SERVICE_ERROR} obj={SVC_SERVICE_USER} displayname=\"{SVC_DISPLAY_NAME}\"";
                        result += "Install Service: " + RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' }) + "\r\n";
                    };
                }
                else
                {
                    RunProcStdOutForm f = new RunProcStdOutForm(SVC_DISPLAY_NAME, $"Install Service `{SVC_SERVICE_NAME}`");
                    {   // COPY FILE TO System32 Folder
                        string path = Assembly.GetExecutingAssembly().Location;
                        string cmd = $"copy /Y \"{path}\" \"%SystemRoot%\\system32\\{SVC_SERVICE_NAME}.dll\"";
                        f.WriteLine("######## COPY DLL FILE TO System32 Folder ########");
                        result += f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                        f.WriteLine();
                    };
                    {   // Set DLL Path
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\\Parameters\" /v \"ServiceDll\" /t REG_EXPAND_SZ /d \"%SystemRoot%\\system32\\{SVC_SERVICE_NAME}.dll\" /f";
                        f.WriteLine("########v SET DLL PATH ########");
                        result += f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                        f.WriteLine();
                    };
                    {   // Set Display Name
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\" /v \"DisplayName\" /t REG_SZ /d \"{SVC_DISPLAY_NAME}\" /f";
                        f.WriteLine("######## SET DISPLAY NAME ########");
                        result += f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                        f.WriteLine();
                    };
                    {   // Set Description
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\" /v \"Description\" /t REG_SZ /d \"{SVC_DESCRIPTION}\" /f";
                        f.WriteLine("######## SET DESCRIPTION ########");
                        result += f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                        f.WriteLine();
                    };
                    {   // Set Service Group
                        string cmd = $"reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\SvcHost\" /v \"{SVC_GROUP_NAME}\" /t REG_MULTI_SZ /d \"{SVC_SERVICE_NAME}\" /f";
                        f.WriteLine("######## SET SERVICE GROUP ########");
                        result += f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                        f.WriteLine();
                    };
                    if (!string.IsNullOrEmpty(LogPath))
                    {   // Set Log Path
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\\Parameters\" /v \"LogPath\" /t REG_SZ /d \"{LogPath}\" /f";
                        f.WriteLine("######## SET LOG PATH ########");
                        result += f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                        f.WriteLine();
                    };
                    {   // Set FailureActions
                        string cmd = $"reg add \"HKLM\\SYSTEM\\CurrentControlSet\\services\\{SVC_SERVICE_NAME}\" /v \"FailureActions\" /t REG_BINARY /d 00000000000000000000000003000000140000000100000060ea00000100000060ea00000100000060ea0000 /f";
                        f.WriteLine("######## SET FAILURE ACTIONS ########");
                        result += f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                        f.WriteLine();
                    };
                    {   // Install Service
                        string cmd = $"sc create {SVC_SERVICE_NAME} binPath=\"%SystemRoot%\\System32\\svchost.exe -k {SVC_GROUP_NAME}\" type={SVC_SERVICE_TYPE} start={SVC_SERVICE_START} error={SVC_SERVICE_ERROR} obj={SVC_SERVICE_USER} displayname=\"{SVC_DISPLAY_NAME}\"";
                        f.WriteLine("######## INSTALL SERVICE ########");
                        result += f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                        f.WriteLine();
                    };
                    f.WriteLine("######## OPERATIONS COMPLETED ########");
                    f.WaitWhileShowed();
                    f.Dispose();
                };

                return string.IsNullOrEmpty(result) ? (uint)1 : 0;
            }

            /// <summary>
            ///     rundll32.exe {dllname},Uninstall
            /// </summary>
            [DllExport(CallingConvention = CallingConvention.StdCall, ExportName = "Uninstall")]
            public static uint UninstallService(IntPtr hwnd, IntPtr hinst, [MarshalAs(UnmanagedType.LPStr)] string pszCmdLine, int nCmdShow)
            {
                string cmd = $"sc delete {SVC_SERVICE_NAME}";
                bool silent = (!string.IsNullOrEmpty(pszCmdLine)) && pszCmdLine.ToLower().Contains("/s");
                string result;
                if (silent)
                {
                    result = RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' });
                }
                else
                {
                    RunProcStdOutForm f = new RunProcStdOutForm(SVC_DISPLAY_NAME, $"Uninstall Service `{SVC_SERVICE_NAME}`");
                    f.WriteLine("######## UNINSTALL SERVICE ########");
                    result = f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                    f.WriteLine("######## OPERATIONS COMPLETED ########");
                    f.WaitWhileShowed();
                    f.Dispose();
                };               
                return string.IsNullOrEmpty(result) ? (uint)1 : 0; 
            }

            /// <summary>
            ///     rundll32.exe {dllname},Start
            /// </summary>
            [DllExport(CallingConvention = CallingConvention.StdCall, ExportName = "Start")]
            public static uint StartService(IntPtr hwnd, IntPtr hinst, [MarshalAs(UnmanagedType.LPStr)] string pszCmdLine, int nCmdShow)
            {
                string cmd = $"net start {SVC_SERVICE_NAME}";
                bool silent = (!string.IsNullOrEmpty(pszCmdLine)) && pszCmdLine.ToLower().Contains("/s");
                string result;
                if (silent)
                {
                    result = RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' });
                }
                else
                {
                    RunProcStdOutForm f = new RunProcStdOutForm(SVC_DISPLAY_NAME, $"Start Service `{SVC_SERVICE_NAME}`");
                    f.WriteLine("######## START SERVICE ########");
                    result = f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                    f.WriteLine("######## OPERATIONS COMPLETED ########");
                    f.WaitWhileShowed();
                    f.Dispose();
                };
                return string.IsNullOrEmpty(result) ? (uint)1 : 0;
            }

            /// <summary>
            ///     rundll32.exe {dllname},Stop
            /// </summary>
            [DllExport(CallingConvention = CallingConvention.StdCall, ExportName = "Stop")]
            public static uint StopService(IntPtr hwnd, IntPtr hinst, [MarshalAs(UnmanagedType.LPStr)] string pszCmdLine, int nCmdShow)
            {
                string cmd = $"net stop {SVC_SERVICE_NAME}";
                bool silent = (!string.IsNullOrEmpty(pszCmdLine)) && pszCmdLine.ToLower().Contains("/s");
                string result;
                if (silent)
                {
                    result = RunCmd($"/C {cmd}").TrimEnd(new char[] { '\r', '\n' });
                }
                else
                {
                    RunProcStdOutForm f = new RunProcStdOutForm(SVC_DISPLAY_NAME, $"Stop Service `{SVC_SERVICE_NAME}`");
                    f.WriteLine("######## STOP SERVICE ########");
                    result = f.StartProcAndShow(new ProcessStartInfo("cmd.exe", $"/C {cmd}") { StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage) });
                    f.WriteLine("######## OPERATIONS COMPLETED ########");
                    f.WaitWhileShowed();
                    f.Dispose();
                };
                return string.IsNullOrEmpty(result) ? (uint)1 : 0;
            }            
            
            #endregion RUNDLL32 METHODS

            #region PRIVATES
            private static string RunCmd(string cmd)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.Arguments = cmd;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
                proc.Start();
                proc.WaitForExit();
                return proc.StandardOutput.ReadToEnd();
            }
            #endregion PRIVATES

            #region INTERNAL
            internal static void WriteToLog(string text)
            {
                if (!SVC_WRITE_LOG || String.IsNullOrEmpty(LOG_FILE_PATH)) return;
                FileStream fs = null;
                try
                {
                    string fpath = Environment.ExpandEnvironmentVariables(LOG_FILE_PATH);
                    fs = new FileStream(fpath, FileMode.Append, FileAccess.Write, FileShare.Read);
                    string dt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    byte[] data = Encoding.GetEncoding(1251).GetBytes($"{dt}: {text}\r\n");
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                }
                catch { if (fs != null) fs.Dispose(); };
            }
            #endregion INTERNAL
        }
    }
}
