// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
internal static partial class Interop
{
    public static partial class UxTheme
    {
        /// <summary>
        /// Begins a buffered paint operation.
        /// </summary>
        /// <param name="hdc">The handle of the target DC on which the buffer will be painted.</param>
        /// <param name="prcTarget">A pointer to a RECT structure that specifies the area of the target DC in which to paint.</param>
        /// <param name="dwFormat">A member of the <see cref="BP_BUFFERFORMAT"/> enumeration that specifies the format of the buffer.</param>
        /// <param name="pPaintParams">A pointer to a <see cref="BP_PAINTPARAMS"/> structure that defines the paint operation parameters. This value can be NULL.</param>
        /// <param name="phdc">When this function returns, points to the handle of the new device context.</param>
        /// <returns> Type: <see cref="HPAINTBUFFER"/>
        /// A handle to the buffered paint context. If this function fails, the return value is NULL, and phdc is NULL. To get extended error information, call GetLastError.</returns>
        [DllImport(Libraries.UxTheme, SetLastError = true)]

        public static extern HPAINTBUFFER BeginBufferedPaint(Gdi32.HDC hdc, ref RECT prcTarget, BP_BUFFERFORMAT dwFormat, ref BP_PAINTPARAMS pPaintParams, out Gdi32.HDC phdc);
    }
}
