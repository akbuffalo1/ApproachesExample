using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using Foundation;
using System.Linq;
using AD.iOS;
using System.Reactive.Linq;
using System.Globalization;

namespace TigerApp.iOS.Pages
{

    public class PigTailView : UIView
    {
        void _moveButton_TouchUpInside(object sender, EventArgs e)
        {

        }

        float PUNTI_IMAGE_SCALING_FACTOR = 0.7F;
        float PUNTI_IMAGE_OFFSET = 45;
        float LINE_WIDTH = 4F;
        float TRANSLATE_X = 210;
        float TRANSLATE_Y = -220;

        const string USERDEFAULTS_KEY = "BezierPath";

        private List<string> _savedList = new List<string>();
        private List<BezierSegment> _segments;
        private UIBezierPath _path = new UIBezierPath();
        private bool hideControl = true;
        private UIView _frameControl;

        void AnimateButton_TouchUpInside(object sender, EventArgs e)
        {
            AnimatePathReveal();
        }

        void ToggleButton_TouchUpInside(object sender, EventArgs e)
        {
            hideControl = !hideControl;
            ToggleControls();
        }

        public void ToggleControls()
        {
            foreach (var segment in _segments)
            {
                segment.ControlPointA.Hidden = hideControl;
                segment.ControlPointB.Hidden = hideControl;
                segment.StartPoint.Hidden = hideControl;
                segment.EndPoint.Hidden = hideControl;
                SetNeedsLayout();
                SetNeedsDisplay();
            }
        }

        float offsetX = 0;
        float offsetY = 0;

        public void LoadPath()
        {
            //var arr = NSUserDefaults.StandardUserDefaults.StringArrayForKey(USERDEFAULTS_KEY);
            var arr = new[] { "309.3333,484;239.3333,454.3333;281.3333,425.6667;227,423.3333", "239.3333,454.3333;220.3333,332.6667;265.3333,490.6667;343.6667,384.6667", "220.3333,332.6667;199.3333,306.3333;130.3333,306.3333;207.3333,410.3333", "199.3333,306.3333;118,268.6667;195.6667,277.6667;153.3333,253" };
            var ci = CultureInfo.InvariantCulture.NumberFormat;

            offsetX = 0;
            offsetY = 0;

            foreach (var seg in _segments)
            {
                seg.ControlPointA.RemoveFromSuperview();
                seg.ControlPointB.RemoveFromSuperview();
                seg.StartPoint.RemoveFromSuperview();
                seg.EndPoint.RemoveFromSuperview();
            }

            _segments.Clear();

            for (var i = 0; i < arr.Length; i += 1)
            {
                var points = arr[i].Split(';');
                var startPoint = points[0].Split(',');
                var endPoint = points[1].Split(',');
                var caPoint = points[2].Split(',');
                var cbPoint = points[3].Split(',');

                var segment = new BezierSegment();

                segment.StartPoint.Center = new CGPoint(float.Parse(startPoint[0], ci), float.Parse(startPoint[1], ci));
                segment.EndPoint.Center = new CGPoint(float.Parse(endPoint[0], ci), float.Parse(endPoint[1], ci));
                segment.ControlPointA.Center = new CGPoint(float.Parse(caPoint[0], ci), float.Parse(caPoint[1], ci));
                segment.ControlPointB.Center = new CGPoint(float.Parse(cbPoint[0], ci), float.Parse(cbPoint[1], ci));

                AddSubviews(segment.StartPoint, segment.EndPoint, segment.ControlPointA, segment.ControlPointB);

                var lastSegment = _segments.LastOrDefault();
                _segments.Add(segment);

                if (lastSegment != null)
                {
                    segment.StartPoint.RemoveFromSuperview();
                    segment.StartPoint = lastSegment.EndPoint;
                }
            }

            SetNeedsLayout();
            SetNeedsDisplay();
        }

        void LoadButton_TouchUpInside(object sender, EventArgs e)
        {
            LoadPath();
        }

        void SavePath()
        {
            _savedList = new List<string>();

            foreach (var segment in _segments)
            {
                var start = segment.StartPoint.Center;
                var end = segment.EndPoint.Center;
                var ca = segment.ControlPointA.Center;
                var cb = segment.ControlPointB.Center;
                var str = $"{start.X},{start.Y};{end.X},{end.Y};{ca.X},{ca.Y};{cb.X},{cb.Y}";
                _savedList.Add(str);
            }

            NSUserDefaults.StandardUserDefaults.SetValueForKeyPath(NSArray.FromObjects(_savedList.ToArray()), new NSString(USERDEFAULTS_KEY));
        }

        void SaveButton_TouchUpInside(object sender, EventArgs e)
        {
            SavePath();
        }

