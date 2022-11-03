using System;
using System.Drawing;
using static SkinFramWorkCore.NativeMethods;
using static SkinFramWorkCore.Extensions;
namespace SkinFramWorkCore
{
    
    public abstract class CaptionButtons
    {
        public CaptionButtons()
        {

        }
        public static DwmWindowCaption WindowCaption
        {
            get
            {
                switch (GetMsstylePlatform())
                {
                    case Platform.Vista:
                    case Platform.Win7:
                    case Platform.Win8:
                    case Platform.Win81:
                        return new Windows8Caption();
                    case Platform.Win10:
                    case Platform.Win11:
                    default:
                        return new Windows10Caption();
                }
            }
        }

        public static DwmButtonState ButtonState { get; set; }
        public abstract Image BackgrounImage { get; set; }
        public abstract Image GlyphImage { get; set; }
        public abstract Rectangle Bounds { get; set; }
        public abstract Color BackgroundColor { get; set; }
        public abstract bool IsActive { get; set; }
    }

    public class CloseButton : CaptionButtons
    {

        public CloseButton()
        {

        }

        public override Image BackgrounImage { get; set; } = GetDwmWindowButton(WindowCaption.BUTTONACTIVECLOS, (int)ButtonState);
        public override Image GlyphImage { get; set; } = GetDwmWindowButton(WindowCaption.BUTTONCLOSEGLYPH96, (int)ButtonState);
        public override Rectangle Bounds { get; set; }
        public override Color BackgroundColor { get; set; }
        public static new DwmButtonState ButtonState { get; set; }
        public override bool IsActive { get; set; }

        private void dr(Graphics graphics)
        {
            //Rectangle bounRect;
            //BackgrounImage = GetDwmWindowButton(active ? windowCaption.BUTTONACTIVECLOS : windowCaption.BUTTONAINCTIVECLOS, (int)state);
            //Image = GetDwmWindowButton(windowCaption.BUTTONCLOSEGLYPH96, active ? (int)state : (int)ButtonState.Disabled);
            //if (BackgrounImage == null || Image == null) return;
            //graphics.DrawImage(BackgrounImage, rect);
            //bounRect = new Rectangle((rect.Width - Image.Width) / 2, (rect.Height - Image.Height) / 2, Image.Width, Image.Height);
            //bounRect.Offset(rect.Location);
            //graphics.DrawImage(Image, bounRect);
        }
    }
}
