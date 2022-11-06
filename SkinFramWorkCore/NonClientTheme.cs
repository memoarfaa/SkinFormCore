using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static SkinFramWorkCore.NativeMethods;
using static SkinFramWorkCore.Extensions;
using Microsoft.Win32;
using System.Linq;

namespace SkinFramWorkCore
{
    [DesignerCategory("Code")]
    public partial class NonClientTheme : Form
    {
        public NonClientTheme()
        {
            ActiveCaptionColor = activeCaption;
        }
        #region Fields
        private const int TME_HOVER = 0x1;
        private const int TME_NONCLIENT = 0x10;
        private bool _bMouseTracking;
        private const uint TME_LEAVE = 0x00000002;
        private TrackmouseEvent _tme = TrackmouseEvent.Empty;
        private DwmButtonState _buttonState = DwmButtonState.Normal;
        private CaptionButton? _captionButton = null;
        private static bool _isColorEnable = (int)
                           Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorPrevalence", 0) == 1;
        private int _captionHeight = 31;
        private int _borderWidth = 8;
        private Color _activeCaptionColor = SystemColors.ActiveCaption;
        private int _ncOpacity = 255;
        private int _borderRadius =0;
        #endregion

        #region Properties
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
        private bool DrawRtl => RightToLeftLayout && RightToLeft == RightToLeft.Yes;
        private static RECT DefultControlBoxBounds
        {
            get
            {
                switch (GetMsstylePlatform())
                {

                    case Platform.Win8:
                    case Platform.Win81:
                        return RECT.FromRectangle(new Rectangle(7, 0, 105, 22));
                    case Platform.Win10:
                    case Platform.Win11:
                        return RECT.FromRectangle(new Rectangle(7, 0, 146, 30));
                    default:
                        return RECT.FromRectangle(new Rectangle(7, 0, 146, 30));
                }
            }
        }
        private RECT ControlBoxBounds { get; set; } = DefultControlBoxBounds;
        private bool IsActive { get; set; }
        [DefaultValue(31)]
        [Description("Caption hight of nonclient area.")]
        [Category("Theme")]
        public int CaptionHight
        {
            get
            {
                return _captionHeight;

            }
            set
            {

                _captionHeight = value;
                //Fix .Net Core designer does not return immediately
                InvalidateWindow();
            }
        }


        [DefaultValue(8)]
        [Description("Border Width around nonclient area.")]
        [Category("Theme")]
        public int BorderWidth
        {
            get
            {

                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
                InvalidateWindow();
            }
        }

        [DefaultValue(8)]
        [Description("Border Width around nonclient area.")]
        [Category("Opacity of nonclient area Color")]
        public int NcOpacity
        {
            get
            {
                return _ncOpacity;
            }
            set
            {
                _ncOpacity = value;
                InvalidateWindow();
            }
        }
        [DefaultValue(true)]
        [Description("Allow nonclient area transparency.")]
        [Category("Theme")]
        public bool AllowNcTransparency { get; set; } = true;

        [Category("Theme")]
        [Description("Set nonclient area active caption color.")]
        public Color ActiveCaptionColor
        {
            get
            {
                return _activeCaptionColor;
            }
            set
            {
                _activeCaptionColor = value;
                InvalidateWindow();
            }
        }

