// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using static Interop;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SkinFramWorkCore
{
    internal static class SkinExtensions
    {
        internal static IntPtr hTheme;
        internal enum SkinPlatform
        {
            Vista,
            Win7,
            Win8,
            Win81,
            Win10,
            Win11
        }

        internal enum DwmButtonState
        {
            Normal = 1,
            Hot = 2,
            Pressed = 3,
            Disabled = 4
        }


        internal static Bitmap GetImageAtlasFromTheme()
        {
            hTheme = UxTheme.OpenThemeData(IntPtr.Zero, "DWMWINDOW");
            var path = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ThemeManager", "DllName", string.Empty)?.ToString();
            if (path == null)
                return new Bitmap(1, 1);
            var hInstance = Kernel32.LoadLibraryAsDataFile(path);
            try
            {
                IntPtr themeStream;
                uint streamSize;
                UxTheme.GetThemeStream(hTheme, 0, 0, 213, out themeStream, out streamSize, hInstance);
                var bufferStream = new byte[streamSize];
                Marshal.Copy(themeStream, bufferStream, 0, bufferStream.Length);
                using (var ms = new MemoryStream(bufferStream))
                {
                    return new Bitmap(Image.FromStream(ms));
                }
            }

            catch (Exception ex)
            {
                //ignore
                Debug.WriteLine(ex);
                if (hTheme != IntPtr.Zero)
                    UxTheme.CloseThemeData(hTheme);
                return new Bitmap(1, 1);
            }

            finally
            {
                Kernel32.FreeLibrary(hInstance);
                if (hTheme != IntPtr.Zero)
                    UxTheme.CloseThemeData(hTheme);
            }
        }

        static SkinExtensions()
        {
            MsStylePlatform = GetMsstylePlatform();
        }
        
        internal static SkinPlatform MsStylePlatform { get; private set; }

        internal static SkinPlatform GetMsstylePlatform()
        {
            var currentMsstylePath = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ThemeManager", "DllName", string.Empty)?.ToString();
            if (string.IsNullOrEmpty(currentMsstylePath))
                return SkinPlatform.Win10;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(currentMsstylePath);
            var fileVersion = fileVersionInfo.FileVersion;
            if (fileVersion == null)
                return SkinPlatform.Win10;
            var versionToken = fileVersion.Split('.');
            if (versionToken.Length > 0)
            {
                switch (versionToken[0])
                {
                    case "6":
                        switch (versionToken[1])
                        {
                            case "0":
                                return SkinPlatform.Vista;
                            case "1":
                                return SkinPlatform.Win7;
                            case "2":
                                return SkinPlatform.Win8;
                            case "3":
                                return SkinPlatform.Win81;
                        }

                        break;
                    case "10":
                        return int.Parse(versionToken[2]) < 22000 ? SkinPlatform.Win10 : SkinPlatform.Win11;

                    default:
                        return SkinPlatform.Win10;
                }
            }

            return SkinPlatform.Win10;
        }

        internal static DwmWindowCaption WindowCaption
        {
            get
            {
                switch (MsStylePlatform)
                {
                    case SkinPlatform.Vista:
                    case SkinPlatform.Win7:
                    case SkinPlatform.Win81:
                    case SkinPlatform.Win8:
                        return new Windows8Caption();
                    case SkinPlatform.Win10:
                    case SkinPlatform.Win11:
                        return new Windows10Caption();
                    default:
                        return new Windows10Caption();
                }
            }
        }

        internal static Bitmap Slice(this Bitmap original, Point loc, Size size)
        {
            return original.Clone(new Rectangle(loc, size), original.PixelFormat);
        }

        internal static Bitmap GetDwmWindowButton(int button, int state)
        {
            try
            {
                hTheme = UxTheme.OpenThemeData(IntPtr.Zero, "DWMWINDOW");
                const int tmtAtlasrect = 8002;
                const int tmtImagecount = 2401;
                var atlas = GetImageAtlasFromTheme();

                if (atlas.IsEmpty())
                    return new Bitmap(1, 1);
                UxTheme.GetThemeRect(hTheme, button, state, tmtAtlasrect, out RECT rect);

                var result = Slice(atlas, new Point(rect.X, rect.Y), rect.Size);
                if (state == 0)
                    return result;
                int count = 0;
                UxTheme.GetThemeInt(hTheme, button, state, tmtImagecount, ref count);
                var buttonSize = rect.Height / count;
                var startPoint = Point.Empty;
                var btnSize = new Size(rect.Width, buttonSize);
                startPoint.Offset(0, buttonSize * (state - 1));
                var buttonRect = new Rectangle(startPoint, btnSize);
                buttonRect.Inflate(-1, -1);
                result = Slice(result, buttonRect.Location, buttonRect.Size);
                return result;
            }
            catch
            {
                if (hTheme != IntPtr.Zero)
                    UxTheme.CloseThemeData(hTheme);
                return new Bitmap(0, 0);
            }

            finally
            {
                if (hTheme != IntPtr.Zero)
                    UxTheme.CloseThemeData(hTheme);
            }
        }

        internal static bool IsEmpty(this Bitmap image)
        {
            var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, image.PixelFormat);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            image.UnlockBits(data);
            return bytes.All(x => x == 0);
        }

        internal static GraphicsPath RoundedRect(Rectangle bounds, int radius)
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
            arc.Y = bounds.Bottom /*- diameter*/;
            path.AddArc(arc, 0, 90);

            // bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        internal static Color ContrastColor(this Color iColor)
        {
            // Calculate the perceptive luminance (aka luma) - human eye favors green color...
            double luma = ((0.299 * iColor.R) + (0.587 * iColor.G) + (0.114 * iColor.B)) / 255;
            // Return black for bright colors, white for dark colors
            return luma > 0.5 ? Color.Black : Color.White;
        }
        internal static bool isDarkColor(this Color iColor)
        {
            // Calculate the perceptive luminance (aka luma) - human eye favors green color...
            double luma = ((0.299 * iColor.R) + (0.587 * iColor.G) + (0.114 * iColor.B)) / 255;
            // Return black for bright colors, white for dark colors
            return luma <= 0.5;
        }
        internal static void DrawCloseButton(Graphics graphics, Rectangle rect, int state, bool active,bool isDark = false)
        {
            var backgroundImage = GetDwmWindowButton(active ? WindowCaption.BUTTONACTIVECLOSE : WindowCaption.BUTTONINACTIVECLOSE, state);
            int BUTTONCLOSEGLYPH = WindowCaption.BUTTONCLOSEGLYPH96;
            switch (graphics.DpiX)
            {
                case 96:
                    BUTTONCLOSEGLYPH = isDark ? WindowCaption.BUTTONCLOSEGLYPH96DARK : WindowCaption.BUTTONCLOSEGLYPH96;
                    break;
                case 120:
                    BUTTONCLOSEGLYPH = isDark ? WindowCaption.BUTTONCLOSEGLYPH120DARK : WindowCaption.BUTTONCLOSEGLYPH120;
                    break;
                case 144:
                    BUTTONCLOSEGLYPH = isDark ? WindowCaption.BUTTONCLOSEGLYPH144DARK : WindowCaption.BUTTONCLOSEGLYPH144;
                    break;
                case 168:
                case 192:
                    BUTTONCLOSEGLYPH = isDark ? WindowCaption.BUTTONCLOSEGLYPH192DARK : WindowCaption.BUTTONCLOSEGLYPH192;
                    break;
            }
            var Image = GetDwmWindowButton(BUTTONCLOSEGLYPH, active ? state : (int)DwmButtonState.Disabled);
            if (BackgrounImage == null || Image == null)
                return;
            graphics.DrawImage(backgroundImage, rect);
            var boundRect = new Rectangle((rect.Width - image.Width) / 2, (rect.Height - image.Height) / 2, image.Width, image.Height);
            boundRect.Offset(rect.Location);
            graphics.DrawImage(image, boundRect);
        }

        internal static void DrawMinimizeButton(Graphics graphics, Rectangle rect, int state, bool active, bool isDark = false)
        {
            var backgroundImage = GetDwmWindowButton(active ? WindowCaption.BUTTONACTIVECAPTION : WindowCaption.BUTTONINACTIVECAPTION, state);
            int BUTTONMINGLYPH = WindowCaption.BUTTONMINGLYPH96;
            switch (graphics.DpiX)
            {
                case 96:
                    BUTTONMINGLYPH = isDark ? WindowCaption.BUTTONMINGLYPH96DARK : WindowCaption.BUTTONMINGLYPH96;
                    break;
                case 120:
                    BUTTONMINGLYPH = isDark ? WindowCaption.BUTTONMINGLYPH120DARK : WindowCaption.BUTTONMINGLYPH120;
                    break;
                case 144:
                    BUTTONMINGLYPH = isDark ? WindowCaption.BUTTONMINGLYPH144DARK : WindowCaption.BUTTONMINGLYPH144;
                    break;
                case 168:
                case 192:
                    BUTTONMINGLYPH = isDark ? WindowCaption.BUTTONMINGLYPH192DARK : WindowCaption.BUTTONMINGLYPH192;
                    break;
            }

            var image = GetDwmWindowButton(BUTTONMINGLYPH, active ? state : (int)DwmButtonState.Disabled);
            if (backgroundImage == null || image == null)
                return;
            graphics.DrawImage(backgroundImage, rect);
            var boundRect = new Rectangle((rect.Width - image.Width) / 2, (rect.Height - image.Height) / 2, image.Width, image.Height);
            boundRect.Offset(rect.Location);
            graphics.DrawImage(image, boundRect);
        }

        internal static void DrawMaximizeButton(Graphics graphics, Rectangle rect, int state, bool active, bool isDark = false)
        {
            var backgroundImage = GetDwmWindowButton(active ? WindowCaption.BUTTONACTIVECAPTION : WindowCaption.BUTTONINACTIVECAPTION, state);
            int BUTTONMAXGLYPH = WindowCaption.BUTTONMAXGLYPH96;
            switch (graphics.DpiX)
            {
                case 96:
                    BUTTONMAXGLYPH = isDark ? WindowCaption.BUTTONMAXGLYPH96DARK : WindowCaption.BUTTONMAXGLYPH96;
                    break;
                case 120:
                    BUTTONMAXGLYPH = isDark ? WindowCaption.BUTTONMAXGLYPH120DARK : WindowCaption.BUTTONMAXGLYPH120;
                    break;
                case 144:
                    BUTTONMAXGLYPH = isDark ? WindowCaption.BUTTONMAXGLYPH144DARK : WindowCaption.BUTTONMAXGLYPH144;
                    break;
                case 168:
                case 192:
                    BUTTONMAXGLYPH = isDark ? WindowCaption.BUTTONMAXGLYPH192DARK : WindowCaption.BUTTONMAXGLYPH192;
                    break;
            }

            var image = GetDwmWindowButton(BUTTONMAXGLYPH, active ? state : (int)DwmButtonState.Disabled);
            if (backgroundImage == null || image == null)
                return;
            graphics.DrawImage(backgroundImage, rect);
            var boundRect = new Rectangle((rect.Width - image.Width) / 2, (rect.Height - image.Height) / 2, image.Width, image.Height);
            boundRect.Offset(rect.Location);
            graphics.DrawImage(image, boundRect);
        }


       
        internal static void DrawRestorButton(Graphics graphics, Rectangle rect, int state, bool active, bool isDark)
        {
            var BackgrounImage = GetDwmWindowButton(active ? WindowCaption.BUTTONACTIVECAPTION : WindowCaption.BUTTONINACTIVECAPTION, state);
            int BUTTONRESTOREGLYPH = WindowCaption.BUTTONRESTOREGLYPH96;
            switch (graphics.DpiX)
            {
                case 96:
                    BUTTONRESTOREGLYPH = isDark ? WindowCaption.BUTTONRESTOREGLYPH96DARK : WindowCaption.BUTTONRESTOREGLYPH96;
                    break;
                case 120:
                    BUTTONRESTOREGLYPH = isDark ? WindowCaption.BUTTONRESTOREGLYPH120DARK : WindowCaption.BUTTONMAXGLYPH120;
                    break;
                case 144:
                    BUTTONRESTOREGLYPH = isDark ? WindowCaption.BUTTONRESTOREGLYPH144DARK : WindowCaption.BUTTONMAXGLYPH144;
                    break;
                case 168:
                case 192:
                    BUTTONRESTOREGLYPH = isDark ? WindowCaption.BUTTONRESTOREGLYPH192DARK : WindowCaption.BUTTONMAXGLYPH192;
                    break;
            }
            var Image = GetDwmWindowButton(BUTTONRESTOREGLYPH, active ? state : (int)DwmButtonState.Disabled);
            if (BackgrounImage == null || Image == null)
                return;
            graphics.DrawImage(backgroundImage, rect);
            var boundRect = new Rectangle((rect.Width - image.Width) / 2, (rect.Height - image.Height) / 2, image.Width, image.Height);
            boundRect.Offset(rect.Location);
            graphics.DrawImage(image, boundRect);
        }

        internal static Rectangle RtlRectangle(this Rectangle rectangle, int width)
        {
            return new Rectangle(width - rectangle.Width - rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
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


        internal static void DrawCaptionButton(this Graphics graphics, Rectangle rect, CaptionButton captionButton, DwmButtonState state, bool active,bool isDark =false)
        {
            switch (captionButton)
            {
                case CaptionButton.Close:
                    DrawCloseButton(graphics, rect, (int)state, active,isDark);
                    break;
                case CaptionButton.Minimize:
                    DrawMinimizeButton(graphics, rect, (int)state, active,isDark);
                    break;
                case CaptionButton.Maximize:
                    DrawMaximizeButton(graphics, rect, (int)state, active, isDark);
                    break;
                case CaptionButton.Restore:
                    DrawRestorButton(graphics, rect, (int)state, active,isDark);
                    break;
                case CaptionButton.Help:
                    break;
            }
        }

        internal static bool IsDrawMaximizeBox(this Form form)
        {
            return form.MaximizeBox && form.FormBorderStyle != FormBorderStyle.SizableToolWindow &&
                   form.FormBorderStyle != FormBorderStyle.FixedToolWindow;
        }

        internal static bool IsDrawMinimizeBox(this Form form)
        {
            return form.MinimizeBox && form.FormBorderStyle != FormBorderStyle.SizableToolWindow &&
                   form.FormBorderStyle != FormBorderStyle.FixedToolWindow;
        }
    }
}
