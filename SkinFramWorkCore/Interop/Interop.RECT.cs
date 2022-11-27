// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    /// <summary>
    /// The RECT structure defines a rectangle by the coordinates of its upper-left and lower-right corners.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>
        /// Specifies the x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int left;
        /// <summary>
        /// Specifies the y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int top;
        /// <summary>
        /// Specifies the x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int right;
        /// <summary>
        /// Specifies the y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int bottom;
        /// <summary>
        /// Initializes a new instance of the RECT class with the specified location and size
        /// </summary>
        /// <param name="left">Specifies the x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">Specifies the y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">Specifies the x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">Specifies the y-coordinate of the lower-right corner of the rectangle.</param>
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
        /// <summary>
        /// Initializes a new instance of the RECT class from specified <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rectangle">Rectangle that will convert to RECT</param>
        public RECT(Rectangle rectangle)
        {
            left = rectangle.Left;
            top = rectangle.Top;
            right = rectangle.Right;
            bottom = rectangle.Bottom;
        }
        /// <summary>
        /// Initializes a new instance of the Rectangle class from specified <see cref="RECT"/>
        /// </summary>
        /// <param name="rectangle">Rectangle that will convert to RECT</param>
        public static implicit operator Rectangle(RECT r)
            => Rectangle.FromLTRB(r.left, r.top, r.right, r.bottom);
        /// <summary>
        /// Initializes a new instance of the RECT class from specified <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rectangle">Rectangle that will convert to RECT</param>
        public static implicit operator RECT(Rectangle r)
            => new RECT(r);
        public RECT RtlRect(int width)
        {
            return new RECT(width - Width - left, top, width - left, bottom);
        }
        /// <summary>
        /// Specifies the x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int X => left;
        /// <summary>
        /// Specifies the y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int Y => top;
        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public int Width
            => right - left;
        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        public int Height
            => bottom - top;
        /// <summary>
        /// The <see cref="Size"/> of the rectangle.
        /// </summary>
        public Size Size
            => new Size(Width, Height);

        public override string ToString()
            => $"{{{left}, {top}, {right}, {bottom} (LTRB)}}";
    }
   
}
