// Licensed to the.NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        /// The zero-based offset to the value to be retrieved. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of a LONG_PTR.  used by <see cref="GetWindowLong"/> and <see cref="SetWindowLong"/>
        /// </summary>
        public enum GWLIndex
        {
            /// <summary>
            /// Retrieves the  <see cref="WindowStyles"/>.
            /// </summary>
            GWL_STYLE = -16,
            /// <summary>
            /// Retrieves the extended window styles.
            /// </summary>
            GWL_EXSTYLE = -20,
            /// <summary>
            /// Retrieves a handle to the application instance.
            /// </summary>
            GWL_HINSTANCE = -6,
            /// <summary>
            /// Retrieves a handle to the parent window, if any.
            /// </summary>
            GWL_HWNDPARENT = -8,
            /// <summary>
            /// Retrieves the identifier of the window.
            /// </summary>
            GWL_ID = -12,
            /// <summary>
            /// Retrieves the user data associated with the window. This data is intended for use by the application that created the window. Its value is initially zero.
            /// </summary>
            GWL_USERDATA = -21,
            /// <summary>
            /// Retrieves the address of the window procedure, or a handle representing the address of the window procedure. You must use the CallWindowProc function to call the window procedure.
            /// </summary>
            GWL_WNDPROC = -4
        }
    }
}
