// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    public static partial class UxTheme
    {
        public enum BP_BUFFERFORMAT
        {
            /// <summary>
            /// Compatible bitmap. The number of bits per pixel is based on the color format of the device associated with the HDC specified with BeginBufferedPaint or BeginBufferedAnimation—typically, this is the display device.
            /// </summary>
            CompatibleBitmap,
            /// <summary>
            /// Bottom-up device-independent bitmap. The origin of the bitmap is the lower-left corner. Uses 32 bits per pixel.
            /// </summary>
            DIB,
            /// <summary>
            /// Top-down device-independent bitmap. The origin of the bitmap is the upper-left corner. Uses 32 bits per pixel.
            /// </summary>
            TopDownDIB,
            /// <summary>
            /// Top-down, monochrome, device-independent bitmap. Uses 1 bit per pixel.
            /// </summary>
            TopDownMonoDIB
        }
    }
}
