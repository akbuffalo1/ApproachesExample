using System;
using UIKit;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using MaskTuple = System.Tuple<CoreGraphics.CGRect, UIKit.UIImage>;

namespace TigerApp.iOS.Views
{
    public class PartialTransparentView : UIView
    {
        private List<CGRect> _rects;
        private List<CGRect> _circles;
        private List<MaskTuple> _masks;
        private UIColor _bgColor;

        public PartialTransparentView(IntPtr handle) : base(handle) { }

        public PartialTransparentView() : base()
        {
            _rects = new List<CGRect>();
            _circles = new List<CGRect>();
            _masks = new List<MaskTuple>();

            Opaque = false;

            _bgColor = new UIColor(0, 0, 0, 0.5F);
        }

        public void AddCircle(CGRect circleFrame)
        {
            _circles.Add(circleFrame);
        }

        public void AddRect(CGRect rectFrame)
        {
            _rects.Add(rectFrame);
        }

        public override void Draw(CGRect area)
        {
            base.Draw(area);

            var context = UIGraphics.GetCurrentContext();
            context.SetFillColor(_bgColor.CGColor);
            context.FillRect(this.Frame);

            foreach (var rect in _rects)
            {
                if (CGRect.Intersect(rect, area) != CGRect.Empty)
                {
                    context.ClearRect(rect);
                }
            }

            foreach (var circleRect in _circles)
            {
                if (CGRect.Intersect(circleRect, area) != CGRect.Empty)
                {
                    context.AddEllipseInRect(circleRect);
                    context.Clip();
                    context.ClearRect(circleRect);
                    context.SetFillColor(UIColor.Clear.CGColor);
                    context.FillRect(circleRect);
                }
            }

            foreach (var mask in _masks)
            {
                var rect = mask.Item1;
                var image = mask.Item2;

                if (CGRect.Intersect(rect, area) != CGRect.Empty)
                {
                    context.SaveState();
                    context.AddRect(rect);
                    context.TranslateCTM(0, rect.Height);
                    context.ScaleCTM(1F, -1F);
                    context.ClipToMask(new CGRect(rect.X, -rect.Y, rect.Width, rect.Height), image.CGImage);
                    context.ScaleCTM(1F, -1F);
                    context.TranslateCTM(0, -rect.Height);
                    context.ClearRect(rect);
                    context.RestoreState();
                }
            }
        }

        public void AddImageMask(CGRect frame, UIImage image)
        {
            _masks.Add(new MaskTuple(frame, image));
        }

        public void Clear()
        {
            _masks.Clear();
            _rects.Clear();
            _circles.Clear();
            SetNeedsLayout();
            SetNeedsDisplay();
        }
    }
}
