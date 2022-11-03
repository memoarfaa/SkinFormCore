using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SkinFramWorkCore;
namespace TestApp
{
    public partial class MDIParent1 : SkinForm
    {
        private int childFormNumber = 0;

        public MDIParent1()
        {
            InitializeComponent();
            if (GetDeskTopWallpaper() != null)
            {
                BackgroundImageLayout = ImageLayout.Stretch;
                BackgroundImage = GetDeskTopWallpaper();
            }
        }
        private Image GetDeskTopWallpaper()
        {
            int SPI_GETDESKWALLPAPER = 0x73;
            int MAX_PATH = 260;
            string wallpaper = new string('\0', (int)MAX_PATH);
            NativeMethods.SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, wallpaper, 0);

            wallpaper = wallpaper.Substring(0, wallpaper.IndexOf('\0'));
            return !string.IsNullOrEmpty(wallpaper) ? new Bitmap(Image.FromFile(wallpaper)) : null;
        }
        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new SkinForm() { BackgroundImage = BackgroundImage,BackgroundImageLayout= BackgroundImageLayout,RightToLeft= RightToLeft,RightToLeftLayout= RightToLeftLayout,NcOpacity = NcOpacity,AllowNcTransparency = AllowNcTransparency};
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            if (!RightToLeftLayout)
            {
                this.RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }
            else
            {
                this.RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
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

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }
    }
}
