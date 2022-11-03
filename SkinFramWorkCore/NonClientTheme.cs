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
        private bool m_bMouseTracking;
        private const uint TME_LEAVE = 0x00000002;
        private TrackmouseEvent tme = TrackmouseEvent.Empty;
        private DwmButtonState _buttonState = DwmButtonState.Normal;
        private CaptionButton? _captionButton = null;
        private static bool isColorEnable = (int)
                           Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorPrevalence", 0) == 1;
        private NCHITTEST _downHitTest;
        private NCHITTEST _moveHitTest;
        #endregion

        #region Properties
        private bool DrawRtl => RightToLeftLayout && RightToLeft == RightToLeft.Yes;
        private static RECT _captionButtonBounds
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
        private RECT CaptionButtonBounds { get; set; } = _captionButtonBounds;
        private bool IsActive { get; set; }
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
        public Color ActiveCaptionColor { get; set; } = ActiveCaption;

        [Category("Theme")]
        [Description("Set nonclient area inactive caption color.")]
        public Color InActiveCaptionColor { get; set; } = GetMsstylePlatform() == Platform.Win10 || GetMsstylePlatform() == Platform.Win11 ? Color.White : Color.FromArgb(235, 235, 235);

        private static Color ActiveCaption
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
                        return isColorEnable ? Color.FromArgb(r, g, b) : Color.White;
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

            Controls.Cast<Control>().ToList().ForEach(control =>
            {
                var mdiClient = control as MdiClient;
                if (mdiClient != null && DrawRtl)
                {
                    mdiClient.Paint += Rtl_Paint;
                }

            });
            
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
                    OnWmNcMouseMove(m.HWnd);
                    _moveHitTest = (NCHITTEST)m.WParam;
                    switch ((NCHITTEST)m.WParam)
                    {
                        case NCHITTEST.HTMINBUTTON:

                            _captionButton = CaptionButton.Minimize;
                            _buttonState = DwmButtonState.Hot;
                            DrawTop(ref m);
                            break;
                        case NCHITTEST.HTMAXBUTTON:
                            _captionButton = CaptionButton.Maximize;
                            _buttonState = DwmButtonState.Hot;
                            DrawTop(ref m);
                            break;

                        case NCHITTEST.HTCLOSE:
                            _buttonState = DwmButtonState.Hot;
                            _captionButton = CaptionButton.Close;
                            DrawTop(ref m);
                            break;
                        default:
                            _captionButton = null;
                            base.WndProc(ref m);
                            OnWmNcPaint(ref m);
                            break;

                    }
                    break;
                case WindowsMessages.NCLBUTTONDOWN:
                    IntPtr hdc = GetWindowDC(m.HWnd);
                    RECT rect;
                    GetWindowRect(m.HWnd, out rect);
                    OffsetRect(ref rect, -rect.left, -rect.top);
                    _downHitTest = (NCHITTEST)m.WParam;
                    switch ((NCHITTEST)m.WParam)
                    {

                        case NCHITTEST.HTMINBUTTON:
                            _captionButton = CaptionButton.Minimize;
                            _buttonState = DwmButtonState.Pressed;
                            DrawTop(ref m);
                            break;
                        case NCHITTEST.HTMAXBUTTON:
                            _captionButton = CaptionButton.Maximize;
                            _buttonState = DwmButtonState.Pressed;
                            DrawTop(ref m);
                            break;

                        case NCHITTEST.HTCLOSE:
                            _captionButton = CaptionButton.Close;
                            _buttonState = DwmButtonState.Pressed;
                            DrawTop(ref m);
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
                    DrawTop(ref m);
                    break;
                case WindowsMessages.NCUAHDRAWCAPTION:
                case WindowsMessages.NCUAHDRAWFRAME:
                    _captionButton = null;
                    DrawTop(ref m);
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
        //TODO: DrawImage with Image Layouts
        private void Rtl_Paint(object sender, PaintEventArgs e)
        {
            if (BackgroundImage != null)
            {
                e.Graphics.DrawImage(BackgroundImage, ClientRectangle);
            }
        }
        private void ResetNcTracking(IntPtr hwnd)
        {
            m_bMouseTracking = false;
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
            var btnWidth = CaptionButtonBounds.Width / 3;
            var btnbottom = CaptionButtonBounds.Height;
            var closeRect = new RECT(width - 45 - BorderWidth, 1, width - BorderWidth, btnbottom);
            var restoreRect = new RECT(width - btnWidth * 2, 1, width - btnWidth + 1, btnbottom);
            var minRect = new RECT(width - CaptionButtonBounds.Width, 1, width - btnWidth * 2 + 1, btnbottom);


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
        private void OnWmNcMouseMove(IntPtr hwnd)
        {
            if (m_bMouseTracking) return;
            tme.cbSize = (uint)Marshal.SizeOf(tme);
            tme.hwndTrack = hwnd;
            tme.dwFlags = TME_HOVER | TME_LEAVE | TME_NONCLIENT;
            tme.dwHoverTime = 1/*HOVER_DEFAULT*/;
            TrackMouseEvent(ref tme);
            m_bMouseTracking = true;
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
            IsActive = m.WParam.ToInt32() > 0;
            OnWmNcPaint(ref m);
        }
        private void OnWmNcPaint(ref Message m)
        {
            IntPtr hdc = GetWindowDC(m.HWnd);
            RECT rect;
            GetWindowRect(m.HWnd, out rect);
            OffsetRect(ref rect, -rect.left, -rect.top);
            int width = rect.right - rect.left;
            RECT clientRect;
            GetClientRect(m.HWnd, out clientRect);

            OffsetRect(ref clientRect, BorderWidth, CaptionHight);
            SetWindowRgn(m.HWnd, IntPtr.Zero, false);
            if (!(IsMdiChild && WindowState == FormWindowState.Minimized))
                ExcludeClipRect(hdc, clientRect.left, clientRect.top, clientRect.right, clientRect.bottom);
            var paintParams = new BP_PAINTPARAMS(BPPF.NoClip | BPPF.Erase | BPPF.NonClient);
            if (IsMdiChild && AllowNcTransparency)
            {
                paintParams.BlendFunction = new BP_PAINTPARAMS.BLENDFUNCTION(0, 0, 255, 1);
            }
            IntPtr memdc;
            var hbuff = BeginBufferedPaint(hdc, ref rect, BP_BUFFERFORMAT.DIB, ref paintParams, out memdc);
            var color = IsActive ? ActiveCaptionColor : InActiveCaptionColor;
            var ncColor = Color.FromArgb(NcOpacity, color);
            using (var nCGraphics = Graphics.FromHdc(memdc))
            {
                nCGraphics.Clear(ncColor);

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
            if (ControlBox)
            {
                DrawCaptionButtons(width, memdc);
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
            var color = IsActive ? ActiveCaptionColor : InActiveCaptionColor;
            int btnWidth = CaptionButtonBounds.Width / 3;
            var btnbottom = CaptionButtonBounds.Height;
            var closeRect = new Rectangle(width - BorderWidth - 45, 1, 45, btnbottom);
            var maxBtnRect = new RECT(width - btnWidth * 2, 1, width - btnWidth + 1, btnbottom);
            var minBtnRect = new RECT(width - CaptionButtonBounds.Width, 1, width - btnWidth * 2 + 1, btnbottom);
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
                    minBtnRect = new RECT(width - CaptionButtonBounds.Width, 1, width - btnWidth * 2 + 1, btnbottom);
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
            var disableState = IsActive
                ? DwmButtonState.Normal
                : DwmButtonState.Disabled;
            using (var nCGraphics = Graphics.FromHdc(memdc))
            {
                nCGraphics.Clear(ncColor);

                if (ControlBox)
                {

                    if (_downHitTest == NCHITTEST.HTCLOSE) Extensions.DrawCloseButton(nCGraphics, closeRect, (int)DwmButtonState.Pressed, IsActive);
                    else if (_moveHitTest == NCHITTEST.HTCLOSE)
                    {
                        if (_downHitTest == NCHITTEST.HTCLOSE) Extensions.DrawCloseButton(nCGraphics, closeRect, (int)DwmButtonState.Pressed, IsActive);
                        else DrawCloseButton(nCGraphics, closeRect, (int)DwmButtonState.Hot, IsActive);
                    }
                    else DrawCloseButton(nCGraphics, closeRect, (int)DwmButtonState.Normal, IsActive);
                    if (MinimizeBox)
                    {
                        if (_downHitTest == NCHITTEST.HTMINBUTTON) Extensions.DrawMinimizeButton(nCGraphics, minBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, IsActive);
                        else if (_moveHitTest == NCHITTEST.HTMINBUTTON)
                        {
                            if (_downHitTest == NCHITTEST.HTMINBUTTON) Extensions.DrawMinimizeButton(nCGraphics, minBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, IsActive);
                            else DrawMinimizeButton(nCGraphics, minBtnRect.ToRectangle(), (int)DwmButtonState.Hot, IsActive);
                        }
                        else DrawMinimizeButton(nCGraphics, minBtnRect.ToRectangle(), (int)DwmButtonState.Normal, IsActive);
                    }
                    if (MaximizeBox)
                    {
                        if (WindowState == FormWindowState.Maximized)
                        {
                            if (_downHitTest == NCHITTEST.HTMAXBUTTON) Extensions.DrawRestorButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, IsActive);
                            else if (_moveHitTest == NCHITTEST.HTMAXBUTTON)
                            {
                                if (_downHitTest == NCHITTEST.HTMAXBUTTON) Extensions.DrawRestorButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, IsActive);
                                else DrawRestorButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Hot, IsActive);
                            }
                            else DrawRestorButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Normal, IsActive);
                        }
                        else
                        {
                            if (_downHitTest == NCHITTEST.HTMAXBUTTON) Extensions.DrawMaximizeButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, IsActive);
                            else if (_moveHitTest == NCHITTEST.HTMAXBUTTON)
                            {
                                if (_downHitTest == NCHITTEST.HTMAXBUTTON) Extensions.DrawMaximizeButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Pressed, IsActive);
                                else DrawMaximizeButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Hot, IsActive);
                            }
                            else DrawMaximizeButton(nCGraphics, maxBtnRect.ToRectangle(), (int)DwmButtonState.Normal, IsActive);
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
                    nCGraphics.DrawString(Text, SystemFonts.CaptionFont, IsActive ? Brushes.White : Brushes.Black, i, 10,
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

        private void DrawCaptionButtons(int width, IntPtr hdc)
        {
            using (Graphics nCGraphics = Graphics.FromHdc(hdc))
            {

                var btnWidth = CaptionButtonBounds.Width / 3;
                var btnbottom = CaptionButtonBounds.Height;
                var closeRect = new Rectangle(width - BorderWidth - 45, 1, 45, btnbottom);
                var maxBtnRect = new RECT(width - btnWidth * 2, 1, width - btnWidth + 1, btnbottom);
                var minBtnRect = new RECT(width - CaptionButtonBounds.Width, 1, width - btnWidth * 2 + 1, btnbottom);
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
                        minBtnRect = new RECT(width - CaptionButtonBounds.Width, 1, width - btnWidth * 2 + 1, btnbottom);
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
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, disableState, IsActive);
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, disableState, IsActive);
                        break;
                    case CaptionButton.Minimize:
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, _buttonState, IsActive);
                        nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, disableState, IsActive);
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, disableState, IsActive);
                        break;
                    case CaptionButton.Maximize:
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, _buttonState, IsActive);
                        nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, disableState, IsActive);
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, disableState, IsActive);
                        break;
                    case CaptionButton.Restore:
                        break;
                    case CaptionButton.Help:
                        break;

                    case null:
                        nCGraphics.DrawCaptionButton(closeRect, CaptionButton.Close, disableState, IsActive);
                        nCGraphics.DrawCaptionButton(minBtnRect.ToRectangle(), minresBtn, disableState, IsActive);
                        nCGraphics.DrawCaptionButton(maxBtnRect.ToRectangle(), resMaxBtn, disableState, IsActive);


                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }

            }
        }

        #endregion
    }

}
