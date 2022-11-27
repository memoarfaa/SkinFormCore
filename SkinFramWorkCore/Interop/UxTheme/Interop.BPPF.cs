internal static partial class Interop
{
    public static partial class UxTheme
    {
        /// <summary>
        /// value used in <see cref="BP_PAINTPARAMS.Flags"/>  struct
        /// </summary>
        public enum BPPF : uint
        {
            /// <summary>
            /// Initialize the buffer to ARGB = {0, 0, 0, 0} during B<see cref="eginBufferedPaint."/> This erases the previous contents of the buffer.
            /// </summary>
            Erase = 1,
            /// <summary>
            /// Do not apply the clip region of the target DC to the double buffer. If this flag is not set and if the target DC is a window DC, then clipping due to overlapping windows is applied to the double buffer.
            /// </summary>
            NoClip = 2,
            /// <summary>
            /// A non-client DC is being used.
            /// </summary>
            NonClient = 4
        }
    }
}