        void AddSegmentButton_TouchUpInside(object sender, EventArgs e)
        {
            AddSegment();
        }

        public class BezierSegment
        {
            public UIView StartPoint;
            public UIView EndPoint;
            public UIView ControlPointA;
            public UIView ControlPointB;

            public BezierSegment()
            {
                StartPoint = new UIView(new CGRect(150, 150, 8, 8));
                StartPoint.BackgroundColor = UIColor.Green;

                EndPoint = new UIView(new CGRect(243, 184, 8, 8));
                EndPoint.BackgroundColor = UIColor.Blue;

                ControlPointA = new UIView(new CGRect(148, 189, 8, 8));
                ControlPointA.BackgroundColor = UIColor.Orange;

                ControlPointB = new UIView(new CGRect(210, 182, 8, 8));
                ControlPointB.BackgroundColor = UIColor.Orange;

                AttachPanGestureRecognizer(StartPoint);
                AttachPanGestureRecognizer(EndPoint);
                AttachPanGestureRecognizer(ControlPointA);
                AttachPanGestureRecognizer(ControlPointB);
            }

            protected void AttachPanGestureRecognizer(UIView view)
            {
                view.AddGestureRecognizer(new UIPanGestureRecognizer((recognizer) =>
                {
                    var location = recognizer.LocationInView(view.Superview);
                    view.Center = location;
                    view.Superview.SetNeedsLayout();
                    view.Superview.SetNeedsDisplay();
                }));
            }
        }

        CAShapeLayer _layer;
        UIImageView _pungiImageView;
        UIButton _moveButton;
        CGPoint _previousLocation;

        public override bool PointInside(CGPoint point, UIEvent uievent)
        {
            return false;
        }

        public PigTailView() : base(CGRect.Empty)
        {
            _segments = new List<BezierSegment>();

            _pungiImageView = new UIImageView(UIImage.FromFile("punti"));
            _pungiImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            _pungiImageView.Hidden = true;
            Add(_pungiImageView);

            TranslatesAutoresizingMaskIntoConstraints = false;
            BackgroundColor = UIColor.Clear;

            AddSegment();

            _frameControl = new UIView(CGRect.Empty);
            _frameControl.Layer.BorderWidth = 2F;
            _frameControl.Layer.BorderColor = UIColor.Green.CGColor;

            _moveButton = new UIButton(UIButtonType.System);
            _moveButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            _moveButton.SetTitle("X", UIControlState.Normal);

            _moveButton.AddGestureRecognizer(new UIPanGestureRecognizer(recognizer =>
            {
                if (recognizer.State == UIGestureRecognizerState.Began)
                {
                    var _moveButtonCenter = this.ConvertPointFromView(_moveButton.Center, _moveButton.Superview);
                    _previousLocation = this.ConvertPointFromView(_moveButtonCenter, this);
                }
                else {
                    var location = recognizer.LocationInView(this);
                    offsetX = (float)(location.X - _previousLocation.X);
                    offsetY = (float)(location.Y - _previousLocation.Y);
                }

                SetNeedsLayout();
                SetNeedsDisplay();
            }));

            _moveButton.TranslatesAutoresizingMaskIntoConstraints = false;
            _frameControl.Add(_moveButton);

            _moveButton.CenterXAnchor.ConstraintEqualTo(_frameControl.CenterXAnchor).Active = true;
            _moveButton.CenterYAnchor.ConstraintEqualTo(_frameControl.CenterYAnchor).Active = true;

            //Add(_frameControl);

            //var addSegmentButton = new UIButton(UIButtonType.ContactAdd);
            //addSegmentButton.TranslatesAutoresizingMaskIntoConstraints = false;
            //addSegmentButton.TouchUpInside += AddSegmentButton_TouchUpInside;
            //Add(addSegmentButton);

            //var saveButton = new UIButton(UIButtonType.RoundedRect);
            //saveButton.SetTitle("SAVE", UIControlState.Normal);
            //saveButton.TranslatesAutoresizingMaskIntoConstraints = false;
            //saveButton.TouchUpInside += SaveButton_TouchUpInside;
            //Add(saveButton);

            //var loadButton = new UIButton(UIButtonType.RoundedRect);
            //loadButton.SetTitle("LOAD", UIControlState.Normal);
            //loadButton.TranslatesAutoresizingMaskIntoConstraints = false;
            //loadButton.TouchUpInside += LoadButton_TouchUpInside;
            //Add(loadButton);

            //var toggleButton = new UIButton(UIButtonType.RoundedRect);
            //toggleButton.SetTitle("TOGGLE", UIControlState.Normal);
            //toggleButton.TranslatesAutoresizingMaskIntoConstraints = false;
            //toggleButton.TouchUpInside += ToggleButton_TouchUpInside;
            //Add(toggleButton);

            //var animateButton = new UIButton(UIButtonType.RoundedRect);
            //animateButton.SetTitle("ANIMATE", UIControlState.Normal);
            //animateButton.TranslatesAutoresizingMaskIntoConstraints = false;
            //animateButton.TouchUpInside += AnimateButton_TouchUpInside;
            //Add(animateButton);

            //addSegmentButton.RightAnchor.ConstraintEqualTo(RightAnchor, -10).Active = true;
            //saveButton.RightAnchor.ConstraintEqualTo(toggleButton.LeftAnchor, -10).Active = true;
            //loadButton.RightAnchor.ConstraintEqualTo(saveButton.LeftAnchor, -10).Active = true;
            //addSegmentButton.TopAnchor.ConstraintEqualTo(TopAnchor, 30).Active = true;
            //saveButton.CenterYAnchor.ConstraintEqualTo(addSegmentButton.CenterYAnchor).Active = true;
            //loadButton.CenterYAnchor.ConstraintEqualTo(addSegmentButton.CenterYAnchor).Active = true;
            //toggleButton.CenterYAnchor.ConstraintEqualTo(addSegmentButton.CenterYAnchor).Active = true;
            //toggleButton.RightAnchor.ConstraintEqualTo(animateButton.LeftAnchor, -10).Active = true;
            //animateButton.CenterYAnchor.ConstraintEqualTo(addSegmentButton.CenterYAnchor).Active = true;
            //animateButton.RightAnchor.ConstraintEqualTo(addSegmentButton.LeftAnchor, -10).Active = true;
            DeviceHelper.OnIphone5(() =>
            {
                TRANSLATE_X = 25f;
                PUNTI_IMAGE_SCALING_FACTOR = 0.5F;
                PUNTI_IMAGE_OFFSET = 45;
                LINE_WIDTH = 3F;
                TRANSLATE_Y = -360;
            });

            DeviceHelper.OnIphone6(() =>
            {
                TRANSLATE_X = 140f;
                PUNTI_IMAGE_SCALING_FACTOR = 0.6F;
                PUNTI_IMAGE_OFFSET = 40;
                LINE_WIDTH = 4F;
                TRANSLATE_Y = -260;
            });
        }

