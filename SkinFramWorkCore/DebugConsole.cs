using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
namespace SkinFramWorkCore
{
    internal class DebugConsole
    {
        #region Methods
        /// <summary>
        /// Show Debug messages in Console Window
        /// </summary>
        public static void Show()
        {
            var consoleAttached = true;
            if (AttachConsole(ATTACH_PARRENT) == 0
                && Marshal.GetLastWin32Error() != ERROR_ACCESS_DENIED)
            {
                consoleAttached = Debugger.IsAttached || AllocConsole() != 0;

            }
            Trace.Listeners.Add(new ConsoleTraceListener { TraceOutputOptions = TraceOptions.Timestamp | TraceOptions.Callstack });
        }

        public static void ShowConsole(IList<string> cmdArgs)
        {
            if (cmdArgs.Count > 0 && cmdArgs[0].ToLower().Equals("/ShowDebug".ToLower()))
            {
                Show();
            }
        }

        public static void WriteLine(object ex)
        {
            // System.Threading.Thread.Sleep(200);
#if DEBUG
            Debug.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            Debug.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss.fff tt", CultureInfo.CurrentCulture));
            if (ex is Exception | ex.GetType().BaseType == typeof(Exception))
            {
                Debug.WriteLine($"Exception Message:{((Exception)ex).Message}");
                if (((Exception)ex).InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception:{((Exception)ex).InnerException}");
                }
                Debug.WriteLine($"StackTrace:\r\n{((Exception)ex).StackTrace}");
            }
            else
            {
                Debug.WriteLine(ex);
            }

            Debug.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            Debug.WriteLine("\r\n");
#else
            Trace.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            Trace.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss.fff tt", CultureInfo.CurrentCulture));
          if (ex is Exception | ex.GetType().BaseType == typeof(Exception))
            {
                Trace.WriteLine($"Exception Message:{((Exception)ex).Message}");
                if (((Exception)ex).InnerException != null)
                {
                   Trace.WriteLine($"Inner Exception:{((Exception)ex).InnerException}");
                }
                Trace.WriteLine($"StackTrace:\r\n{((Exception)ex).StackTrace}");
                Trace.WriteLine("\r\n");
            }
            else
            {
                Debug.WriteLine(ex);
            }
            Trace.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            Trace.WriteLine("\r\n");
#endif
        }

        public static void WriteLine(object ex, int millisecondsTimeout)
        {
            System.Threading.Thread.Sleep(millisecondsTimeout);
#if DEBUG
            Debug.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            Debug.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss.fff tt", CultureInfo.CurrentCulture));
            if (ex is Exception | ex.GetType().BaseType == typeof(Exception))
            {
                Debug.WriteLine($"Exception Message:{((Exception)ex).Message}");
                if (((Exception)ex).InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception:{((Exception)ex).InnerException}");
                }
                Debug.WriteLine($"StackTrace:\r\n{((Exception)ex).StackTrace}");
            }
            else
            {
                Debug.WriteLine(ex);
            }

            Debug.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            Debug.WriteLine("\r\n");
#else
            Trace.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            Trace.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss.fff tt", CultureInfo.CurrentCulture));
          if (ex is Exception | ex.GetType().BaseType == typeof(Exception))
            {
                Trace.WriteLine($"Exception Message:{((Exception)ex).Message}");
                if (((Exception)ex).InnerException != null)
                {
                   Trace.WriteLine($"Inner Exception:{((Exception)ex).InnerException}");
                }
                Trace.WriteLine($"StackTrace:\r\n{((Exception)ex).StackTrace}");
                Trace.WriteLine("\r\n");
            }
            else
            {
                Debug.WriteLine(ex);
            }
            Trace.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            Trace.WriteLine("\r\n");
#endif
        }

        public static void Clear()
        {
            try
            {
                Console.Clear();
            }
            catch
            {
                // ignored
            }
        }
        #endregion

        #region Win API Functions and Constants
        /// <summary>
        /// Allocates a new console for the calling process.
        /// </summary>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();
        /// <summary>
        /// Attaches the calling process to the console of the specified process.
        /// </summary>
        /// <param name="dwProcessId">The identifier of the process whose console is to be used. This parameter can be one of the following values.
        /// A- pid :Use the console of the specified process. B- ATTACH_PARENT_PROCESS :Use the console of the parent of the current process.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>

        [DllImport("kernel32.dll", EntryPoint = "AttachConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern uint AttachConsole(uint dwProcessId);
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// System Error Code The system cannot open the file Access is denied.
        /// </summary>
        private const uint ERROR_ACCESS_DENIED = 5;
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Use the console of the parent of the current process.
        /// </summary>
        private const uint ATTACH_PARRENT = 0xFFFFFFFF;

        #endregion
    }
}
