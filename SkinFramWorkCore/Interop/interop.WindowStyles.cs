using System;
internal static partial class Interop
{
        [Flags]
        public enum WindowStyles : uint
        {
            /// <summary>
            /// The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_TILED style.
            /// </summary>
            WS_OVERLAPPED = 0x00000000,
            /// <summary>
            /// The window is a pop-up window. This style cannot be used with the WS_CHILD style.
            /// </summary>
            WS_POPUP = 0x80000000,
            /// <summary>
            /// The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.
            /// </summary>
            WS_CHILD = 0x40000000,
            /// <summary>
            /// The window is initially minimized. Same as the WS_ICONIC style.
            /// </summary>
            WS_MINIMIZE = 0x20000000,
            /// <summary>
            /// The window is initially visible.
            /// This style can be turned on and off by using the ShowWindow or SetWindowPos function.
            /// </summary>
            WS_VISIBLE = 0x10000000,
            /// <summary>
            /// The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.
            /// </summary>
            WS_DISABLED = 0x08000000,
            /// <summary>
            /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated. If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
            /// </summary>
            WS_CLIPSIBLINGS = 0x04000000,
            /// <summary>
            /// Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.
            /// </summary>
            WS_CLIPCHILDREN = 0x02000000,
            /// <summary>
            /// The window is initially maximized.
            /// </summary>
            WS_MAXIMIZE = 0x01000000,
            /// <summary>
            /// The window has a thin-line border
            /// </summary>
            WS_BORDER = 0x00800000,
            /// <summary>
            /// The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.
            /// </summary>
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            /// <summary>
            /// The window has a horizontal scroll bar.
            /// </summary>
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            /// <summary>
            /// The window has a sizing border. Same as the WS_SIZEBOX style.
            /// </summary>
            WS_THICKFRAME = 0x00040000,
            /// <summary>
            /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style. The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
            /// </summary>
            WS_GROUP = 0x00020000,
            /// <summary>
            /// The window is a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.
            /// </summary>
            WS_TABSTOP = 0x00010000,
            /// <summary>
            /// The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
            /// </summary>

            WS_MINIMIZEBOX = 0x00020000,
            /// <summary>
            /// The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
            /// </summary>
            WS_MAXIMIZEBOX = 0x00010000,
            /// <summary>
            /// The window has a title bar (includes the WS_BORDER style).
            /// </summary>
            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            /// <summary>
            /// The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_OVERLAPPED style.
            /// </summary>
            WS_TILED = WS_OVERLAPPED,
            /// <summary>
            /// The window is initially minimized. Same as the WS_MINIMIZE style.
            /// </summary>
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            /// <summary>
            /// The window is an overlapped window. Same as the WS_OVERLAPPEDWINDOW style.
            /// </summary>
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
            /// <summary>
            /// The window is an overlapped window. Same as the WS_TILEDWINDOW style.
            /// </summary>
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            /// <summary>
            /// The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.
            /// </summary>
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            /// <summary>
            /// Same as the WS_CHILD style.
            /// </summary>
            WS_CHILDWINDOW = WS_CHILD,

            //Extended Window Styles

            /// <summary>
            /// The window has a double border; the window can, optionally, be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.
            /// </summary>
            WS_EX_DLGMODALFRAME = 0x00000001,
            /// <summary>
            /// The window does not pass its window layout to its child windows.
            /// </summary>
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            /// <summary>
            /// The window should be placed above all non-topmost windows and should stay above them, even when the window is deactivated. To add or remove this style, use the SetWindowPos function.
            /// </summary>
            WS_EX_TOPMOST = 0x00000008,
            /// <summary>
            /// The window accepts drag-drop files.
            /// </summary>
            WS_EX_ACCEPTFILES = 0x00000010,
            /// <summary>
            /// The window should not be painted until siblings beneath the window (that were created by the same thread) have been painted. The window appears transparent because the bits of underlying sibling windows have already been painted.
            ///To achieve transparency without these restrictions, use the SetWindowRgn function.
            /// </summary>
            WS_EX_TRANSPARENT = 0x00000020,

            /// <summary>
            /// The window is a MDI child window.
            /// </summary>

            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,

            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,

            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),

            WS_EX_LAYERED = 0x00080000,

            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_LAYOUTRTL = 0x00400000,

            /// <summary>
            /// Paints all descendants of a window in bottom-to-top painting order using double-buffering. Bottom-to-top painting order allows a descendent window to have translucency (alpha) and transparency (color-key) effects, but only if the descendent window also has the WS_EX_TRANSPARENT bit set. Double-buffering allows the window and its descendents to be painted without flicker. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.
            /// </summary>

            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000
            //#endif /* WIN32WINNT >= 0x0500 */

        }

    
}
