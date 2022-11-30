using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkinFramWorkCore;
using TestApp1.Properties;

namespace TestApp1
{
    public partial class MDIParent1 : SkinForm
    {
        private int childFormNumber = 0;
        private TextBox txtBorders;
        private Button submitButton;
        private Button btnToggleMenuStrip;
        private Label labelBorders;
        private SkinForm frmSubmit;
        private SkinForm frmChild;

        public MDIParent1()
        {
            InitializeComponent();
            RightToLeft = Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            RightToLeftLayout = Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft;
            frmSubmit = new SkinForm { RightToLeft = RightToLeft, RightToLeftLayout = RightToLeftLayout, Text = "Window", BorderWidth = BorderWidth, CaptionHieght = CaptionHieght, BorderRadius = BorderRadius, NcOpacity = NcOpacity, StartPosition = FormStartPosition.CenterParent, AllowNcTransparency = AllowNcTransparency, Size = new Size(240, 200) };
            txtBorders = new TextBox();
            txtBorders.Location = new Point(50, 60);
            txtBorders.Size = new Size(120, 23);
            frmSubmit.Controls.Add(txtBorders);
            submitButton = new Button();
            submitButton.Location = new Point(50, 110);
            submitButton.Size = new Size(120, 30);
            submitButton.Text = "Submit";
            submitButton.UseVisualStyleBackColor = true;
            submitButton.Click += SubmitButton_Click;
            frmSubmit.Controls.Add(submitButton);
            labelBorders = new Label();
            labelBorders.AutoSize = true;
            labelBorders.Location = new Point(35, 25);
            labelBorders.Size = new Size(155, 15);
            frmSubmit.Controls.Add(labelBorders);
            frmChild = new SkinForm { MdiParent = this, RightToLeft = RightToLeft, RightToLeftLayout = RightToLeftLayout, Text = "frmChild", BorderWidth = BorderWidth, CaptionHieght = CaptionHieght, BorderRadius = BorderRadius, NcOpacity = NcOpacity, StartPosition = FormStartPosition.CenterScreen, AllowNcTransparency = AllowNcTransparency };
           
            btnToggleMenuStrip = new Button { Text = "Toggle MenuStrip", Size = new Size(160, 30) };
            btnToggleMenuStrip.Click += BtnToggleMenuStrip_Click;
            frmChild.Controls.Add(btnToggleMenuStrip);
            Load += MDIParent1_Load;
        }
       
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
       
        [Flags]
        internal enum SWPFlags
        {

            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
        }

        private void MDIParent1_Load(object? sender, EventArgs e)
        {
            frmChild.Show();
            if (GetDeskTopWallpaper != null)
            {
                BackgroundImage = GetDeskTopWallpaper();
                BackgroundImageLayout = ImageLayout.Stretch;
            }
        }
        private void InvalidateForm(Form form)
        {
            if (!form.IsDisposed && form.IsHandleCreated)
            {
                SetWindowPos(form.Handle, IntPtr.Zero, 0, 0, 0, 0,
                    (int)(SWPFlags.SWP_NOACTIVATE | SWPFlags.SWP_NOMOVE | SWPFlags.SWP_NOSIZE |
                    SWPFlags.SWP_NOZORDER | SWPFlags.SWP_FRAMECHANGED));
            }
        }

        private void BtnToggleMenuStrip_Click(object? sender, EventArgs e)
        {
            if (MainMenuStrip is null)
            {
                MainMenuStrip = menuStrip1;
                Controls.Add(menuStrip1);
            }

            else
            {
                Controls.Remove(menuStrip1);
                MainMenuStrip = null;
            }
        }

       
        private Image? GetDeskTopWallpaper()
        {
            int SPI_GETDESKWALLPAPER = 0x73;
            int MAX_PATH = 260;
            string wallpaper = new string('\0', (int)MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, wallpaper, 0);

            wallpaper = wallpaper.Substring(0, wallpaper.IndexOf('\0'));
            return !string.IsNullOrEmpty(wallpaper) ? new Bitmap(Image.FromFile(wallpaper)) : null;
        }
      
       
        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void rightToLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Settings settings = Settings.Default;
            if (settings.Culture  == "ar-EG") return;
            settings.Culture = "ar-EG";
            settings.Save();
            Application.Restart();
        }

