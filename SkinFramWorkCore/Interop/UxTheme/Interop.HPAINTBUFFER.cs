// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#pragma warning disable CS8765
using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
internal static partial class Interop
{
    public static partial class UxTheme
    {
        /// <summary>Provides a handle to a paint buffer.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HPAINTBUFFER : IHandle, IEquatable<HPAINTBUFFER>
        {
            public IntPtr Handle { get; }
            public HPAINTBUFFER(IntPtr preHandle) => Handle = preHandle;
            public bool IsNull => Handle == IntPtr.Zero;
            public static explicit operator IntPtr(HPAINTBUFFER h) => h.Handle;
            public static implicit operator HPAINTBUFFER(IntPtr h) => new HPAINTBUFFER(h);
            public static bool operator !=(HPAINTBUFFER h1, HPAINTBUFFER h2) => !(h1 == h2);
            public static bool operator ==(HPAINTBUFFER h1, HPAINTBUFFER h2) => h1.Equals(h2);
            public bool Equals(HPAINTBUFFER other) => Handle == other.Handle;
            public override bool Equals(object obj) => obj is HPAINTBUFFER h && Equals(h);
            public override int GetHashCode() => Handle.GetHashCode();
        }
    }
}