        [Category("Theme")]
        [Description("Set nonclient area inactive caption color.")]
        public Color InActiveCaptionColor { get; set; } = GetMsstylePlatform() == Platform.Win10 || GetMsstylePlatform() == Platform.Win11 ? Color.White : Color.FromArgb(235, 235, 235);
        [Category("Theme")]
        [Description("Set nonclient area round corners.")]
        public int BorderRadius 
        { get 
            { 
                return _borderRadius;
            } set 
            {
                _borderRadius = value;
                InvalidateWindow();
            } 
        }
        private static Color activeCaption
        {
            get
            {
                if (GetMsstylePlatform() == Platform.Win10 || GetMsstylePlatform() == Platform.Win11)
                {
                    int ColorizationColor;
                    int ColorizationColorBalance;
                    try
                    {
                        ColorizationColor =
                            (int)
                            Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", 0);
                        ColorizationColorBalance =
                            (int)
                            Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColorBalance", 0);

                        int ALPHA = (255 * ColorizationColorBalance / 100); // Convert from 0-100 to 0-255
                        int RED = (byte)((ColorizationColor >> 16) & 0xFF);
                        int GREEN = (byte)((ColorizationColor >> 8) & 0xFF);
                        int BLUE = (byte)(ColorizationColor & 0xFF);

                        int r = (((RED * ALPHA) + (0xD9 * (255 - ALPHA))) / 255);
                        int g = (byte)(((GREEN * ALPHA) + (0xD9 * (255 - ALPHA))) / 255);
                        int b = (byte)(((BLUE * ALPHA) + (0xD9 * (255 - ALPHA))) / 255);
                        return _isColorEnable ? Color.FromArgb(r, g, b) : Color.White;
                    }
                    catch (Exception ex)
                    {
                        DebugConsole.WriteLine(ex);
                        return SystemColors.ActiveBorder;
                    }
                }

                else if (GetMsstylePlatform() == Platform.Win8 || GetMsstylePlatform() == Platform.Win81)
                {
                    try
                    {
                        int ColorizationColor = (int)
                           Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", 0);
                        int ColorizationColorBalance = (int)
                                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColorBalance", 0);

                        var ALPHA = 255 * ColorizationColorBalance / 100; // Convert from 0-100 to 0-255
                        var RED = (ColorizationColor >> 16) & 0xFF;
                        var GREEN = (ColorizationColor >> 8) & 0xFF;
                        var BLUE = ColorizationColor & 0xFF;

                        var r = ((RED * ALPHA) + (0xD9 * (255 - ALPHA))) / 255;
                        var g = ((GREEN * ALPHA) + (0xD9 * (255 - ALPHA))) / 255;
                        var b = ((BLUE * ALPHA) + (0xD9 * (255 - ALPHA))) / 255;
                        return Color.FromArgb(r, g, b);
                    }
                    catch (Exception ex)
                    {
                        DebugConsole.WriteLine(ex);
                        return SystemColors.ActiveBorder;
                    }
                }

                else
                    return SystemColors.ActiveBorder;
            }


        }
        #endregion

        #region Overrides 
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            var mdiClient = Controls.Cast<Control>().OfType<MdiClient>().FirstOrDefault();

