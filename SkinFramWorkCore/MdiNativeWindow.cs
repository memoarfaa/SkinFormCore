using System;
using System.Drawing;
using System.Windows.Forms;
using static Interop;
using static Interop.Gdi32;
using static Interop.User32;
namespace SkinFramWorkCore
{
    internal class MdiNativeWindow:NativeWindow
    {
        private MdiClient _mdiClient;
        
        public MdiNativeWindow(MdiClient mdiClient)
        {
            _mdiClient = mdiClient;
            ReleaseHandle();
             AssignHandle(_mdiClient.Handle);
            _mdiClient.HandleDestroyed += delegate { ReleaseHandle(); };
        }

        protected override void WndProc(ref Message m)
        {
            switch ((WindowsMessages)m.Msg)
            {
                case WindowsMessages.PAINT:
                    PAINTSTRUCT paintStruct = new PAINTSTRUCT();
                    var screenHdc = BeginPaint(m.HWnd, ref paintStruct);
                    UxTheme.BP_PAINTPARAMS paintParams = new UxTheme.BP_PAINTPARAMS(UxTheme.BPPF.Erase| UxTheme.BPPF.NoClip);
                    RECT clientRect = new RECT();
                    GetClientRect(_mdiClient.Handle, ref clientRect);
                    UxTheme.HPAINTBUFFER bufferedPaint = UxTheme.BeginBufferedPaint(screenHdc, ref clientRect, UxTheme.BP_BUFFERFORMAT.CompatibleBitmap, ref paintParams, out screenHdc);
                    using (Graphics g = Graphics.FromHdcInternal(screenHdc.Handle))
                    {
                       
                        if (_mdiClient.BackgroundImage != null)
                        {
                            g.DrawBackgroundImage(_mdiClient.BackgroundImage, _mdiClient.BackColor, _mdiClient.BackgroundImageLayout, clientRect, clientRect, Point.Empty,_mdiClient.RightToLeft);
                        }
                        else 
                            g.Clear(SystemColors.AppWorkspace);
                    }
                    UxTheme.EndBufferedPaint(bufferedPaint, true);
                   
                    EndPaint(m.HWnd, ref paintStruct);
                    break;
                case WindowsMessages.ERASEBKGND:
                   
                    if (((int)User32.GetWindowLong(_mdiClient.Handle, User32.GWLIndex.GWL_EXSTYLE) & (int)WindowStyles.WS_EX_COMPOSITED) == (int)WindowStyles.WS_EX_COMPOSITED)
                    {
                        _mdiClient.Invalidate();
                    }

                    break;
               
                default:
                    base.WndProc(ref m);
                    break;
            }
           
        }
    }
}
