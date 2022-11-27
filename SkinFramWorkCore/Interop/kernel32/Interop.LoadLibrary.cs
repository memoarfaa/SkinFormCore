using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


internal partial class Interop
{
    internal partial class Kernel32
    {
        
        private const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr LoadLibraryExW(string lpModuleName, IntPtr hFile, uint dwFlags);

        public static IntPtr LoadLibraryAsDataFile(string libraryName)
        {
            IntPtr kernel32 = GetModuleHandleW(Libraries.Kernel32);
            if (kernel32 == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            // LOAD_LIBRARY_SEARCH_SYSTEM32 was introduced in KB2533623. Check for its presence
            // to preserve compat with Windows 7 SP1 without this patch.
            IntPtr result = LoadLibraryExW(libraryName, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
            if (result != IntPtr.Zero)
            {
                return result;
            }

            // Load without this flag.
            if (Marshal.GetLastWin32Error() != ERROR.INVALID_PARAMETER)
            {
                return IntPtr.Zero;
            }

            return LoadLibraryExW(libraryName, IntPtr.Zero, 0);
        }
    }
}
