// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum GCL : int
        {
            /// <summary>
            ///  Retrieves the address of the window procedure, or a handle representing the address of the window procedure. You must use the CallWindowProc function to call the window procedure.
            /// </summary>
            GCLP_WNDPROC = -24,

            /// <summary>
            ///  Retrieves an ATOM value that uniquely identifies the window class. This is the same atom that the RegisterClassEx function returns.
            /// </summary>
            GCW_ATOM = -32,
            /// <summary>
            /// Retrieves a handle to the background brush associated with the class.
            /// </summary>
            GCLP_HBRBACKGROUND=-10

        }
    }
}
