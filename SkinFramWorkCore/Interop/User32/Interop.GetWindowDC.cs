// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System;
internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static extern Gdi32.HDC GetWindowDC(IntPtr hWnd);

        public static Gdi32.HDC GetWindowDC(IHandle hWnd)
        {
            Gdi32.HDC result = GetWindowDC(hWnd.Handle);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
