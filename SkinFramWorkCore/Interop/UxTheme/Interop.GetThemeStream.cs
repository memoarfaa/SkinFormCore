﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme)]
        public static extern int GetThemeStream(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out IntPtr ppvStream, out uint pcbStream, IntPtr hInst);
    }
}