        private void leftToRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = Settings.Default;
            if (settings.Culture == "en-US") return;
            settings.Culture = "en-US";
            settings.Save();
            Application.Restart();
        }

      
        private void borderWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            labelBorders.Text = "Insert Border Width";
            frmSubmit.ShowDialog(this);
        }

        private void newFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SkinForm form = new SkinForm { RightToLeft = RightToLeft, RightToLeftLayout = RightToLeftLayout, Text = "Form1", BorderWidth = BorderWidth, CaptionHieght = CaptionHieght, BorderRadius = BorderRadius, NcOpacity = NcOpacity, StartPosition = FormStartPosition.CenterParent, AllowNcTransparency = AllowNcTransparency, Size = new Size(1024, 768), BackgroundImage = BackgroundImage, BackgroundImageLayout = BackgroundImageLayout , ActiveCaptionColor = ActiveCaptionColor, InActiveCaptionColor = InActiveCaptionColor };

            WindowState = FormWindowState.Minimized;
            form.Closing += delegate { WindowState = FormWindowState.Maximized; };
            form.Show();
            form.Activate();
        }

        private void newChildFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SkinForm frmChild = new SkinForm { MdiParent = this, RightToLeft = RightToLeft, RightToLeftLayout = RightToLeftLayout, Text = "frmChild " + childFormNumber++, BorderWidth = BorderWidth, CaptionHieght = CaptionHieght, BorderRadius = BorderRadius, NcOpacity = NcOpacity, BackgroundImage = BackgroundImage, BackgroundImageLayout = BackgroundImageLayout, AllowNcTransparency = AllowNcTransparency,ActiveCaptionColor = ActiveCaptionColor,InActiveCaptionColor = InActiveCaptionColor };
            frmChild.Show();
        }

        private void imageLayoutTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GetDeskTopWallpaper != null)
            {
                BackgroundImage = GetDeskTopWallpaper();
                BackgroundImageLayout = ImageLayout.Tile;
            }

           
        }

        private void imageLayoutCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GetDeskTopWallpaper != null)
            {
                BackgroundImage = GetDeskTopWallpaper();
                BackgroundImageLayout = ImageLayout.Center;
            }

           
        }

        private void imageLayoutStretchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GetDeskTopWallpaper != null)
            {
                BackgroundImage = GetDeskTopWallpaper();
                BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private void imageLayoutZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GetDeskTopWallpaper != null)
            {
                BackgroundImage = GetDeskTopWallpaper();
                BackgroundImageLayout = ImageLayout.Zoom;
            }
        }

        private void removeBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundImage = null;
        }

        private void borderRadiusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            labelBorders.Text = "Insert Border Radius";
            frmSubmit.ShowDialog(this);
        }

        private void borderOpacityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            labelBorders.Text = "Insert Border Opacity";
            frmSubmit.ShowDialog(this);
        }
        private void CaptionHieghtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            labelBorders.Text = "Insert Caption Height";
            frmSubmit.ShowDialog(this);
        }
        private void SubmitButton_Click(object? sender, EventArgs e)
        {
            switch (labelBorders.Text)
            {
                case "Insert Border Width":
                    {
                        if (txtBorders.Text.Length > 0)
                        {
                            int borderWidth;
                            bool isnumber = int.TryParse(txtBorders.Text, out borderWidth);
                            if (isnumber && borderWidth > 0)
                            {
                                BorderWidth = frmChild.BorderWidth = frmSubmit.BorderWidth = borderWidth;
                                InvalidateForm(this);
                                InvalidateForm(frmChild);
                                InvalidateForm(frmSubmit);
                                MdiChildren.ToList().ForEach(child =>
                                {
                                    ((SkinForm)child).BorderWidth = borderWidth;
                                    InvalidateForm(child);
                                });
                            }
                        }
                    }

                    break;

                case "Insert Border Radius":
                    {
                        if (txtBorders.Text.Length > 0)
                        {
                            int borderRadius;
                            bool isnumber = int.TryParse(txtBorders.Text, out borderRadius);
                            if (isnumber && borderRadius > 0)
                            {
                                BorderRadius = frmChild.BorderRadius = frmSubmit.BorderRadius = borderRadius;
                                InvalidateForm(this);
                                InvalidateForm(frmChild);
                                InvalidateForm(frmSubmit);
                                MdiChildren.ToList().ForEach(child =>
                                {
                                    ((SkinForm)child).BorderRadius = borderRadius;
                                    InvalidateForm(child);
                                });
                            }
                        }
                    }

                    break;
                case "Insert Border Opacity":
                    {
                        if (txtBorders.Text.Length > 0)
                        {
                            int ncOpacity;
                            bool isnumber = int.TryParse(txtBorders.Text, out ncOpacity);
                            if (isnumber && ncOpacity > 0)
                            {
                                NcOpacity = frmChild.NcOpacity = frmSubmit.NcOpacity = ncOpacity;
                                InvalidateForm(this);
                                InvalidateForm(frmChild);
                                InvalidateForm(frmSubmit);
                                MdiChildren.ToList().ForEach(child =>
                                {
                                    ((SkinForm)child).NcOpacity = ncOpacity;
                                    InvalidateForm(child);
                                });
                            }
                        }
                    }

                    break;

                case "Insert Caption Height":
                    {
                        if (txtBorders.Text.Length > 0)
                        {
                            int captionHieght;
                            bool isnumber = int.TryParse(txtBorders.Text, out captionHieght);
                            if (isnumber && captionHieght > 0)
                            {
                                CaptionHieght = frmChild.CaptionHieght = frmSubmit.CaptionHieght = captionHieght;
                                InvalidateForm(this);
                                InvalidateForm(frmChild);
                                InvalidateForm(frmSubmit);
                                MdiChildren.ToList().ForEach(child =>
                                {
                                    ((SkinForm)child).CaptionHieght = captionHieght;
                                    InvalidateForm(child);
                                });
                            }
                        }
                    }

                    break;
            }

            frmSubmit.Close();
        }

        private void activeBorederColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                ActiveCaptionColor = frmChild.ActiveCaptionColor = frmSubmit.ActiveCaptionColor = colorDialog.Color;
                InvalidateForm(this);
                InvalidateForm(frmChild);
                InvalidateForm(frmSubmit);
                MdiChildren.ToList().ForEach(child =>
                {
                  
                    InvalidateForm(child);
                });
            }
        }

        private void inactiveBorederColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                InActiveCaptionColor = frmChild.InActiveCaptionColor = frmSubmit.InActiveCaptionColor = colorDialog.Color;
                InvalidateForm(this);
                InvalidateForm(frmChild);
                InvalidateForm(frmSubmit);
                MdiChildren.ToList().ForEach(child =>
                {

                    InvalidateForm(child);
                });
            }
        }
    }
}