        protected BezierSegment AddSegment()
        {
            var segment = new BezierSegment();
            AddSubviews(segment.StartPoint, segment.EndPoint, segment.ControlPointA, segment.ControlPointB);
            var lastSegment = _segments.LastOrDefault();
            _segments.Add(segment);

            if (lastSegment != null)
            {
                segment.StartPoint.RemoveFromSuperview();
                segment.StartPoint = lastSegment.EndPoint;
                SetNeedsLayout();
                SetNeedsDisplay();
            }

            return segment;
        }

        public void CreatePath()
        {
            _path = new UIBezierPath();
            _path.LineWidth = 2F;

            foreach (var segment in _segments)
            {
                var _startPoint = segment.StartPoint;
                var _endPoint = segment.EndPoint;
                var _controlPointA = segment.ControlPointA;
                var _controlPointB = segment.ControlPointB;

                _path.MoveTo(new CGPoint(_startPoint.Frame.GetMidX() + offsetX, _startPoint.Frame.GetMidY() + offsetY));

                _path.AddCurveToPoint(
                    new CGPoint(_endPoint.Frame.GetMidX() + offsetX, _endPoint.Frame.GetMidY() + offsetY),
                    new CGPoint(_controlPointA.Frame.GetMidX() + offsetX, _controlPointA.Frame.GetMidY() + offsetY),
                    new CGPoint(_controlPointB.Frame.GetMidX() + offsetX, _controlPointB.Frame.GetMidY() + offsetY));
            }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            // Draw bezier
            UIColor.DarkGray.SetStroke();
            CreatePath();

            var context = UIGraphics.GetCurrentContext();

            foreach (var segment in _segments)
            {
                if (hideControl)
                    continue;

                var _startPoint = segment.StartPoint;
                var _endPoint = segment.EndPoint;
                var _controlPointA = segment.ControlPointA;
                var _controlPointB = segment.ControlPointB;

                context.SaveState();
                context.SetLineWidth(2F);

                // Draw from A to start point
                UIColor.Red.SetStroke();
                context.MoveTo(_startPoint.Frame.GetMidX() + offsetX, _startPoint.Frame.GetMidY() + offsetY);
                context.AddLineToPoint(_controlPointA.Frame.GetMidX() + offsetX, _controlPointA.Frame.GetMidY() + offsetY);
                context.StrokePath();

                // Draw from B to end point
                UIColor.Red.SetStroke();
                context.MoveTo(_endPoint.Frame.GetMidX() + offsetX, _endPoint.Frame.GetMidY() + offsetY);
                context.AddLineToPoint(_controlPointB.Frame.GetMidX() + offsetX, _controlPointB.Frame.GetMidY() + offsetY);
                context.StrokePath();
                context.RestoreState();
            }

            if (!hideControl)
                _path.Stroke();

            _frameControl.Frame = _path.Bounds.Inset(-4, -4);
        }

