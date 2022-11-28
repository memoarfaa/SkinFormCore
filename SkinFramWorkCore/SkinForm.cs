// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;
using static Interop;
using System.Diagnostics;
using SkinFramWorkCore;
using static SkinFramWorkCore.SkinExtensions;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System;
using System.Drawing.Drawing2D;

namespace SkinFramWorkCore
{
    public partial class SkinForm : Form
    {
        #region fields
        private User32.TRACKMOUSEEVENT _trackMouseEvent;
        private bool _trackingMouseEvent;
        private int _captionHeight;
        private int _borderWidth;
        private Color _activeCaptionColor = SystemColors.ActiveCaption;
        private DwmButtonState _buttonState = DwmButtonState.Normal;
        private CaptionButton? _captionButton;
        private int _ncOpacity = 255;
        private int _borderRadius;
        private const int DTT_COMPOSITED = 8192;
        private const int DTT_GLOWSIZE = 2048;
        private const int DTT_TEXTCOLOR = 1;
        #endregion

        #region Constructor
        public SkinForm()
        {
            InitializeComponent();
            ActiveCaptionColor = DefaultCaptionColor;
            CaptionHeight = DefaultCaptionHeight(this);
            ControlBoxBounds = DefaultControlBoxBounds;
            BorderWidth = DefaultBorderWidth;
        }
        #endregion

        #region Properties
        private static int DefaultBorderWidth
        {
            get
            {
                return User32.GetSystemMetrics(User32.SystemMetric.SM_CXFRAME) * 2;
            }
        }

        /// <summary>
        /// return Default Caption Height per Dpi
        /// </summary>
        private static int DefaultCaptionHeight(Form form)
        {

            var isToolWindow = form.FormBorderStyle == FormBorderStyle.SizableToolWindow || form.FormBorderStyle == FormBorderStyle.FixedToolWindow;
            int height = isToolWindow ?
                User32.GetSystemMetrics(User32.SystemMetric.SM_CYSMCAPTION) : User32.GetSystemMetrics(User32.SystemMetric.SM_CYCAPTION);
            return User32.GetSystemMetrics(User32.SystemMetric.SM_CYFRAME) + height + User32.GetSystemMetrics(User32.SystemMetric.SM_CXPADDEDBORDER);
        }

        /// <summary>
        /// check if there is a Maximized child
        /// this used to draw internal mdi menu in case MainMenuStrip is not present
        /// </summary>
        private bool IsMaxChild => ActiveMdiChild != null && ActiveMdiChild.WindowState is FormWindowState.Maximized;

        /// <summary>
        /// check is <see cref="Form"/> is Active
        /// </summary>
        private bool IsActive { get; set; }
        private static bool IsColoredTitleBar
        {
            get
            {
                int? colorPrevalence = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorPrevalence", 0) as int?;
                return colorPrevalence.HasValue && colorPrevalence.Value == 1;
            }
        }

        /// <summary>
        /// return ControlBox Bounds
        /// </summary>
        private RECT ControlBoxBounds { get; set; }
        /// <summary>
        /// return Default ControlBox Bounds
        /// </summary>
        private Rectangle DefaultControlBoxBounds
        {
            get
            {
                switch (MsStylePlatform)
                {
                    case SkinPlatform.Win8:
                    case SkinPlatform.Win81:
                        return new Rectangle(7, 0, 105, 22);
                    case SkinPlatform.Win10:
                    case SkinPlatform.Win11:
                        return new Rectangle(7, 0, User32.GetSystemMetrics(User32.SystemMetric.SM_CXMINTRACK) + DefaultBorderWidth, _captionHeight);
                    default:
                        return new Rectangle(7, 0, User32.GetSystemMetrics(User32.SystemMetric.SM_CXMINTRACK) + DefaultBorderWidth, _captionHeight);
                }

            }
        }

        /// <summary>
        /// return Default Caption Color
        /// </summary>
        private static Color DefaultCaptionColor
        {
            get
            {
                int? ColorizationColor;
                int? ColorizationColorBalance;
                try
                {
                    ColorizationColor = (int?)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", 0);
                    ColorizationColorBalance = (int?)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColorBalance", 0);
                    if (!ColorizationColor.HasValue || !ColorizationColorBalance.HasValue)
                        return SystemColors.ActiveBorder;
                    int ALPHA = (255 * ColorizationColorBalance.Value / 100); // Convert from 0-100 to 0-255
                    int RED = (byte)((ColorizationColor.Value >> 16) & 0xFF);
                    int GREEN = (byte)((ColorizationColor.Value >> 8) & 0xFF);
                    int BLUE = (byte)(ColorizationColor.Value & 0xFF);
                    int r = (((RED * ALPHA) + (0xD9 * (255 - ALPHA))) / 255);
                    int g = (byte)(((GREEN * ALPHA) + (0xD9 * (255 - ALPHA))) / 255);
                    int b = (byte)(((BLUE * ALPHA) + (0xD9 * (255 - ALPHA))) / 255);
                    return IsColoredTitleBar || MsStylePlatform is SkinPlatform.Win8 || MsStylePlatform is SkinPlatform.Win81 ? Color.FromArgb(r, g, b) : Color.White;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return SystemColors.ActiveBorder;
                }
            }
        }