            if (mdiClient != null && RightToLeftLayout)
            {
                mdiClient.Paint += Rtl_Paint;
            }

        }
        protected override void OnRightToLeftLayoutChanged(EventArgs e)
        {
            base.OnRightToLeftLayoutChanged(e);
            var mdiClient = Controls.Cast<Control>().OfType<MdiClient>().FirstOrDefault();

            if (mdiClient != null && RightToLeftLayout)
            {
                mdiClient.Paint += Rtl_Paint;
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

        protected override void WndProc(ref Message m)
        {
            switch ((WindowsMessages)m.Msg)
            {

                case WindowsMessages.NCCREATE:
                    OnWmNcCreate(ref m);
                    SetWindowRegion(m.HWnd, BorderRadius);
                    break;
                case WindowsMessages.NCACTIVATE:
                    OnWmNcActive(ref m);

                    break;
                case WindowsMessages.MDIACTIVATE:
                    base.WndProc(ref m);
                    if (m.WParam == Handle)
                        IsActive = false;
                    else if (m.LParam == Handle)
                        IsActive = true;
                    OnWmNcPaint(ref m);
                    break;
                case WindowsMessages.NCPAINT:
                    OnWmNcPaint(ref m);
                    break;
                case WindowsMessages.NCCALCSIZE:
                    OnWmNcCalcSize(ref m);
                    break;
                case WindowsMessages.NCHITTEST:
                    OnWmNcHitTest(ref m);
                    break;
                case WindowsMessages.NCMOUSEMOVE:
                    OnWmNcMouseTracking(m.HWnd);
                    OnWmNcMouseMove(ref m);
                    break;
                case WindowsMessages.NCLBUTTONDOWN:
                    switch ((NCHITTEST)m.WParam)
                    {

                        case NCHITTEST.HTMINBUTTON:
                            _captionButton = CaptionButton.Minimize;
                            _buttonState = DwmButtonState.Pressed;
                            OnWmNcPaint(ref m);
                            break;
                        case NCHITTEST.HTMAXBUTTON:
                            _captionButton = CaptionButton.Maximize;
                            _buttonState = DwmButtonState.Pressed;
                            OnWmNcPaint(ref m);
                            break;

                        case NCHITTEST.HTCLOSE:
                            _captionButton = CaptionButton.Close;
                            _buttonState = DwmButtonState.Pressed;
                            OnWmNcPaint(ref m);
                            break;

                        default:
                            base.WndProc(ref m);

                            break;

                    }
                    break;
                case WindowsMessages.NCLBUTTONUP:
                    switch ((NCHITTEST)m.WParam)
                    {

                        case NCHITTEST.HTMINBUTTON:
                            WindowState = WindowState == FormWindowState.Minimized ? FormWindowState.Normal : FormWindowState.Minimized;
                            break;
                        case NCHITTEST.HTMAXBUTTON:
                            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                            break;

                        case NCHITTEST.HTCLOSE:
                            Close();
                            break;
                        default:
                            _captionButton = null;
                            base.WndProc(ref m);
                            OnWmNcPaint(ref m);
                            break;

                    }
                    break;
                case WindowsMessages.NCMOUSELEAVE:
                    ResetNcTracking(m.HWnd);
                    _captionButton = null;
                    _buttonState = DwmButtonState.Normal;
                    OnWmNcPaint(ref m);
                    break;
                case WindowsMessages.NCUAHDRAWCAPTION:
                case WindowsMessages.NCUAHDRAWFRAME:
                    _captionButton = null;
                    OnWmNcPaint(ref m);
                    break;
                case WindowsMessages.SIZE:
                    base.WndProc(ref m);
                    SetWindowRegion(m.HWnd, BorderRadius);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (DrawRtl)
            {
                Rtl_Paint(this, e);
            }
            else
            {
                base.OnPaintBackground(e);
            }

        }
        #endregion

        #region Methods
        private void InvalidateWindow()
        {
            if (!IsDisposed && IsHandleCreated)
            {
                SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0,
                    (int)(SWPFlags.SWP_NOACTIVATE | SWPFlags.SWP_NOMOVE | SWPFlags.SWP_NOSIZE |
                    SWPFlags.SWP_NOZORDER | SWPFlags.SWP_FRAMECHANGED));

                RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero,
                 (int)(NativeMethods.RedrawWindowOptions.RDW_FRAME | NativeMethods.RedrawWindowOptions.RDW_UPDATENOW | NativeMethods.RedrawWindowOptions.RDW_INVALIDATE));
            }
        }
        private void Rtl_Paint(object sender, PaintEventArgs e)
        {
            if (BackgroundImage != null)
            {
                e.Graphics.DrawBackgroundImage(BackgroundImage, BackColor, BackgroundImageLayout, ClientRectangle, ClientRectangle, Point.Empty, RightToLeft);
            }

        }
        private void ResetNcTracking(IntPtr hwnd)
        {
            _bMouseTracking = false;
        }
        private void OnWmNcHitTest(ref Message m)
        {
            POINT point;
            point.x = LOWORD(m.LParam.ToInt32());
            point.y = HIWORD(m.LParam.ToInt32());
            RECT winRect;
            GetWindowRect(m.HWnd, out winRect);
            int width = winRect.right - winRect.left;
            point.x -= winRect.left;
            point.y -= winRect.top;
            var btnWidth = ControlBoxBounds.Width / 3;
            var btnbottom = ControlBoxBounds.Height;
            var closeRect = new RECT(width - 45 - BorderWidth, 1, width - BorderWidth, btnbottom);
            var restoreRect = new RECT(width - btnWidth * 2, 1, width - btnWidth + 1, btnbottom);
            var minRect = new RECT(width - ControlBoxBounds.Width, 1, width - btnWidth * 2 + 1, btnbottom);


            if (IsMdiChild && WindowState == FormWindowState.Minimized)
            {

                base.WndProc(ref m);

            }
            else
            {
                if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                {
                    closeRect = closeRect.GetRtlRect(width);

                    restoreRect = restoreRect.GetRtlRect(width);
                    minRect = minRect.GetRtlRect(width);
                }

                if (PtInRect(ref closeRect, point))
                {
                    m.Result = (IntPtr)NCHITTEST.HTCLOSE;

                }
                else if (PtInRect(ref restoreRect, point))
                {
                    m.Result = (IntPtr)NCHITTEST.HTMAXBUTTON;

                }
                else if (PtInRect(ref minRect, point))
                {
                    m.Result = (IntPtr)NCHITTEST.HTMINBUTTON;
                }
                else
                    base.WndProc(ref m);
            }
        }
        private void OnWmNcMouseTracking(IntPtr hwnd)
        {
            if (_bMouseTracking) return;
            _tme.cbSize = (uint)Marshal.SizeOf(_tme);
            _tme.hwndTrack = hwnd;
            _tme.dwFlags = /*TME_HOVER |*/ TME_LEAVE | TME_NONCLIENT;
            _tme.dwHoverTime = 1/*HOVER_DEFAULT*/;
            TrackMouseEvent(ref _tme);
            _bMouseTracking = true;
        }
        private void OnWmNcMouseMove(ref Message m)
        {
           
            switch ((NCHITTEST)m.WParam)
            {
                case NCHITTEST.HTMINBUTTON:

                    _captionButton = CaptionButton.Minimize;
                    _buttonState = DwmButtonState.Hot;
                    OnWmNcPaint(ref m);
                    break;
                case NCHITTEST.HTMAXBUTTON:
                    _captionButton = CaptionButton.Maximize;
                    _buttonState = DwmButtonState.Hot;
                    OnWmNcPaint(ref m);
                    break;

                case NCHITTEST.HTCLOSE:
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
        private void OnWmNcCreate(ref Message m)
        {
            base.WndProc(ref m);
            SetWindowTheme(m.HWnd, "Window", "DWmWindow");
            if (AllowNcTransparency && !IsMdiChild)
            {
                MARGINS win10Margins = new MARGINS(BorderWidth, BorderWidth, CaptionHight, BorderWidth);
                MARGINS win11Margins = new MARGINS(1, 1, 1, 1);
                MARGINS margins = GetMsstylePlatform() == Platform.Win11 ? win11Margins : win10Margins;
                DwmExtendFrameIntoClientArea(m.HWnd, ref margins);
            }
            if ((IsMdiChild && AllowNcTransparency))
            {

                Composited(GetParent(m.HWnd));
            }

        }
        private void OnWmNcActive(ref Message m)
        {
            base.WndProc(ref m);
            IsActive = m.WParam.ToInt32() > 0;
            OnWmNcPaint(ref m);

        }
        private void OnWmNcPaint(ref Message m)
        {
            

            //Windows 8 High Contrast theme not work with OpenThemeData its required app manifest so let's do default
            //See https://learn.microsoft.com/en-us/windows/win32/controls/supporting-high-contrast-themes

            if (FormBorderStyle == FormBorderStyle.None || !IsThemeActive())
            {
                base.WndProc(ref m);
                return;
            }
            IntPtr hdc = GetWindowDC(m.HWnd);
            RECT winRect;
            GetWindowRect(m.HWnd, out winRect);
            OffsetRect(ref winRect, -winRect.left, -winRect.top);
            int width = winRect.right - winRect.left;
            RECT clientRect;
            GetClientRect(m.HWnd, out clientRect);

            OffsetRect(ref clientRect, BorderWidth, CaptionHight);
            if (m.Msg == (int)WindowsMessages.NCPAINT)
            {
                if (!(BorderRadius > 0 && !AllowNcTransparency))
                {
                    SetWindowRgn(m.HWnd, IntPtr.Zero, false);
                }
            }


            if (!(IsMdiChild && WindowState == FormWindowState.Minimized && FormBorderStyle != FormBorderStyle.None))
                ExcludeClipRect(hdc, clientRect.left, clientRect.top, clientRect.right, clientRect.bottom);
            var paintParams = new BP_PAINTPARAMS(BPPF.NoClip | BPPF.Erase | BPPF.NonClient);
            if (IsMdiChild && AllowNcTransparency)
            {
                paintParams.BlendFunction = new BP_PAINTPARAMS.BLENDFUNCTION(0, 0, 255, 1);
            }
            IntPtr memdc;
            var hbuff = BeginBufferedPaint(hdc, ref winRect, BP_BUFFERFORMAT.DIB, ref paintParams, out memdc);
            var color = IsActive ? ActiveCaptionColor : InActiveCaptionColor;
            var ncColor = Color.FromArgb(NcOpacity, color);
            using (var nCGraphics = Graphics.FromHdc(memdc))
            {
                if (BorderRadius > 0)
                {
                    nCGraphics.Clip = new Region(RoundedRect(winRect.ToRectangle(), BorderRadius));
                }
                nCGraphics.Clear(ncColor);

                
                if (ControlBox)
                {
                    DrawCaptionButtons(width, nCGraphics);
                }
                if (ShowIcon)
                {
                    nCGraphics.DrawIcon(new Icon(Icon, SystemInformation.SmallIconSize), 9, 7);
                }
                if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                {
                    nCGraphics.Transform = new Matrix(-1, 0, 0, 1, width, 0);
                }
                
                if (Text != null)
                {
                    var stringFormat = new StringFormat
                    {
                        Trimming = StringTrimming.EllipsisCharacter,

                    };
                    if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                    {
                        nCGraphics.Transform = new Matrix(-1, 0, 0, 1, width - 17, 0);
                        stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                    }


                    var i = RightToLeftLayout && RightToLeft == RightToLeft.Yes ? width - 17 - 31 : 31;
                    nCGraphics.DrawString(Text, SystemFonts.CaptionFont, IsActive ? Brushes.White : Brushes.Black, i, 10,
                        stringFormat);
                    
                }
            }

            EndBufferedPaint(hbuff, true);

            ReleaseDC(Handle, hdc);
        }

        public static void Composited(IntPtr hwnd)
        {

            int extendedStyle = GetWindowLong(hwnd, GWLIndex.GWL_EXSTYLE);
            var dwNewLong = extendedStyle | (int)WindowStyles.WS_EX_COMPOSITED;
            SetWindowLong(hwnd, GWLIndex.GWL_EXSTYLE, dwNewLong);
        }

        private void OnWmNcCalcSize(ref Message m)
        {
            if (FormBorderStyle == FormBorderStyle.None)
            {
                base.WndProc(ref m);
                return;
            }
            NCCALCSIZE_PARAMS ncc;


            if (m.WParam == new IntPtr(1))
            {
                ncc =
                    (NCCALCSIZE_PARAMS)
                    Marshal.PtrToStructure(m.LParam,
                        typeof(NCCALCSIZE_PARAMS));
                ncc.rgrc0.left += BorderWidth;
                ncc.rgrc0.top += CaptionHight;
                ncc.rgrc0.right -= BorderWidth;
                ncc.rgrc0.bottom -= BorderWidth;

                Marshal.StructureToPtr(ncc, m.LParam, true);
                m.Result = IntPtr.Zero;
            }
            else
            {
                base.WndProc(ref m);
            }

        }

        private void DrawCaptionButtons(int width, Graphics nCGraphics)
        {
            var btnWidth = ControlBoxBounds.Width / 3;
            var btnbottom = ControlBoxBounds.Height;
            var closeRect = new Rectangle(width - BorderWidth - 45, 1, 45, btnbottom);
            var maxBtnRect = new RECT(width - btnWidth * 2, 1, width - btnWidth + 1, btnbottom);
            var minBtnRect = new RECT(width - ControlBoxBounds.Width, 1, width - btnWidth * 2 + 1, btnbottom);
            var isMinimizedChild = IsMdiChild && WindowState == FormWindowState.Minimized;
            switch (GetMsstylePlatform())
            {
                case Platform.Vista:
                case Platform.Win7:
                case Platform.Win8:
                case Platform.Win81:
                    closeRect = new Rectangle(width - BorderWidth - 45, 1, 45, btnbottom);
                    maxBtnRect = new RECT(width - BorderWidth - closeRect.Width - 25, 1, width - BorderWidth - closeRect.Width + 1, btnbottom);
                    minBtnRect = new RECT(width - BorderWidth - closeRect.Width - 50, 1, width - BorderWidth - closeRect.Width - 25 + 1, btnbottom);
                    break;
                case Platform.Win10:
                case Platform.Win11:
                    closeRect = new Rectangle(width - BorderWidth - 45, 1, 45, btnbottom);
                    maxBtnRect = new RECT(width - btnWidth * 2, 1, width - btnWidth + 1, btnbottom);
                    minBtnRect = new RECT(width - ControlBoxBounds.Width, 1, width - btnWidth * 2 + 1, btnbottom);
                    break;
            }
            if (isMinimizedChild)
            {
                closeRect = new Rectangle(width - BorderWidth - 35, 1, 45, btnbottom);
                maxBtnRect = new RECT(width - 35 - closeRect.Width, 1, width - closeRect.Width + 1, 35);
                minBtnRect = new RECT(width - 35 - closeRect.Width * 2, 1, width + 2 - closeRect.Width * 2, 35);
            }
            var resMaxBtn = WindowState == FormWindowState.Maximized
                    ? CaptionButton.Restore
                    : CaptionButton.Maximize;
            var minresBtn = isMinimizedChild
                    ? CaptionButton.Restore
                    : CaptionButton.Minimize;
            var disableState = IsActive
                    ? DwmButtonState.Normal
                    : DwmButtonState.Disabled;
            switch (_captionButton)
            {
                case CaptionButton.Close:
                    nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, _buttonState, IsActive);
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, disableState, IsActive);
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, disableState, IsActive);
                    break;
                case CaptionButton.Minimize:
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, _buttonState, IsActive);
                    nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, disableState, IsActive);
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, disableState, IsActive);
                    break;
                case CaptionButton.Maximize:
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, _buttonState, IsActive);
                    nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, disableState, IsActive);
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, disableState, IsActive);
                    break;
                case CaptionButton.Restore:
                    break;
                case CaptionButton.Help:
                    break;

                case null:
                    nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, disableState, IsActive);
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, disableState, IsActive);
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, disableState, IsActive);


                    break;
                default:
                    throw new IndexOutOfRangeException();
            }


        }
        public void SetWindowRegion(IntPtr handle, int borderRadius = 0)
        {
            if (AllowNcTransparency) return;
            RECT rWindow;
            GetWindowRect(handle, out rWindow);
            OffsetRect(ref rWindow, -rWindow.left, -rWindow.top);
            if (RightToLeftLayout)
            {
                OffsetRect(ref rWindow, -BorderWidth * 2 + 1, -rWindow.top);
            }
            var p = RoundedRect(rWindow.ToRectangle(), borderRadius);
            if (!AllowNcTransparency | !DesignMode)
            {
                Region = new Region(p);
            }
            else
            {
                var hrgn = CreateRoundRectRgn(rWindow.left, rWindow.top, rWindow.right, rWindow.bottom, borderRadius,
                    borderRadius);
                SetWindowRgn(handle, hrgn, IsWindowVisible(handle));

            }

        }
        #endregion
    }

}