        public void AnimatePathReveal()
        {
            _pungiImageView.Hidden = true;

            _layer?.RemoveFromSuperLayer();

            _layer = new CAShapeLayer();

            Layer.AddSublayer(_layer);

            var trans = CGAffineTransform.MakeScale(1f, 1F);
            trans.Translate(_segments[0].StartPoint.Center.X + TRANSLATE_X, _segments[0].StartPoint.Center.Y + TRANSLATE_Y);
            trans.Scale(0.5F, 0.5F);

            _layer.Frame = Bounds;
            _layer.Path = _path.CGPath.CopyByTransformingPath(trans);
            //_layer.AffineTransform.Rotate((float)Math.PI / 3);
            _layer.ZPosition = -2F;
            _layer.LineWidth = LINE_WIDTH;
            _layer.StrokeEnd = 0F;
            _layer.LineCap = new NSString("round");
            _layer.LineJoin = new NSString("round");
            _layer.FillColor = UIColor.Clear.CGColor;
            _layer.StrokeColor = UIColor.Black.CGColor;

            Observable.Timer(TimeSpan.FromMilliseconds(700)).ObserveOnUI().Subscribe(l =>
            {
                _pungiImageView.Hidden = false;

                var pt = _layer.Path.CurrentPoint;
                _pungiImageView.CenterXAnchor.ConstraintEqualTo(LeftAnchor, pt.X).Active = true;
                _pungiImageView.BottomAnchor.ConstraintEqualTo(TopAnchor, pt.Y + PUNTI_IMAGE_OFFSET).Active = true;
                _pungiImageView.Transform = CGAffineTransform.MakeScale(0.00001F, 0.00001F);

                UIView.AnimateNotify(0.3F, 0.0, 0.4F, 0.56F, UIViewAnimationOptions.CurveEaseIn, () =>
                {
                    _pungiImageView.Transform = CGAffineTransform.MakeScale(PUNTI_IMAGE_SCALING_FACTOR, PUNTI_IMAGE_SCALING_FACTOR);
                }, null);
            });

            var animation = CABasicAnimation.FromKeyPath("strokeEnd");
            animation.From = NSObject.FromObject(_layer.StrokeEnd);
            animation.To = NSObject.FromObject(1F);
            animation.Duration = 0.7F;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn);
            animation.FillMode = CAFillMode.Both;
            animation.RemovedOnCompletion = false;
            _layer.AddAnimation(animation, animation.KeyPath);
        }

        public void AnimatePathHide()
        {
            //var animation = CABasicAnimation.FromKeyPath("strokeStart");
            //animation.From = NSObject.FromObject(_layer.StrokeStart);
            //animation.To = NSObject.FromObject(1F);
            //animation.Duration = 1F;
            //animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
            //animation.FillMode = CAFillMode.Both;
            //animation.RemovedOnCompletion = false;
            //_layer.AddAnimation(animation, animation.KeyPath);

            //UIView.AnimateNotify(0.15F, 0.99F, UIViewAnimationOptions.CurveEaseOut, () =>
            //{
            //    _pungiImageView.Transform = CGAffineTransform.MakeScale(0.00001F, 0.00001F);
            //}, null);
        }
    }

    public partial class PuntoAnimationViewController : UIViewController
    {
        //private PigTailView piggie;

        //public PuntoAnimationViewController() : base("PuntoAnimationViewController", null)
        //{
        //    ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
        //    View.BackgroundColor = UIColor.Clear;
        //}

        //public override void ViewDidLoad()
        //{
        //    base.ViewDidLoad();

        //    //View.BackgroundColor = UIColor.DarkGray;
        //    //var pigTail = new UIImageView(UIImage.FromFile("pigtail.png"));
        //    //pigTail.ContentMode = UIViewContentMode.ScaleAspectFit;
        //    //pigTail.TranslatesAutoresizingMaskIntoConstraints = false;
        //    //pigTail.Layer.ZPosition = -100F;
        //    //Add(pigTail);
        //    //pigTail.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.5F).Active = true;
        //    //pigTail.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.5F).Active = true;
        //    //pigTail.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
        //    //pigTail.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;


        //    View.LayoutIfNeeded();
        //}

        //public override void ViewWillAppear(bool animated)
        //{
        //    base.ViewWillAppear(animated);
        //}

        //public void StartAnimation()
        //{
        //    piggie.AnimatePathReveal();
        //}
    }
}