using System;
using CoreAnimation;
using CoreGraphics;
using TigerApp.iOS.Utils;
using UIKit;
using Foundation;

namespace TigerApp.iOS.Pages.ExpHome
{
    public class ProgressView : UIView
    {
        public CAShapeLayer circleLayer = new CAShapeLayer();
        public float Radius { get; set; }
        public nfloat LineWidth { get; set; }

        public override CALayer Layer
        {
            get
            {
                return base.Layer;
            }
        }

        public CGRect CircleFrame
        {
            get
            {
                var circleFrame = new CGRect(x: 0, y: 0, width: 2 * Radius, height: 2 * Radius);
                circleFrame.Location = new CGPoint(circleLayer.Bounds.GetMidX() - circleFrame.GetMidX(),
                    circleLayer.Bounds.GetMidY() - circleFrame.GetMidY());
                return circleFrame;
            }
        }

        public nfloat Percentage { get; set; }
        private const float offset = 0.010F; // offset from the black line

        private UIBezierPath CirclePath
        {
            get
            {
                var startAngle = (nfloat)(-Math.PI / 2F + offset);
                var endAngle = (nfloat)(2F * Math.PI) * Percentage + startAngle - offset * 2F;
                return UIBezierPath.FromArc(circleLayer.Bounds.Location, Radius, startAngle, endAngle, true);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            circleLayer.Frame = Bounds;
            circleLayer.EdgeAntialiasingMask = CAEdgeAntialiasingMask.All;
            circleLayer.Path = CirclePath.CGPath;
            AnimateCirclePath();
        }

        public string HexColor { get; private set; }

        public ProgressView(CGRect frame, float lineWidth, float radius, float percentage, string hexColor) : base(frame)
        {
            Percentage = percentage;
            Radius = radius;
            LineWidth = lineWidth;
            HexColor = hexColor;
            Initialize();
        }

        private void Initialize()
        {
            circleLayer.Frame = Bounds;
            circleLayer.LineWidth = LineWidth;
            circleLayer.StrokeEnd = 0F;
            circleLayer.FillColor = UIColor.Clear.CGColor;
            circleLayer.StrokeColor = Colors.ColorFromHexString(HexColor).CGColor;
            Layer.AddSublayer(circleLayer);
            BackgroundColor = UIColor.Clear;
        }

        private void AnimateCirclePath()
        {
            var animation = CABasicAnimation.FromKeyPath("strokeEnd");
            animation.From = NSObject.FromObject(circleLayer.StrokeEnd);
            animation.To = NSObject.FromObject(1F);
            animation.Duration = 1F;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
            animation.FillMode = CAFillMode.Both;
            animation.RemovedOnCompletion = false;
            circleLayer.AddAnimation(animation, animation.KeyPath);
        }
    }
}