        public new FormBorderStyle FormBorderStyle
        {
            get
            {
                return base.FormBorderStyle;
            }
            set
            {

                base.FormBorderStyle = value;
                if (DesignMode)
                {
                    Message m = Message.Create(Handle, (int)WindowsMessages.NCPAINT, IntPtr.Zero, IntPtr.Zero);
                    OnWmNcPaint(ref m);
                }
            }
        }



        public int CaptionHeight
        {
            get
            {
                return _captionHeight;
            }

            set
            {
                _captionHeight = value;

                if (DesignMode)
                {
                    InvalidateWindow();
                }
            }
        }


        public int BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
                if (DesignMode)
                {
                    InvalidateWindow();
                }
            }
        }


        public Color InActiveCaptionColor { get; set; } = (MsStylePlatform == SkinPlatform.Win10 || MsStylePlatform == SkinPlatform.Win11) ? Color.White : Color.FromArgb(235, 235, 235);


        public Color ActiveCaptionColor
        {
            get
            {
                return _activeCaptionColor;
            }
            set
            {
                _activeCaptionColor = value;
                if (DesignMode)
                {
                    InvalidateWindow();
                }
            }
        }


        public int NcOpacity
        {
            get
            {
                return _ncOpacity;
            }
            set
            {
                _ncOpacity = value;
                if (DesignMode)
                {
                    InvalidateWindow();
                }
            }
        }


        public bool AllowNcTransparency { get; set; } = true;


        public int BorderRadius
        {
            get
            {
                return _borderRadius;
            }
            set
            {
                _borderRadius = value;
                if (DesignMode)
                {
                    InvalidateWindow();
                }
            }
        }
        #endregion

        #region Overrides

        protected override void WndProc(ref Message m)
        {
            switch ((WindowsMessages)m.Msg)
            {
                case WindowsMessages.NCACTIVATE:
                    OnWmNcActive(ref m);
                    break;
                case WindowsMessages.NCCREATE:
                    OnWmNcCreate(ref m);
                    break;
                case WindowsMessages.NCPAINT:
                    OnWmNcPaint(ref m);
                    break;
                case WindowsMessages.NCMOUSEMOVE:
                    OnWmNcMouseMove(ref m);
                    break;
                case WindowsMessages.NCMOUSELEAVE:
                    OnWmNcMouseLeave(ref m);
                    break;
                case WindowsMessages.NCCALCSIZE:
                    OnWmNcCalcSize(ref m);
                    break;
                case WindowsMessages.NCLBUTTONDOWN:
                    OnWmNclButtonDown(ref m);
                    break;
                case WindowsMessages.NCLBUTTONUP:
                    OnWmNclButtonUp(ref m);
                    break;
                case WindowsMessages.NCHITTEST:
                    OnWmNcHitTest(ref m);
                    break;
                case WindowsMessages.MDIACTIVATE:
                    base.WndProc(ref m);
                    if (m.WParam == Handle)
                        IsActive = false;
                    else if (m.LParam == Handle)
                        IsActive = true;
                    OnWmNcPaint(ref m);
                    break;
                case WindowsMessages.SIZE:
                    base.WndProc(ref m);
                    SetRoundRegion();
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var mdiClient = Controls.Cast<Control>().OfType<MdiClient>().FirstOrDefault();

            if (mdiClient != null)
            {
                new MdiNativeWindow(mdiClient);
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                if (IsMdiChild && AllowNcTransparency)
                    cp.ExStyle |= (int)WindowStyles.WS_EX_TRANSPARENT;
                return cp;
            }
        }

        protected override void OnRightToLeftLayoutChanged(EventArgs e)
        {
            base.OnRightToLeftLayoutChanged(e);
            if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
            {
                var mdiClient = Controls.Cast<Control>().OfType<MdiClient>().FirstOrDefault();

                if (mdiClient != null)
                {
                    mdiClient.Paint += Rtl_Paint;
                }

                else
                {
                    Paint += Rtl_Paint;
                }
            }
        }

        #endregion

        #region Methods
        private void Rtl_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.Clear( IsMdiContainer ? SystemColors.AppWorkspace: BackColor);
            if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
            {
                e.Graphics.Transform = new Matrix(-1, 0, 0, 1, Width - BorderWidth * 2 + 1, 0);

            }
            var clipRectangle = e.ClipRectangle.RtlRectangle(Width - BorderWidth * 2 + 1);
            if ((HScroll || VScroll) && BackgroundImage != null &&
                (BackgroundImageLayout == ImageLayout.Zoom || BackgroundImageLayout == ImageLayout.Stretch ||
                 BackgroundImageLayout == ImageLayout.Center))
            {
                if (IsImageTransparent(BackgroundImage))
                    PaintTransparentBackground(e, clipRectangle);
                e.Graphics.DrawBackgroundImage(BackgroundImage, BackColor, BackgroundImageLayout, clipRectangle,
                    clipRectangle, clipRectangle.Location, RightToLeft);
            }
            else
            {

                PaintBackground(e, clipRectangle, BackColor);
            }
        }
        private void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor)
        {
            if (BackColor == Color.Transparent)
                PaintTransparentBackground(e, rectangle);
            if (BackgroundImage != null && !SystemInformation.HighContrast)
            {
                if (BackgroundImageLayout == ImageLayout.Tile && IsImageTransparent(BackgroundImage))
                    PaintTransparentBackground(e, rectangle);
                e.Graphics.DrawBackgroundImage(BackgroundImage, backColor, BackgroundImageLayout, ClientRectangle,
                    ClientRectangle, Point.Empty, RightToLeft);
            }

        }

        private void PaintTransparentBackground(PaintEventArgs e, Rectangle rectangle, Region? transparentRegion = null)
        {
            Graphics graphics = e.Graphics;
            Control? parentInternal = Parent;
            if (parentInternal != null)
            {
                if (Application.RenderWithVisualStyles)
                {
                    GraphicsState? gstate = null;
                    if (transparentRegion != null)
                        gstate = graphics.Save();
                    try
                    {
                        if (transparentRegion != null)
                            graphics.Clip = transparentRegion;
                        ButtonRenderer.DrawParentBackground(graphics, rectangle, this);
                    }
                    finally
                    {
                        if (gstate != null)
                            graphics.Restore(gstate);
                    }
                }
                else
                {
                    Rectangle rectangle1 = new Rectangle(-Left, -Top, parentInternal.Width, parentInternal.Height);
                    Rectangle clipRect = new Rectangle(rectangle.Left + Left, rectangle.Top + Top, rectangle.Width, rectangle.Height);
                    using (Graphics windowsGraphics = graphics)
                    {
                        windowsGraphics.TranslateTransform(-Left, -Top);
                        using (PaintEventArgs e1 = new PaintEventArgs(windowsGraphics, clipRect))
                        {
                            if (transparentRegion != null)
                            {
                                e1.Graphics.Clip = transparentRegion;
                                e1.Graphics.TranslateClip(-rectangle1.X, -rectangle1.Y);
                            }
                            try
                            {
                                InvokePaintBackground(parentInternal, e1);
                                InvokePaint(parentInternal, e1);
                            }
                            finally
                            {
                                if (transparentRegion != null)
                                    e1.Graphics.TranslateClip(rectangle1.X, rectangle1.Y);
                            }
                        }
                    }
                }
            }
            else
                graphics.FillRectangle(SystemBrushes.Control, rectangle);
        }


        private static bool IsImageTransparent(Image backgroundImage)
        {
            return backgroundImage != null && (backgroundImage.Flags & 2) > 0;
        }
        /// <summary>
        /// Invalidate the <see cref="Form"/> and make it repaint immediately use to fix .net core Designer.
        /// see https://github.com/dotnet/winforms/issues/8107
        /// </summary>
        private void InvalidateWindow()
        {
            if (!IsDisposed && IsHandleCreated)
            {
                User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, User32.SWPFlags.SWP_NOMOVE | User32.SWPFlags.SWP_NOSIZE |
                     User32.SWPFlags.SWP_NOZORDER | User32.SWPFlags.SWP_FRAMECHANGED);
            }
        }

        /// <summary>
        /// Hook the Nonclient area Mouse Event
        /// </summary>
        /// <param name="hwnd"></param>
        private void HookNcMouseEvent(IntPtr hwnd)
        {
            if (!_trackingMouseEvent)
            {
                _trackingMouseEvent = true;
                if (_trackMouseEvent.IsDefault())
                {
                    _trackMouseEvent = new User32.TRACKMOUSEEVENT
                    {
                        cbSize = (uint)Marshal.SizeOf<User32.TRACKMOUSEEVENT>(),
                        dwFlags = User32.TME.LEAVE | User32.TME.NONCLIENT,
                        hwndTrack = hwnd,
                        dwHoverTime = 1
                    };
                }

                User32.TrackMouseEvent(ref _trackMouseEvent);
            }
        }

        /// <summary>
        /// UnHook the  Mouse Event
        /// </summary>
        private void UnHookNcMouseEvent() => _trackingMouseEvent = false;
        /// <summary>
        /// Draw NonClient area Caption Buttons in specified <see cref="Graphics"/>
        /// </summary>
        /// <param name="width"> the Width of <see cref="Form"/> in pixel</param>
        /// <param name="nCGraphics"><see cref="Graphics"/> Caption buttons will be drawn on it</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private void DrawCaptionButtons(int width, Graphics nCGraphics)
        {
            int btnWidth = ControlBoxBounds.Width / 3;
            int btnbottom = ControlBoxBounds.Height;
            Rectangle closeBtnRect = new Rectangle(width - BorderWidth - btnWidth, 1, btnWidth, btnbottom);
            Rectangle maxBtnRect = new Rectangle(width - btnWidth * 2, 1, btnWidth, btnbottom);
            Rectangle minBtnRect = new Rectangle(width - ControlBoxBounds.Width, 1, btnWidth, btnbottom);
            bool isMinimizedChild = IsMdiChild && WindowState == FormWindowState.Minimized;
            switch (MsStylePlatform)
            {
                case SkinPlatform.Vista:
                case SkinPlatform.Win7:
                case SkinPlatform.Win8:
                case SkinPlatform.Win81:
                    closeBtnRect = new Rectangle(width - BorderWidth - 45, 1, 45, btnbottom);
                    maxBtnRect = new Rectangle(width - BorderWidth - closeBtnRect.Width - 25, 1, 25, btnbottom);
                    minBtnRect = new Rectangle(width - BorderWidth - closeBtnRect.Width - 50, 1, 25, btnbottom);
                    break;
                case SkinPlatform.Win10:
                case SkinPlatform.Win11:
                    closeBtnRect = new Rectangle(width - BorderWidth - btnWidth, 1, btnWidth, btnbottom);
                    maxBtnRect = new Rectangle(width - btnWidth * 2 - BorderWidth, 1, btnWidth, btnbottom);
                    minBtnRect = new Rectangle(width - btnWidth * 3 - BorderWidth, 1, btnWidth, btnbottom);
                    break;
            }

            if (isMinimizedChild)
            {
                btnWidth = 35;
                closeBtnRect = new Rectangle(width - BorderWidth - btnWidth, 1, btnWidth, btnbottom);
                maxBtnRect = new Rectangle(width - btnWidth * 2, 1, closeBtnRect.Width, btnWidth);
                minBtnRect = new Rectangle(width - btnWidth * 3, 1, closeBtnRect.Width, btnWidth);
            }

            CaptionButton resMaxBtn = WindowState == FormWindowState.Maximized ? CaptionButton.Restore : CaptionButton.Maximize;
            CaptionButton minresBtn = isMinimizedChild ? CaptionButton.Restore : CaptionButton.Minimize;
            DwmButtonState disableState = IsActive ? DwmButtonState.Normal : DwmButtonState.Disabled;
            switch (_captionButton)
            {
                case CaptionButton.Close:
                    nCGraphics.DrawCaptionButton(closeBtnRect, CaptionButton.Close, _buttonState, IsActive);
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect, minresBtn, disableState, IsActive);
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect, resMaxBtn, disableState, IsActive);
                    break;
                case CaptionButton.Minimize:
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect, minresBtn, _buttonState, IsActive);
                    nCGraphics.DrawCaptionButton(closeBtnRect, CaptionButton.Close, disableState, IsActive);
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect, resMaxBtn, disableState, IsActive);
                    break;
                case CaptionButton.Maximize:
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect, resMaxBtn, _buttonState, IsActive);
                    nCGraphics.DrawCaptionButton(closeBtnRect, CaptionButton.Close, disableState, IsActive);
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect, minresBtn, disableState, IsActive);
                    break;
                case CaptionButton.Restore:
                    break;
                case CaptionButton.Help:
                    break;

                case null:
                    nCGraphics.DrawCaptionButton(closeBtnRect, CaptionButton.Close, disableState, IsActive);
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect, minresBtn, disableState, IsActive);
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect, resMaxBtn, disableState, IsActive);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle <see cref="WindowsMessages.NCCALCSIZE"/> <see cref="Message"/>
        /// </summary>
        /// <param name="m"></param>
        private void OnWmNcCalcSize(ref Message m)
        {
            if (FormBorderStyle == FormBorderStyle.None || (IsMdiChild && WindowState == FormWindowState.Maximized) || m.WParam.ToInt32() != 1)
            {
                base.WndProc(ref m);
                return;
            }


            int mdiMenuHeight = IsMaxChild && MainMenuStrip is null ? User32.GetSystemMetrics(User32.SystemMetric.SM_CYMENUSIZE) : 0;
            int defaultCaptionHeight = IsMaxChild && MainMenuStrip is null ? DefaultCaptionHeight(this) : _captionHeight;
            int captionHeight = defaultCaptionHeight + mdiMenuHeight;
            User32.NCCALCSIZE_PARAMS nccParama = Marshal.PtrToStructure<User32.NCCALCSIZE_PARAMS>(m.LParam);
            nccParama.rgrc0.left += BorderWidth;
            nccParama.rgrc0.top += captionHeight;
            nccParama.rgrc0.right -= BorderWidth;
            nccParama.rgrc0.bottom -= BorderWidth;
            Marshal.StructureToPtr(nccParama, m.LParam, true);
            if (AllowNcTransparency && !IsMdiChild)
            {
                UxTheme.MARGINS winMargins = new UxTheme.MARGINS { cxLeftWidth = BorderWidth, cxRightWidth = BorderWidth, cyTopHeight = CaptionHeight, cyBottomHeight = BorderWidth };
                UxTheme.MARGINS win11Margins = new UxTheme.MARGINS { cxLeftWidth = 1, cxRightWidth = 1, cyTopHeight = 1, cyBottomHeight = 1 };
                UxTheme.MARGINS margins = MsStylePlatform == SkinPlatform.Win11 ? win11Margins : winMargins;
                Dwmapi.DwmExtendFrameIntoClientArea(this.Handle, ref margins);
            }

            m.Result = IntPtr.Zero;
        }

        /// <summary>
        /// Handle <see cref="WindowsMessages.NCPAINT"/> <see cref="Message"/>
        /// </summary>
        /// <param name="m"></param>
        private void OnWmNcPaint(ref Message m)
        {
            //Windows 8,8.1 High Contrast theme not work with OpenThemeData and IsThemeActive return false its required app manifest so let's do default
            //See https://learn.microsoft.com/en-us/windows/win32/controls/supporting-high-contrast-themes

            if ((IsMdiChild && WindowState == FormWindowState.Maximized) || !VisualStyleRenderer.IsSupported || FormBorderStyle == FormBorderStyle.None)
            {
                base.WndProc(ref m);
                return;
            }


            //Very good at resizing Form  when resizing Form from left in Desktop in windows 10 and windows 11
            //no flicker at all but it can't use without Dwm Frame and with rounded corner
            if (m.Msg == (int)WindowsMessages.NCPAINT)
            {
                if (!(!AllowNcTransparency && BorderRadius > 0))
                {
                    User32.SetWindowRgn(Handle, default, false);
                }
            }

            Gdi32.HDC hdc = User32.GetWindowDC(Handle);
            if (!hdc.IsNull)
            {
                RECT currentBounds = new RECT();
                User32.GetWindowRect(m.HWnd, ref currentBounds);
                User32.OffsetRect(ref currentBounds, -currentBounds.left, -currentBounds.top);
                RECT currentClient = new RECT();
                User32.GetClientRect(Handle, ref currentClient);

                int captionHeight = IsMaxChild && MainMenuStrip is null ? DefaultCaptionHeight(this) + User32.GetSystemMetrics(User32.SystemMetric.SM_CYMENUSIZE) : _captionHeight;
                User32.OffsetRect(ref currentClient, BorderWidth, captionHeight);

                // if minimized child there is no client Rectangle
                if (!(IsMdiChild && WindowState == FormWindowState.Minimized))
                    Gdi32.ExcludeClipRect(hdc, currentClient.left, currentClient.top, currentClient.right, currentClient.bottom);

                UxTheme.BP_PAINTPARAMS paintParams = new UxTheme.BP_PAINTPARAMS(UxTheme.BPPF.NonClient);

                // if Child Form and Allow Nonclient area  transparency Blend the buffer to get transparency effect.
                if (IsMdiChild && AllowNcTransparency)
                {
                    paintParams.BlendFunction = new UxTheme.BP_PAINTPARAMS.BLENDFUNCTION(0, 0, 255, 1);
                }

                //The best buffer to use in drawing at Nonclient area is Uxtheme BufferedPaint
                //it's better than BitBlt,BufferedGraphics and it's allow Nonclient transparency in Child Form
                //DrawThemeTextEx required BufferedPaint DIB to be TopDownDIB.
                //see https://learn.microsoft.com/en-us/windows/win32/api/uxtheme/ns-uxtheme-dttopts DTT_COMPOSITED flag
                UxTheme.HPAINTBUFFER bufferedPaint = UxTheme.BeginBufferedPaint(hdc, ref currentBounds, UxTheme.BP_BUFFERFORMAT.TopDownDIB, ref paintParams, out Gdi32.HDC memdc);
                Color color = IsActive ? ActiveCaptionColor : InActiveCaptionColor;
                Color ncColor = Color.FromArgb(NcOpacity, color);
                using (Graphics nCGraphics = Graphics.FromHdcInternal(memdc.Handle))
                {
                    if (BorderRadius > 0)
                    {
                        nCGraphics.IntersectClip(new Region(RoundedRect(currentBounds, BorderRadius)));
                    }

                    nCGraphics.Clear(ncColor);

                    // The internal Mdi menu must be redrawn if the MainMenuStrip is not present because it is in the Nonclient area of the window
                    if (MainMenuStrip is null && IsMaxChild)
                    {
                        nCGraphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(BorderWidth, _captionHeight, Width - BorderWidth * 2, captionHeight - _captionHeight));

                        if (ActiveMdiChild?.Icon is object)
                        {
                            nCGraphics.DrawIcon(new Icon(ActiveMdiChild.Icon, SystemInformation.SmallIconSize), BorderWidth + 2, DefaultCaptionHeight(this));
                        }

                        int mdiBtnWidth = User32.GetSystemMetrics(User32.SystemMetric.SM_CXMENUSIZE);
                        VisualStyleRenderer mdiCloseButtonRenderer = new VisualStyleRenderer(VisualStyleElement.Window.MdiCloseButton.Normal);
                        VisualStyleRenderer mdiRestoreButtonRenderer = new VisualStyleRenderer(VisualStyleElement.Window.MdiRestoreButton.Normal);
                        VisualStyleRenderer mdiMinButtonRenderer = new VisualStyleRenderer(VisualStyleElement.Window.MdiMinButton.Normal);
                        mdiCloseButtonRenderer.DrawBackground(nCGraphics, new Rectangle(Width - DefaultBorderWidth - mdiBtnWidth, DefaultCaptionHeight(this), mdiBtnWidth, mdiBtnWidth));
                        mdiRestoreButtonRenderer.DrawBackground(nCGraphics, new Rectangle(Width - DefaultBorderWidth - (mdiBtnWidth * 2) + 1, DefaultCaptionHeight(this), mdiBtnWidth, mdiBtnWidth));
                        mdiMinButtonRenderer.DrawBackground(nCGraphics, new Rectangle(Width - DefaultBorderWidth - (mdiBtnWidth * 3) + 1, DefaultCaptionHeight(this), mdiBtnWidth, mdiBtnWidth));
                    }

                    // Draw ControlBox
                    if (ControlBox)
                    {
                        DrawCaptionButtons(Width, nCGraphics);
                    }

                    // Draw Icon
                    if (ShowIcon && Icon is object && FormBorderStyle != FormBorderStyle.FixedToolWindow && FormBorderStyle != FormBorderStyle.SizableToolWindow && FormBorderStyle != FormBorderStyle.FixedDialog)
                    {
                        nCGraphics.DrawIcon(new Icon(Icon, SystemInformation.SmallIconSize), 9, 7);
                    }
                }

                //Draw Text
                //Draw Text With  nCGraphics.DrawString has very bad look in Transparency Form so we draw it with DrawThemeTextEx
                if (!string.IsNullOrEmpty(Text))
                {
                    VisualStyleRenderer textRenderer = new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);
                    UxTheme.DTTOPTS dttOpts = default;
                    dttOpts.dwSize = Marshal.SizeOf(typeof(UxTheme.DTTOPTS));
                    dttOpts.dwFlags = DTT_COMPOSITED | DTT_GLOWSIZE | DTT_TEXTCOLOR;
                    dttOpts.crText = ColorTranslator.ToWin32(ncColor.ContrastColor());
                    dttOpts.iGlowSize = 8;
                    Font font = SystemFonts.CaptionFont ?? Font;
                    Gdi32.HGDIOBJ fontHandle = (Gdi32.HGDIOBJ)font.ToHfont();
                    Gdi32.SelectObject(memdc, fontHandle);
                    RECT textBounds = new RECT(30, 6, Width - ControlBoxBounds.Width, 30);
                    if (IsMdiChild && WindowState == FormWindowState.Minimized)
                        textBounds = new RECT(30, 6, Width - 120, 30);
                    UxTheme.DrawThemeTextEx(textRenderer.Handle, memdc, 0, 0, Text, -1, (int)TextFormatFlags.WordEllipsis, ref textBounds, ref dttOpts);
                    Gdi32.DeleteObject(fontHandle);
                }

                UxTheme.EndBufferedPaint(bufferedPaint, true);
            }

            User32.ReleaseDC(m.HWnd, hdc);
        }

        /// <summary>
        /// Handle <see cref="WindowsMessages.NCACTIVATE"/> <see cref="Message"/>
        /// </summary>
        /// <param name="m"></param>
        private void OnWmNcActive(ref Message m)
        {
            base.WndProc(ref m);
            IsActive = m.WParam.ToInt32() > 0;
            OnWmNcPaint(ref m);
        }

        /// <summary>
        /// Handle <see cref="WindowsMessages.NCCREATE"/> <see cref="Message"/>
        /// </summary>
        /// <param name="m"></param>
        private void OnWmNcCreate(ref Message m)
        {
            base.WndProc(ref m);
            UxTheme.SetWindowTheme(m.HWnd, "Window", "DwmWindow");

            if (IsMdiChild && AllowNcTransparency)
            {
                Composited(User32.GetParent(m.HWnd));
            }

            SetRoundRegion();

            // It's better to set background brush to WindowFrame color instead of Black
            Gdi32.HBRUSH backgroundBrush = Gdi32.CreateSolidBrush(ColorTranslator.ToWin32(SystemColors.WindowFrame));
            User32.SetClassLong(m.HWnd, User32.GCL.GCLP_HBRBACKGROUND, backgroundBrush.Handle);
        }

        /// <summary>
        /// Handle <see cref="WindowsMessages.NCMOUSEMOVE"/> <see cref="Message"/>
        /// </summary>
        /// <param name="m"></param>
        private void OnWmNcMouseMove(ref Message m)
        {
            base.WndProc(ref m);
            HookNcMouseEvent(m.HWnd);
            switch ((int)m.WParam)
            {
                case (int)User32.NCHITTEST.HTMINBUTTON:
                    if (this.IsDrawMinimizeBox())
                    {
                        _captionButton = CaptionButton.Minimize;
                        _buttonState = DwmButtonState.Hot;
                        OnWmNcPaint(ref m);
                    }

                    break;
                case (int)User32.NCHITTEST.HTMAXBUTTON:

                    if (this.IsDrawMaximizeBox())
                    {
                        _captionButton = CaptionButton.Maximize;
                        _buttonState = DwmButtonState.Hot;
                        OnWmNcPaint(ref m);
                    }

                    break;
                case (int)User32.NCHITTEST.HTCLOSE:
                    _buttonState = DwmButtonState.Hot;
                    _captionButton = CaptionButton.Close;
                    OnWmNcPaint(ref m);
                    break;
                default:
                    _captionButton = null;
                    base.WndProc(ref m);
                    OnWmNcPaint(ref m);
                    break;
            }
        }

        /// <summary>
        /// Handle <see cref="WindowsMessages.NCMOUSELEAVE"/> <see cref="Message"/>
        /// </summary>
        /// <param name="m"></param>
        private void OnWmNcMouseLeave(ref Message m)
        {
            base.WndProc(ref m);
            UnHookNcMouseEvent();
            _captionButton = null;
            _buttonState = DwmButtonState.Normal;
            OnWmNcPaint(ref m);
        }

        /// <summary>
        /// Handle <see cref="WindowsMessages.NCLBUTTONDOWN"/> <see cref="Message"/>
        /// </summary>
        /// <param name="m"></param>
        private void OnWmNclButtonDown(ref Message m)
        {
            switch ((int)m.WParam)
            {
                case (int)User32.NCHITTEST.HTMINBUTTON:
                    _captionButton = CaptionButton.Minimize;
                    _buttonState = DwmButtonState.Pressed;
                    OnWmNcPaint(ref m);
                    break;
                case (int)User32.NCHITTEST.HTMAXBUTTON:
                    _captionButton = CaptionButton.Maximize;
                    _buttonState = DwmButtonState.Pressed;
                    OnWmNcPaint(ref m);
                    break;

                case (int)User32.NCHITTEST.HTCLOSE:
                    _captionButton = CaptionButton.Close;
                    _buttonState = DwmButtonState.Pressed;
                    OnWmNcPaint(ref m);
                    break;

                default:
                    base.WndProc(ref m);

                    break;
            }
        }

        /// <summary>
        /// Handle <see cref="WindowsMessages.NCLBUTTONUP"/> <see cref="Message"/>
        /// </summary>
        /// <param name="m"></param>
        private void OnWmNclButtonUp(ref Message m)
        {
            switch ((int)m.WParam)
            {
                case (int)User32.NCHITTEST.HTMINBUTTON:
                    if (this.IsDrawMinimizeBox())
                        WindowState = WindowState == FormWindowState.Minimized ? FormWindowState.Normal : FormWindowState.Minimized;
                    break;
                case (int)User32.NCHITTEST.HTMAXBUTTON:
                    if (this.IsDrawMaximizeBox())
                        WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                    break;

                case (int)User32.NCHITTEST.HTCLOSE:
                    Close();
                    break;
                default:
                    _captionButton = null;
                    base.WndProc(ref m);
                    OnWmNcPaint(ref m);
                    break;
            }
        }

        /// <summary>
        /// Handle <see cref="WindowsMessages.NCHITTEST"/> <see cref="Message"/>
        /// </summary>
        /// <param name="m"></param>
        private void OnWmNcHitTest(ref Message m)
        {
            Point point = default;
            point.X = LOWORD(m.LParam.ToInt32());
            point.Y = HIWORD(m.LParam.ToInt32());
            RECT winRect = new RECT();
            User32.GetWindowRect(Handle, ref winRect);
            int width = winRect.right - winRect.left;
            point.X -= winRect.left;
            point.Y -= winRect.top;
            int btnWidth = ControlBoxBounds.Width / 3;
            int btnbottom = ControlBoxBounds.Height;
            Rectangle closeBtnRect = new Rectangle(width - 45 - BorderWidth, 1, 45, btnbottom);
            Rectangle restoreRect = new Rectangle(width - btnWidth * 2, 1, btnWidth, btnbottom);
            Rectangle minRect = new Rectangle(width - ControlBoxBounds.Width, 1, btnWidth * 2 + 1, btnbottom);

            switch (MsStylePlatform)
            {
                case SkinPlatform.Vista:
                case SkinPlatform.Win7:
                case SkinPlatform.Win8:
                case SkinPlatform.Win81:
                    closeBtnRect = new Rectangle(width - BorderWidth - 45, 1, 45, btnbottom);
                    restoreRect = new Rectangle(width - BorderWidth - closeBtnRect.Width - 25, 1, 25, btnbottom);
                    minRect = new Rectangle(width - BorderWidth - closeBtnRect.Width - 50, 1, 25, btnbottom);
                    break;
                case SkinPlatform.Win10:
                case SkinPlatform.Win11:
                    closeBtnRect = new Rectangle(width - BorderWidth - btnWidth, 1, btnWidth, btnbottom);
                    restoreRect = new Rectangle(width - btnWidth * 2, 1, btnWidth, btnbottom);
                    minRect = new Rectangle(width - btnWidth * 3, 1, btnWidth, btnbottom);
                    break;
            }

            if (IsMdiChild && WindowState == FormWindowState.Minimized)
            {
                base.WndProc(ref m);
            }

            else
            {
                if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                {
                    closeBtnRect = closeBtnRect.RtlRectangle(width);

                    restoreRect = restoreRect.RtlRectangle(width);
                    minRect = minRect.RtlRectangle(width);
                }

                if (closeBtnRect.Contains(point))
                {
                    m.Result = (IntPtr)User32.NCHITTEST.HTCLOSE;
                }

                else if (restoreRect.Contains(point))
                {
                    m.Result = (IntPtr)User32.NCHITTEST.HTMAXBUTTON;
                }

                else if (minRect.Contains(point))
                {
                    m.Result = (IntPtr)User32.NCHITTEST.HTMINBUTTON;
                }
                else
                    base.WndProc(ref m);
            }
        }

        private static void Composited(IntPtr hwnd)
        {
            IntPtr exStyle = User32.GetWindowLong(hwnd, User32.GWLIndex.GWL_EXSTYLE);
            int compositedStyle = (int)exStyle | 0x02000000; //WS_EX_COMPOSITED
            User32.SetWindowLong(hwnd, User32.GWLIndex.GWL_EXSTYLE, (IntPtr)compositedStyle);
        }

        /// <summary>
        /// Set <see cref="Form"/> round <see cref="Region"/> in case <see cref="AllowNcTransparency"/> is false and round corners greater than 0.
        /// this kind of <see cref="Region"/> is not recommended.
        /// </summary>
        private void SetRoundRegion()
        {
            if (BorderRadius <= 0 | AllowNcTransparency)
                return;
            RECT rWindow = new RECT();
            User32.GetWindowRect(Handle, ref rWindow);
            int x = RightToLeftLayout && RightToLeft == RightToLeft.Yes ? -rWindow.left - BorderWidth * 2 - 1 : -rWindow.left;
            User32.OffsetRect(ref rWindow, x, -rWindow.top);
            Region = new Region(RoundedRect(rWindow, BorderRadius));
        }

        private static int HIWORD(int i)
        {

            return (short)(i >> 16);
        }

        private static int LOWORD(int i)
        {
            return (short)(i & 0xFFFF);
        }
        #endregion

       
    }
}
