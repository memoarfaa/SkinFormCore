// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Drawing;
using System;

internal static partial class Interop
{
    public static partial class UxTheme
    {


        /// <summary>
        /// Defines paint operation parameters for <see cref=" BeginBufferedPaint"/>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct BP_PAINTPARAMS : IDisposable
        {
            /// <summary>
            /// The size, in bytes, of this structure
            /// </summary>
            private readonly int cbSize;
            /// <summary>
            /// <see cref="BPPF"/>
            /// </summary>
            private readonly BPPF Flags;
            /// <summary>
            /// A pointer to exclusion RECT structure. This rectangle is excluded from the clipping region. May be NULL for no exclusion rectangle.
            /// </summary>
            private IntPtr prcExclude;
            /// <summary>
            /// A pointer to BLENDFUNCTION structure, which controls blending by specifying the blending functions for source and destination bitmaps. If NULL, the source buffer is copied to the destination with no blending.
            /// </summary>
            private IntPtr pBlendFunction;

            public BP_PAINTPARAMS(BPPF flags)
            {
                cbSize = Marshal.SizeOf(typeof(BP_PAINTPARAMS));
                Flags = flags;
                prcExclude = pBlendFunction = IntPtr.Zero;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct BLENDFUNCTION
            {
                public byte BlendOp;
                public byte BlendFlags;
                public byte SourceConstantAlpha;
                public byte AlphaFormat;

                public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
                {
                    BlendOp = op;
                    BlendFlags = flags;
                    SourceConstantAlpha = alpha;
                    AlphaFormat = format;
                }
            }

            public Rectangle Exclude
            {
                get
                {
                    return Marshal.PtrToStructure<Rectangle>(prcExclude);
                }
                set
                {
                    if (prcExclude == IntPtr.Zero)
                        prcExclude = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RECT)));
                    Marshal.StructureToPtr(value, prcExclude, false);
                }
            }

            public BLENDFUNCTION BlendFunction
            {
                get { return Marshal.PtrToStructure<BLENDFUNCTION>(pBlendFunction); }
                set
                {
                    if (pBlendFunction == IntPtr.Zero)
                        pBlendFunction = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BLENDFUNCTION)));
                    Marshal.StructureToPtr(value, pBlendFunction, false);
                }
            }

            public void Dispose()
            {
                if (prcExclude != IntPtr.Zero)
                    Marshal.FreeHGlobal(prcExclude);
                if (pBlendFunction != IntPtr.Zero)
                    Marshal.FreeHGlobal(pBlendFunction);
            }
        }
    }
}
