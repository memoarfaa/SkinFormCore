using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
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
            catch (ArgumentNullException ex)
            {
                //ignore
                DebugConsole.WriteLine(ex);
            }
            catch (NullReferenceException ex)
            {
                //ignore
                DebugConsole.WriteLine(ex);
            }
            catch (OutOfMemoryException ex)
            {
                //ignore
                DebugConsole.WriteLine(ex);
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
                        if (int.Parse(versionToken[2]) < 22000)
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

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            var size = new Size(diameter, diameter);
            var arc = new Rectangle(bounds.Location, size);
            var path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (var path = RoundedRect(bounds, cornerRadius))
            {
                // graphics.Clear(Color.Transparent);
                // graphics.FillRegion(brush,graphics.Clip);
                graphics.FillPath(brush, path);

            }
        }

        internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout)
        {
            var rectangle = bounds;
            if (backgroundImage != null)
            {
                switch (imageLayout)
                {
                    case ImageLayout.None:
                        rectangle.Size = backgroundImage.Size;
                        break;
                    case ImageLayout.Center:
                        rectangle.Size = backgroundImage.Size;
                        var size1 = bounds.Size;
                        if (size1.Width > rectangle.Width)
                            rectangle.X = (size1.Width - rectangle.Width) / 2;
                        if (size1.Height > rectangle.Height)
                        {
                            rectangle.Y = (size1.Height - rectangle.Height) / 2;
                        }
                        break;
                    case ImageLayout.Stretch:
                        rectangle.Size = bounds.Size;
                        break;
                    case ImageLayout.Zoom:
                        var size2 = backgroundImage.Size;
                        var num1 = bounds.Width / (float)size2.Width;
                        var num2 = bounds.Height / (float)size2.Height;
                        if (num1 < (double)num2)
                        {
                            rectangle.Width = bounds.Width;
                            rectangle.Height = (int)(size2.Height * (double)num1 + 0.5);
                            if (bounds.Y >= 0)
                            {
                                rectangle.Y = (bounds.Height - rectangle.Height) / 2;
                            }
                            break;
                        }
                        rectangle.Height = bounds.Height;
                        rectangle.Width = (int)(size2.Width * (double)num2 + 0.5);
                        if (bounds.X >= 0)
                        {
                            rectangle.X = (bounds.Width - rectangle.Width) / 2;
                        }
                        break;
                }
            }
            return rectangle;
        }
        public static void DrawBackgroundImage(this Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset, RightToLeft rightToLeft)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            if (backgroundImageLayout == ImageLayout.Tile)
            {
                using (var textureBrush = new TextureBrush(backgroundImage, WrapMode.Tile))
                {
                    if (scrollOffset != Point.Empty)
                    {
                        var transform = textureBrush.Transform;
                        transform.Translate(scrollOffset.X, scrollOffset.Y);
                        textureBrush.Transform = transform;
                    }
                    g.FillRectangle(textureBrush, clipRect);
                }
            }
            else
            {
                var backgroundImageRectangle = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);
                if (rightToLeft == RightToLeft.Yes && backgroundImageLayout == ImageLayout.None)
                    backgroundImageRectangle.X += clipRect.Width - backgroundImageRectangle.Width;
                if (rightToLeft == RightToLeft.Yes)
                {
                    g.Transform = new Matrix(-1, 0, 0, 1, bounds.Width, 0);

                }
                if (!clipRect.Contains(backgroundImageRectangle))
                {
                    switch (backgroundImageLayout)
                    {
                        case ImageLayout.Stretch:
                        case ImageLayout.Zoom:
                            backgroundImageRectangle.Intersect(clipRect);
                            g.DrawImage(backgroundImage, clipRect);
                            break;
                        case ImageLayout.None:
                            {
                                backgroundImageRectangle.Offset(clipRect.Location);
                                var destRect = backgroundImageRectangle;
                                destRect.Intersect(clipRect);
                                var rectangle = new Rectangle(Point.Empty, destRect.Size);
                                g.DrawImage(backgroundImage, destRect, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, GraphicsUnit.Pixel);
                            }
                            break;
                        default:
                            {
                                var destRect = backgroundImageRectangle;
                                destRect.Intersect(clipRect);
                                var rectangle = new Rectangle(new Point(destRect.X - backgroundImageRectangle.X, destRect.Y - backgroundImageRectangle.Y), destRect.Size);
                                g.DrawImage(backgroundImage, destRect, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, GraphicsUnit.Pixel);
                            }
                            break;
                    }
                }
                else
                {
                    var imageAttr = new ImageAttributes();
                    imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(backgroundImage, backgroundImageRectangle, 0, 0, backgroundImage.Width, backgroundImage.Height, GraphicsUnit.Pixel, imageAttr);
                    imageAttr.Dispose();
                }
            }
        }

        public static Rectangle RtlRectangle(this Rectangle rectangle, int width)
        {
            return new Rectangle(
                width - rectangle.Width - rectangle.X,
                rectangle.Y,
                rectangle.Width,
                rectangle.Height);
        }

        public static bool IsDrawMaximizeBox(this Form form)
        {
            return form.MaximizeBox && form.FormBorderStyle != FormBorderStyle.SizableToolWindow &&
                   form.FormBorderStyle != FormBorderStyle.FixedToolWindow;
        }
        /// <summary>
        /// Gets a value indicating if the minimize box needs to be drawn on the specified form.
        /// </summary>
        /// <param name="form">The form to check .</param>
        /// <returns></returns>
        public static bool IsDrawMinimizeBox(this Form form)
        {
            return form.MinimizeBox && form.FormBorderStyle != FormBorderStyle.SizableToolWindow &&
                   form.FormBorderStyle != FormBorderStyle.FixedToolWindow;
        }

    }
}
