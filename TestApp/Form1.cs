using SkinFramWorkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : SkinForm
    {
        public Form1()
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

    }
}
