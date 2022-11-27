// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
internal static partial class Interop
{
    internal static partial class Dwmapi
    {
        [DllImport(Libraries.Dwmapi, CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern HRESULT DwmExtendFrameIntoClientArea(IntPtr hWnd, ref UxTheme.MARGINS pMarInset);
        public static HRESULT DwmExtendFrameIntoClientArea(IHandle hWnd, ref UxTheme.MARGINS pMarInset)
        {
            var result = DwmExtendFrameIntoClientArea(hWnd.Handle, ref pMarInset);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
