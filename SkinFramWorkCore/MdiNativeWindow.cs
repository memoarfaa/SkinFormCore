using System.Windows.Forms;
using static Interop;

namespace SkinFramWorkCore
{
    internal class MdiNativeWindow:NativeWindow
    {
        private MdiClient _mdiClient;
        public MdiNativeWindow(MdiClient mdiClient)
        {
            _mdiClient = mdiClient;
             AssignHandle(_mdiClient.Handle);
            _mdiClient.HandleDestroyed += delegate { ReleaseHandle(); };
        }

        protected override void WndProc(ref Message m)
        {
            switch ((WindowsMessages)m.Msg)
            {
                case WindowsMessages.ERASEBKGND:
                    base.WndProc(ref m);

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
