using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
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
        private NCHITTEST _downHitTest;
        private NCHITTEST _moveHitTest;
        #endregion

        #region Properties
        private  bool DrawRtl 
        {
            get
            {
                return RightToLeft == RightToLeft.Yes && RightToLeftLayout;
            }
        }
       
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
        private bool _isActive { get; set; }
        [DefaultValue(31)]
        [Description("Caption hight of nonclient area.")]
        [Category("Theme")]
        public int CaptionHight { get; set; } = 31;

        [DefaultValue(8)]
        [Description("Border Width around nonclient area.")]
        [Category("Theme")]
        public int BorderWidth { get; set; } = 8;

        [DefaultValue(8)]
        [Description("Border Width around nonclient area.")]
        [Category("Opacity of nonclient area Color")]
        public int NcOpacity { get; set; } = 255;
        [DefaultValue(true)]
        [Description("Allow nonclient area transparency.")]
        [Category("Theme")]
        public bool AllowNcTransparency { get; set; } = true;

        [Category("Theme")]
        [Description("Set nonclient area active caption color.")]
        [RefreshProperties(RefreshProperties.All)]
        public Color ActiveCaptionColor { get; set; } = activeCaption;

        [Category("Theme")]
        [Description("Set nonclient area inactive caption color.")]
        [RefreshProperties(RefreshProperties.All)]
        public Color InActiveCaptionColor { get; set; } = GetMsstylePlatform() == Platform.Win10 || GetMsstylePlatform() == Platform.Win11 ? Color.White : Color.FromArgb(235, 235, 235);
        [Category("Theme")]
        [Description("Set nonclient area round corners.")]
        [RefreshProperties(RefreshProperties.All)]
        public int BorderRadius { get; set; }
        
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

            set
            {
                activeCaption = value;
            }
        }
        #endregion

        #region Overrides 
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            MdiRtlBackgorund();

        }

        private void MdiRtlBackgorund()
        {
            var mdiClient = Controls.Cast<Control>().OfType<MdiClient>().FirstOrDefault();

            if (mdiClient != null && RightToLeftLayout)
            {
                mdiClient.Paint += Rtl_Paint;
            }
        }

        protected override void OnRightToLeftLayoutChanged(EventArgs e)
        {
            base.OnRightToLeftLayoutChanged(e);
            MdiRtlBackgorund();


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
                        _isActive = false;
                    else if (m.LParam == Handle)
                        _isActive = true;
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
                    _downHitTest = (NCHITTEST)m.WParam;
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
                    _downHitTest = 0;
                    _moveHitTest = 0;
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
        private static void UpdateStyle(IntPtr hwnd)
        {
            // remove the border style
            Int32 currentStyle = GetWindowLong(hwnd, GWLIndex.GWL_STYLE);
            if ((currentStyle & (int)(WindowStyles.WS_BORDER)) != 0)
            {
                currentStyle &= ~(int)(WindowStyles.WS_BORDER);
                SetWindowLong(hwnd, GWLIndex.GWL_STYLE, currentStyle);
                SetWindowPos(hwnd, (IntPtr)0, -1, -1, -1, -1,
                                      (int)(SWPFlags.SWP_NOZORDER | SWPFlags.SWP_NOSIZE | SWPFlags.SWP_NOMOVE |
                                             SWPFlags.SWP_FRAMECHANGED | SWPFlags.SWP_NOREDRAW | SWPFlags.SWP_NOACTIVATE));
            }
        }
        internal void PaintTransparentBackground(PaintEventArgs e, Rectangle rectangle, Region transparentRegion = null)
        {
            Graphics graphics = e.Graphics;
            Control parentInternal = Parent;
            if (parentInternal != null)
            {
                if (Application.RenderWithVisualStyles /*&& parentInternal.RenderTransparencyWithVisualStyles*/)
                {
                    GraphicsState gstate = null;
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
                    Rectangle clipRect = new Rectangle(rectangle.Left + Left, rectangle.Top + Top, rectangle.Width,
                        rectangle.Height);
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

        internal static void PaintBackColor(PaintEventArgs e, Rectangle rectangle, Color backColor)
        {
            Color color = backColor;
            if (color.A == byte.MaxValue)
            {
                using (Graphics windowsGraphics = e.Graphics)
                {
                    Color nearestColor = windowsGraphics.GetNearestColor(color);
                    using (Brush brush = new SolidBrush(nearestColor))
                        windowsGraphics.FillRectangle(brush, rectangle);
                }
            }
            else
            {
                if (color.A <= 0)
                    return;
                using (Brush brush = new SolidBrush(color))
                    e.Graphics.FillRectangle(brush, rectangle);
            }
        }

        internal static bool IsImageTransparent(Image backgroundImage)
        {
            return backgroundImage != null && (backgroundImage.Flags & 2) > 0;
        }
        internal void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor)
        {
            if (BackColor == Color.Transparent)
                PaintTransparentBackground(e, rectangle);
            var isMdiClient = Controls.Cast<Control>().OfType<MdiClient>().FirstOrDefault()!= null;
            if (BackgroundImage != null && !SystemInformation.HighContrast && !isMdiClient)
            {
                if (BackgroundImageLayout == ImageLayout.Tile && IsImageTransparent(BackgroundImage))
                    PaintTransparentBackground(e, rectangle);


                if (IsImageTransparent(BackgroundImage))
                    PaintBackColor(e, rectangle, backColor);
                e.Graphics.DrawBackgroundImage(BackgroundImage, backColor, BackgroundImageLayout, ClientRectangle,
                    rectangle, Point.Empty, RightToLeft);
            }
            else
                PaintBackColor(e, rectangle, backColor);
        }
        private void Rtl_Paint(object sender, PaintEventArgs e)
        {

            if (BackColor == Color.Transparent)
                PaintTransparentBackground(e, e.ClipRectangle);
            if (BackgroundImage != null && !SystemInformation.HighContrast)
            {
                if (BackgroundImageLayout == ImageLayout.Tile && IsImageTransparent(BackgroundImage))
                    PaintTransparentBackground(e, ClientRectangle);
                e.Graphics.DrawBackgroundImage(BackgroundImage, BackColor, BackgroundImageLayout, ClientRectangle,
                    ClientRectangle, Point.Empty, RightToLeft);
            }
            else
                PaintBackColor(e, e.ClipRectangle, BackColor);
            
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
            _moveHitTest = (NCHITTEST)m.WParam;
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
            if (AllowNcTransparency)
            {
                MARGINS win10Margins = new MARGINS(8, 8, 31, 8);
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
            _isActive = m.WParam.ToInt32() > 0;
            OnWmNcPaint(ref m);
        }
        private void OnWmNcPaint(ref Message m)
        {
            IntPtr hdc = GetWindowDC(m.HWnd);
            RECT winRect;
            GetWindowRect(m.HWnd, out winRect);
            OffsetRect(ref winRect, -winRect.left, -winRect.top);
            int width = winRect.right - winRect.left;
            RECT clientRect;
            GetClientRect(m.HWnd, out clientRect);

            OffsetRect(ref clientRect, BorderWidth, CaptionHight);
            if(m.Msg == (int)WindowsMessages.NCPAINT)
            {
                if (!(BorderRadius > 0 && !AllowNcTransparency))
                {
                    SetWindowRgn(m.HWnd, IntPtr.Zero, false);
                }
            }
            if (!(IsMdiChild && WindowState == FormWindowState.Minimized))
                ExcludeClipRect(hdc, clientRect.left, clientRect.top, clientRect.right, clientRect.bottom);
            var paintParams = new BP_PAINTPARAMS(BPPF.NoClip | BPPF.Erase | BPPF.NonClient);
            if (IsMdiChild && AllowNcTransparency)
            {
                paintParams.BlendFunction = new BP_PAINTPARAMS.BLENDFUNCTION(0, 0, 255, 1);
            }
            IntPtr memdc;
            BufferedPaintInit();
            var hbuff = BeginBufferedPaint(hdc, ref winRect, BP_BUFFERFORMAT.DIB, ref paintParams, out memdc);
            var color = _isActive ? ActiveCaptionColor : InActiveCaptionColor;
            var ncColor = Color.FromArgb(NcOpacity, color);
            using (var nCGraphics = Graphics.FromHdc(memdc))
            {
                if (BorderRadius > 0)
                {
                    nCGraphics.Clip = new Region(RoundedRect(winRect.ToRectangle(), BorderRadius));
                }
                nCGraphics.Clear(ncColor);

                if (ShowIcon)
                {
                    nCGraphics.DrawIcon(new Icon(Icon, SystemInformation.SmallIconSize), 9, 7);
                }
                if (ControlBox)
                {
                    DrawCaptionButtons(width, nCGraphics);
                }
                if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                {
                    nCGraphics.Transform = new Matrix(-1, 0, 0, 1, width, 0);
                }
                
                if (Text != null)
                {
                    var stringFormat = new StringFormat(StringFormatFlags.NoWrap)
                    {
                        Trimming = StringTrimming.EllipsisWord,

                    };
                    if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                    {
                        nCGraphics.Transform = new Matrix(-1, 0, 0, 1, width - 17, 0);
                        stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                    }
                    var i = RightToLeftLayout && RightToLeft == RightToLeft.Yes ? winRect.Width - 17 - 31 : 31;
                    nCGraphics.DrawString(Text, SystemFonts.CaptionFont, _isActive ? Brushes.White : Brushes.Black, i,10,
                        stringFormat);
                   
                }
            }
            
            EndBufferedPaint(hbuff, true);
            BufferedPaintUnInit();
            ReleaseDC(Handle, hdc);
        }

        public static void Composited(IntPtr hwnd)
        {

            int extendedStyle = GetWindowLong(hwnd, GWLIndex.GWL_EXSTYLE);
            var dwNewLong = extendedStyle | (int)WindowStyles.WS_EX_COMPOSITED;
            SetWindowLong(hwnd, GWLIndex.GWL_EXSTYLE, dwNewLong);
        }
        private void DrawTop(ref Message m)
        {

            IntPtr hdc = GetWindowDC(m.HWnd);
            RECT rect;
            GetWindowRect(m.HWnd, out rect);
            OffsetRect(ref rect, -rect.left, -rect.top);
            int width = rect.right - rect.left;

            var paintParams = new BP_PAINTPARAMS(BPPF.NoClip | BPPF.Erase | BPPF.NonClient);
            if (IsMdiChild && AllowNcTransparency)
            {
                paintParams.BlendFunction = new BP_PAINTPARAMS.BLENDFUNCTION(0, 0, 255, 1);
            }
            IntPtr memdc;
            RECT bufferRECT = new RECT(0, 0, width, CaptionHight);
            var hbuff = BeginBufferedPaint(hdc, ref bufferRECT, BP_BUFFERFORMAT.DIB, ref paintParams, out memdc);
            var color = _isActive ? ActiveCaptionColor : InActiveCaptionColor;
            int btnWidth = ControlBoxBounds.Width / 3;
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
                    btnWidth = 25;
                    closeRect = new Rectangle(width - BorderWidth - 45, 1, 45, btnbottom);
                    maxBtnRect = new RECT(width - BorderWidth - closeRect.Width - btnWidth, 1, width - BorderWidth - closeRect.Width + 1, btnbottom);
                    minBtnRect = new RECT(width - BorderWidth - closeRect.Width - btnWidth * 2, 1, width - BorderWidth - closeRect.Width - btnWidth + 1, btnbottom);
                    break;
                case Platform.Win10:
                case Platform.Win11:
                    closeRect = new Rectangle(width - BorderWidth - 45, 1, 45, btnbottom);
                    maxBtnRect = new RECT(width - btnWidth * 2, 1, width - btnWidth + 1, btnbottom);
                    minBtnRect = new RECT(width - ControlBoxBounds.Width, 1, width - btnWidth * 2 + 1, btnbottom);
                    break;
            }
            var ncColor = Color.FromArgb(NcOpacity, color);
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
            var disableState = _isActive
                ? DwmButtonState.Normal
                : DwmButtonState.Disabled;
            using (var nCGraphics = Graphics.FromHdc(memdc))
            {
                nCGraphics.Clear(ncColor);

                if (ControlBox)
                {

                    if (_downHitTest == NCHITTEST.HTCLOSE) DrawCloseButton(nCGraphics, closeRect, (int)DwmButtonState.Pressed, _isActive);
                    else if (_moveHitTest == NCHITTEST.HTCLOSE)
                    {
                        if (_downHitTest == NCHITTEST.HTCLOSE) DrawCloseButton(nCGraphics, closeRect, (int)DwmButtonState.Pressed, _isActive);
                        else DrawCloseButton(nCGraphics, closeRect, (int)DwmButtonState.Hot, _isActive);
                    }
                    else DrawCloseButton(nCGraphics, closeRect, (int)DwmButtonState.Normal, _isActive);
                    if (MinimizeBox)
                    {
                        if (_downHitTest == NCHITTEST.HTMINBUTTON) DrawMinimizeButton(nCGraphics, minBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, _isActive);
                        else if (_moveHitTest == NCHITTEST.HTMINBUTTON)
                        {
                            if (_downHitTest == NCHITTEST.HTMINBUTTON) DrawMinimizeButton(nCGraphics, minBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, _isActive);
                            else DrawMinimizeButton(nCGraphics, minBtnRect.ToRectangle(), (int)DwmButtonState.Hot, _isActive);
                        }
                        else DrawMinimizeButton(nCGraphics, minBtnRect.ToRectangle(), (int)DwmButtonState.Normal, _isActive);
                    }
                    if (MaximizeBox)
                    {
                        if (WindowState == FormWindowState.Maximized)
                        {
                            if (_downHitTest == NCHITTEST.HTMAXBUTTON) DrawRestorButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, _isActive);
                            else if (_moveHitTest == NCHITTEST.HTMAXBUTTON)
                            {
                                if (_downHitTest == NCHITTEST.HTMAXBUTTON) DrawRestorButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, _isActive);
                                else DrawRestorButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Hot, _isActive);
                            }
                            else DrawRestorButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Normal, _isActive);
                        }
                        else
                        {
                            if (_downHitTest == NCHITTEST.HTMAXBUTTON) DrawMaximizeButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, _isActive);
                            else if (_moveHitTest == NCHITTEST.HTMAXBUTTON)
                            {
                                if (_downHitTest == NCHITTEST.HTMAXBUTTON) DrawMaximizeButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, _isActive);
                                else DrawMaximizeButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Hot, _isActive);
                            }
                            else DrawMaximizeButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Normal, _isActive);
                        }
                    }
                }

                if (Icon != null && ShowIcon)
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
                    nCGraphics.DrawString(Text, SystemFonts.CaptionFont, _isActive ? Brushes.White : Brushes.Black, i, 10,
                        stringFormat);
                }
            }

          
            EndBufferedPaint(hbuff, true);

            ReleaseDC(Handle, hdc);
        }

        private void OnWmNcCalcSize(ref Message m)
        {
            NCCALCSIZE_PARAMS ncc = new NCCALCSIZE_PARAMS();


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
            var disableState = _isActive
                    ? DwmButtonState.Normal
                    : DwmButtonState.Disabled;
                switch (_captionButton)
                {
                    case CaptionButton.Close:
                        nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, _buttonState, _isActive);
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, disableState, _isActive);
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, disableState, _isActive);
                        break;
                    case CaptionButton.Minimize:
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, _buttonState, _isActive);
                        nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, disableState, _isActive);
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, disableState, _isActive);
                        break;
                    case CaptionButton.Maximize:
                     if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, _buttonState, _isActive);
                        nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, disableState, _isActive);
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, disableState, _isActive);
                        break;
                    case CaptionButton.Restore:
                        break;
                    case CaptionButton.Help:
                        break;

                    case null:
                        nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, disableState, _isActive);
                    if (this.IsDrawMinimizeBox())
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, disableState, _isActive);
                    if (this.IsDrawMaximizeBox())
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, disableState, _isActive);


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
                OffsetRect(ref rWindow, -BorderWidth*2+1, -rWindow.top);
            }
            var p =  RoundedRect(rWindow.ToRectangle(), borderRadius);
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
