using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static SkinFramWorkCore.NativeMethods;
namespace SkinFramWorkCore
{
    public static class Extensions
    {

        private static IntPtr hTheme = OpenThemeData(GetDesktopWindow(), "DWMWINDOW");
        public static Image GetImageAtlasFromTheme()
        {
            var path = (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ThemeManager",
                         "DllName",
                         string.Empty);

            var hInstance = NativeMethods.LoadLibraryEx(path, IntPtr.Zero, NativeMethods.LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
            try
            {

                IntPtr themeStream;
                uint streamSize;

                GetThemeStream(hTheme, 0, 0, 213, out themeStream, out streamSize, hInstance);
                var bufferStream = new byte[streamSize];
                Marshal.Copy(themeStream, bufferStream, 0, bufferStream.Length);


                using (var ms = new MemoryStream(bufferStream))
                {
                    return new Bitmap(Image.FromStream(ms));
                }

            }
            catch (ArgumentNullException)
            {
                //ignore
            }
            catch (NullReferenceException)
            {
                //ignore
            }
            catch (OutOfMemoryException)
            {
                //ignore
            }

            finally
            {
                FreeLibrary(hInstance);
            }
            return null;
        }
        public static Image Slice(this Bitmap original, Point loc, Size size)
        {
            return original.Clone(new Rectangle(loc, size), original.PixelFormat);
        }
        public static Image GetDwmWindowButton(int button, int state)
        {
            try
            {
                const int tmtAtlasrect = 8002;
                const int tmtImagecount = 2401;
                var hWnd = GetDesktopWindow();

                var hTheme = OpenThemeData(hWnd, "DWMWINDOW");


                var atlas = GetImageAtlasFromTheme();

                if (atlas == null) return null;
                RECT rect;
                GetThemeRect(hTheme, (int)button, (int)state, tmtAtlasrect, out rect);

                var result = Slice((Bitmap)atlas, rect.Location, rect.Size);
                if (state == 0) return result;
                int count;
                GetThemeInt(hTheme, (int)button, (int)state, tmtImagecount, out count);
                var buttonSize = rect.Height / count;
                var startPoint = Point.Empty;
                var btnSize = new Size(rect.Width, buttonSize);
                startPoint.Offset(0, buttonSize * ((int)state - 1));
                var buttonRect = new Rectangle(startPoint, btnSize);
                buttonRect.Inflate(-1, -1);
                result = Slice((Bitmap)result, buttonRect.Location, buttonRect.Size);
                return result;
            }
            catch
            {
                // ignored
                return new Bitmap(27, 27);
            }

        }
        public static Platform GetMsstylePlatform()
        {
            var currentMsstylePath = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ThemeManager", "DllName", string.Empty).ToString();
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(currentMsstylePath);
            var fileVersion = fileVersionInfo.FileVersion;
            DebugConsole.WriteLine(fileVersion);
            var versionToken = fileVersion.Split('.');
            if (versionToken.Count() > 0)
            {
                switch (versionToken[0])
                {
                    case "6":
                        switch (versionToken[1])
                        {
                            case "0":
                                return Platform.Vista;
                            case "1":
                                return Platform.Win7;
                            case "2":
                                return Platform.Win8;
                            case "3":
                                return Platform.Win81;
                        }
                        break;
                    case "10":
                        if (int.Parse(versionToken[2]) <= 22000)
                        {
                            return Platform.Win10;
                        }
                        else
                        {
                            return Platform.Win11;
                        }
                    default:
                        return Platform.Win10;

                }
            }
            return Platform.Win10;
        }
        private static DwmWindowCaption WindowCaption
        {
            get
            {
                switch (GetMsstylePlatform())
                {
                    case Platform.Win81:
                    case Platform.Win8:
                        return new Windows8Caption();

                    case Platform.Win10:
                        return new Windows10Caption();
                    default:
                        return new Windows8Caption();
                }
            }
        }

        public static void DrawCaptionButton(this Graphics graphics, Rectangle rect, CaptionButton captionButton, DwmButtonState state, bool active)
        {

            switch (captionButton)
            {

                case CaptionButton.Close:
                    DrawCloseButton(graphics, rect, (int)state, active);
                    break;
                case CaptionButton.Minimize:
                    DrawMinimizeButton(graphics, rect, (int)state, active);
                    break;
                case CaptionButton.Maximize:
                    DrawMaximizeButton(graphics, rect, (int)state, active);
                    break;
                case CaptionButton.Restore:
                    DrawRestorButton(graphics, rect, (int)state, active);
                    break;
                case CaptionButton.Help:
                    break;
            }
        }

        public static void DrawCloseButton(Graphics graphics, Rectangle rect, int state, bool active)
        {


            var BackgrounImage = GetDwmWindowButton(active ? WindowCaption.BUTTONACTIVECLOS : WindowCaption.BUTTONAINCTIVECLOS, (int)state);

            var Image = GetDwmWindowButton(WindowCaption.BUTTONCLOSEGLYPH96, active ? (int)state : (int)DwmButtonState.Disabled);
            if (BackgrounImage == null || Image == null) return;
            graphics.DrawImage(BackgrounImage, rect);
            var bounRect = new Rectangle((rect.Width - Image.Width) / 2, (rect.Height - Image.Height) / 2, Image.Width, Image.Height);
            bounRect.Offset(rect.Location);
            graphics.DrawImage(Image, bounRect);
        }
        public static void DrawMinimizeButton(Graphics graphics, Rectangle rect, int state, bool active)
        {


            var BackgrounImage = GetDwmWindowButton(active ? WindowCaption.BUTTONACTIVECAPTION : WindowCaption.BUTTONINACTIVECAPTION, (int)state);
            var Image = GetDwmWindowButton(WindowCaption.BUTTONMINGLYPH96, active ? (int)state : (int)DwmButtonState.Disabled);
            if (BackgrounImage == null || Image == null) return;
            graphics.DrawImage(BackgrounImage, rect);
            var bounRect = new Rectangle((rect.Width - Image.Width) / 2, (rect.Height - Image.Height) / 2, Image.Width, Image.Height);
            bounRect.Offset(rect.Location);
            graphics.DrawImage(Image, bounRect);
        }
        public static void DrawMaximizeButton(Graphics graphics, Rectangle rect, int state, bool active)
        {


            var BackgrounImage = GetDwmWindowButton(active ? WindowCaption.BUTTONACTIVECAPTION : WindowCaption.BUTTONINACTIVECAPTION, (int)state);
            var Image = GetDwmWindowButton(WindowCaption.BUTTONMAXGLYPH96, active ? (int)state : (int)DwmButtonState.Disabled);
            if (BackgrounImage == null || Image == null) return;
            graphics.DrawImage(BackgrounImage, rect);
            var bounRect = new Rectangle((rect.Width - Image.Width) / 2, (rect.Height - Image.Height) / 2, Image.Width, Image.Height);
            bounRect.Offset(rect.Location);
            graphics.DrawImage(Image, bounRect);
        }
        public static void DrawRestorButton(Graphics graphics, Rectangle rect, int state, bool active)
        {


            var BackgrounImage = GetDwmWindowButton(active ? WindowCaption.BUTTONACTIVECAPTION : WindowCaption.BUTTONINACTIVECAPTION, (int)state);
            var Image = GetDwmWindowButton(WindowCaption.BUTTONRESTOREGLYPH96, active ? (int)state : (int)DwmButtonState.Disabled);
            if (BackgrounImage == null || Image == null) return;
            graphics.DrawImage(BackgrounImage, rect);
            var bounRect = new Rectangle((rect.Width - Image.Width) / 2, (rect.Height - Image.Height) / 2, Image.Width, Image.Height);
            bounRect.Offset(rect.Location);
            graphics.DrawImage(Image, bounRect);
        }


    }
}
