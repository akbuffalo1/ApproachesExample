#region using

using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using TigerApp.Droid.Utils;
using Object = Java.Lang.Object;

#endregion

namespace TigerApp.Droid.UI.ToolTips
{
    public class SimpleTooltip : Object, PopupWindow.IOnDismissListener
    {
        // Default Resources
        private static readonly int DefaultPopupWindowStyleRes = Android.Resource.Attribute.PopupWindowStyle;

        private readonly View _anchorView;
        private readonly bool _animated;
        private readonly long _animationDuration;
        private readonly float _animationPadding;
        private readonly ViewTreeObserver.IOnGlobalLayoutListener _autoDismissLayoutListener;
        private readonly View _contentView;
        private readonly Context _context;
        private readonly bool _dismissOnInsideTouch;
        private readonly bool _dismissOnOutsideTouch;
        private readonly bool _drawOval;
        private readonly GravityFlags _gravity;
        private readonly float _margin;
        private readonly float _maxWidth;

        private readonly ViewTreeObserver.IOnGlobalLayoutListener _mLocationLayoutListener;
        private readonly bool _modal;
        private readonly float _padding;
        private readonly bool _showArrow;
        private readonly string _text;
        private readonly int _textViewId;
        private readonly bool _transparentOverlay;
        private RectF _anchorRect;
        private AnimatorSet _animator;
        private ArrowView _arrow;
        private View _contentLayout;
        private bool _dismissed;
        private IOnDismissListener _onDismissListener;

        public IOnDismissListener OnDismissListener
        {
            get { return _onDismissListener; }
            set { _onDismissListener = value; }
        }

        private IOnShowListener _onShowListener;
        private View _overlay;
        private PopupWindow _popupWindow;
        private ViewGroup _rootView;
        private ContentWrapper _wrapper;
        private Point _winSize;
        private Rect _sideMargins;

        internal SimpleTooltip(TooltipBuilder tooltipBuilder)
        {
            _context = tooltipBuilder.Context;
            _gravity = tooltipBuilder.Gravity;
            _dismissOnInsideTouch = tooltipBuilder.DismissOnInsideTouch;
            _dismissOnOutsideTouch = tooltipBuilder.DismissOnOutsideTouch;
            _modal = tooltipBuilder.Modal;
            _contentView = tooltipBuilder.ContentView;
            _textViewId = tooltipBuilder.TextViewId;
            _text = tooltipBuilder.Text;
            _anchorView = tooltipBuilder.AnchorView;
            _transparentOverlay = tooltipBuilder.TransparentOverlay;
            _maxWidth = tooltipBuilder.MaxWidth;
            _showArrow = tooltipBuilder.ShowArrow;
            _animated = tooltipBuilder.Animated;
            _margin = tooltipBuilder.Margin;
            _drawOval = tooltipBuilder.DrawOval;
            _padding = tooltipBuilder.Adding;
            _animationPadding = tooltipBuilder.AnimationPadding;
            _animationDuration = tooltipBuilder.AnimationDuration;
            _onDismissListener = tooltipBuilder.OnDismissListener;
            _onShowListener = tooltipBuilder.OnShowListener;
            _sideMargins = tooltipBuilder.SidesMargin;
            _rootView = (ViewGroup)(_anchorView != null ? _anchorView.RootView : tooltipBuilder.RootView);

            _mLocationLayoutListener = new LocationOnGlobalLayoutListener(this);
            _autoDismissLayoutListener = new AutoDismissOnGlobalLayoutListener(this);
            //            _animationLayoutListener = new AnimationOnGlobalLayoutListener(this);

            Init();
        }

        public virtual bool Showing
        {
            get { return _popupWindow != null && _popupWindow.IsShowing; }
        }

        public void OnDismiss()
        {
            _dismissed = true;

            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Honeycomb)
            {
                if (_animator != null)
                {
                    _animator.RemoveAllListeners();
                    _animator.End();
                    _animator.Cancel();
                    _animator = null;
                }
            }

            if (_rootView != null)
            {
                if (_overlay != null)
                {
                    //_rootView.RemoveView(_overlay);
                    var anim = new AlphaAnimation(1, 0);
                    anim.Duration = 100;
                    var _over = _overlay;
                    anim.AnimationEnd += (sender, e) =>
                    {
                        ((ViewGroup)_over.Parent).RemoveView(_over);
                        _over = null;
                    };
                    _over.StartAnimation(anim);
                }

                if (_arrow != null && _arrow.Parent == _rootView)
                    _rootView.RemoveView(_arrow);
            }

            _rootView = null;
            _overlay = null;
            _arrow = null;

            if (_onDismissListener != null)
                _onDismissListener.OnDismiss(this);

            _onDismissListener = null;

            _popupWindow = null;
        }

        private void Init()
        {
            ConfigPopupWindow();
            ConfigContentView();
        }

        private void ConfigPopupWindow()
        {
            _popupWindow = new PopupWindow(_context, null, DefaultPopupWindowStyleRes);
            _popupWindow.SetOnDismissListener(this);
            _popupWindow.Width = ViewGroup.LayoutParams.WrapContent;
            _popupWindow.Height = ViewGroup.LayoutParams.WrapContent;
            _popupWindow.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            _popupWindow.ClippingEnabled = false;
        }

        public virtual void Show()
        {
            VerifyDismissed();

            _contentLayout.ViewTreeObserver.AddOnGlobalLayoutListener(_mLocationLayoutListener);
            _contentLayout.ViewTreeObserver.AddOnGlobalLayoutListener(_autoDismissLayoutListener);

            _rootView.Post(
                () =>
                {
                    _popupWindow.ShowAtLocation(_rootView, GravityFlags.NoGravity, _rootView.Width, _rootView.Height);
                });
        }

        private void VerifyDismissed()
        {
            if (_dismissed)
                throw new ArgumentException("Tooltip has ben dismissed.");
        }

        private void CreateOverlay(RectF popupLocation)
        {
            _overlay = _transparentOverlay ? new View(_context) : new OverlayView(_context, _anchorView, _drawOval);
            _overlay.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);

            _overlay.Touch += OnOverlayTouch;
            _rootView.AddView(_overlay);
            _rootView.AddView(_arrow);
        }

        private void OnOverlayTouch(object sender, View.TouchEventArgs e)
        {
            if (_dismissOnOutsideTouch)
                Dismiss();

            if (e.Event.Action == MotionEventActions.Up)
                ((View)sender).PerformClick();

            e.Handled = _modal;
        }

        private PointF CalculePopupLocation()
        {
            var location = new PointF();

            _winSize = ScreenUtils.GetWindowSize(_context);

            if (_anchorView != null)
                _anchorRect = SimpleTooltipUtils.CalculeRectInWindow(_anchorView);
            else
                _anchorRect = new RectF(_winSize.X * 0.5f, _winSize.Y * 0.5f, _winSize.X * 0.5f, _winSize.Y * 0.5f);

            var anchorCenter = new PointF(_anchorRect.CenterX(), _anchorRect.CenterY());

            var f = _gravity;

            switch (f)
            {
                case GravityFlags.Start:
                    location.X = _anchorRect.Left - _popupWindow.ContentView.Width - _margin;
                    location.Y = anchorCenter.Y - _popupWindow.ContentView.Height / 2f;
                    break;
                case GravityFlags.End:
                    location.X = _anchorRect.Right + _margin;
                    location.Y = anchorCenter.Y - _popupWindow.ContentView.Height / 2f;
                    break;
                case GravityFlags.Top:
                    location.X = anchorCenter.X - _popupWindow.ContentView.Width / 2f;
                    location.Y = _anchorRect.Top - _popupWindow.ContentView.Height - _margin;
                    break;
                case GravityFlags.Bottom:
                    location.X = anchorCenter.X - _popupWindow.ContentView.Width / 2f;
                    location.Y = _anchorRect.Bottom + _margin;
                    break;
                case GravityFlags.Center:
                    location.X = anchorCenter.X - _popupWindow.ContentView.Width / 2f;
                    location.Y = anchorCenter.Y - _popupWindow.ContentView.Height / 2f;
                    break;
                default:
                    throw new ArgumentException("Gravity must have be CENTER, START, END, TOP or BOTTOM.");
            }

            return location;
        }

        private void ConfigContentView()
        {
            if (_contentView is TextView)
            {
                var tv = (TextView)_contentView;
                tv.Text = _text;
            }
            else
            {
                var tv = (TextView)_contentView.FindViewById(_textViewId);
                if (tv != null)
                    tv.Text = _text;
            }

            //_contentView.SetPadding((int) _padding, (int) _padding, (int) _padding, (int) _padding);

            var contentViewParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent, 0);
            contentViewParams.Gravity = GravityFlags.Center;
            contentViewParams.SetMargins(_sideMargins.Left, _sideMargins.Top, _sideMargins.Right, _sideMargins.Bottom);
            _contentView.LayoutParameters = contentViewParams;

            _wrapper = new ContentWrapper(_contentView.Context);
            var wrapperParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent);

            _wrapper.LayoutParameters = wrapperParams;

            _arrow = new ArrowView(_contentView.Context);
            _wrapper.AddView(_contentView);
            _wrapper.Invalidate();

            if (_dismissOnInsideTouch || _dismissOnOutsideTouch)
                _contentView.Touch += OnPopUpTouch;

            _contentLayout = _wrapper;
            _contentLayout.Visibility = ViewStates.Invisible;
            _arrow.Visibility = ViewStates.Invisible;
            _popupWindow.ContentView = _contentLayout;
        }

        private void OnPopUpTouch(object sender, View.TouchEventArgs e)
        {
            var v = sender as View;
            var @event = e.Event;
            if (@event.GetX() > 0 && @event.GetX() < v.Width && @event.GetY() > 0 && @event.GetY() < v.Height)
            {
                if (_dismissOnInsideTouch)
                {
                    Dismiss();
                    e.Handled = _modal;
                    return;
                }
                e.Handled = false;
                return;
            }

            if (@event.Action == MotionEventActions.Up)
                v.PerformClick();

            e.Handled = _modal;
        }

        public virtual void Dismiss()
        {
            if (_dismissed)
                return;

            _dismissed = true;

            _arrow.Visibility = ViewStates.Invisible;

            if (_popupWindow != null)
                _popupWindow.Dismiss();
        }

        public virtual T FindViewById<T>(int id) where T : View
        {
            //noinspection unchecked
            return (T)_contentLayout.FindViewById(id);
        }

        private void StartAnimation()
        {
            var property = _gravity == GravityFlags.Top || _gravity == GravityFlags.Bottom
                ? "translationY"
                : "translationX";

            var anim1 = ObjectAnimator.OfFloat(_contentLayout, property, -_animationPadding, _animationPadding);
            anim1.SetDuration(_animationDuration);
            //anim1.SetInterpolator(AccelerateDecelerateInterpolator());

            var anim2 = ObjectAnimator.OfFloat(_contentLayout, property, _animationPadding, -_animationPadding);
            anim2.SetDuration(_animationDuration);
            anim2.SetInterpolator(new AccelerateDecelerateInterpolator());

            _animator = new AnimatorSet();
            _animator.PlaySequentially(anim1, anim2);
            _animator.AddListener(new AnimatorListenerAdapterAnonymousInnerClass(this));
            _animator.Start();
        }

        private class LocationOnGlobalLayoutListener : Object, ViewTreeObserver.IOnGlobalLayoutListener
        {
            private readonly SimpleTooltip t;

            public LocationOnGlobalLayoutListener(SimpleTooltip tip)
            {
                t = tip;
            }

            public void OnGlobalLayout()
            {
                if (t._dismissed)
                {
                    SimpleTooltipUtils.RemoveOnGlobalLayoutListener(t._popupWindow.ContentView, this);
                    return;
                }

                if (t._maxWidth > 0 && t._contentView.Width > t._maxWidth)
                {
                    SimpleTooltipUtils.SetWidth(t._contentView, t._maxWidth);
                    t._popupWindow.Update(ViewGroup.LayoutParams.WrapContent,
                        ViewGroup.LayoutParams.WrapContent);
                    return;
                }

                SimpleTooltipUtils.RemoveOnGlobalLayoutListener(t._popupWindow.ContentView, this);
                //                t._popupWindow.SetContentView.ViewTreeObserver.AddOnGlobalLayoutListener(_arrowLayoutListener);
                var location = t.CalculePopupLocation();
                t._popupWindow.ClippingEnabled = true;

                t._popupWindow.Update((int)location.X, (int)location.Y, t._popupWindow.Width,
                    t._popupWindow.Height);

                t._popupWindow.ContentView.RequestLayout();

                t.CreateOverlay(new RectF((int)location.X, (int)location.Y, t._popupWindow.Width,
                    t._popupWindow.Height));

                if (t._onShowListener != null)
                    t._onShowListener.OnShow(t);

                t._onShowListener = null;

                if (location.X < 0)
                    location.X = t._sideMargins.Left;
                if (location.X + t._contentLayout.Width > t._winSize.X)
                    location.X = t._winSize.X - t._contentLayout.Width - t._sideMargins.Right;

                var anchorCenter = new PointF(t._anchorRect.CenterX(), t._anchorRect.CenterY());
                var popupCenter = new PointF(location.X + t._contentLayout.Width * 0.5f,
                    location.Y + t._contentLayout.Height * 0.5f);
                var ang = (float)MathUtil.GetAngle(anchorCenter.X, anchorCenter.Y, popupCenter.X, popupCenter.Y);

                var a = t._arrow;
                int dx;
                int dy;
                if (t._gravity == GravityFlags.Top)
                {
                    dx = (int)(Math.Sin(ang) * a.ArrowWidth * 0.5f);
                    dy = (int)(Math.Cos(ang) * a.ArrowWidth * 0.5f);
                    a.SetX(anchorCenter.X + dx);
                    a.SetY(anchorCenter.Y - dy);
                }
                else
                {
                    dx = (int)(Math.Sin(ang) * a.ArrowWidth * 0.5f);
                    dy = (int)(Math.Cos(ang) * a.ArrowWidth * 0.5f);
                    a.SetX(anchorCenter.X + dx);
                    a.SetY(anchorCenter.Y - dy);
                }
                a.ArrowHeight = (int)MathUtil.Distance(a.GetX(), a.GetY(), popupCenter.X, popupCenter.Y);

                a.PivotX = 0;
                a.PivotY = 0;
                a.Rotation = (float)MathUtil.ToDegrees * ang + 90;

                t._contentLayout.Visibility = ViewStates.Visible;
                t._arrow.Visibility = t._showArrow ? ViewStates.Visible : ViewStates.Gone;
                var anim = new AlphaAnimation(0, 1);
                anim.Duration = 100;
                t._contentLayout.StartAnimation(anim);
                t._arrow.StartAnimation(anim);
            }
        }

        private class AnimatorListenerAdapterAnonymousInnerClass : AnimatorListenerAdapter
        {
            private readonly SimpleTooltip _outerInstance;

            public AnimatorListenerAdapterAnonymousInnerClass(SimpleTooltip outerInstance)
            {
                _outerInstance = outerInstance;
            }


            public override void OnAnimationEnd(Animator animation)
            {
                if (!_outerInstance._dismissed && _outerInstance.Showing)
                    animation.Start();
            }
        }

        private class AutoDismissOnGlobalLayoutListener : Object, ViewTreeObserver.IOnGlobalLayoutListener
        {
            private readonly SimpleTooltip _outerInstance;

            public AutoDismissOnGlobalLayoutListener(SimpleTooltip outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public void OnGlobalLayout()
            {
                if (_outerInstance._dismissed)
                {
                    SimpleTooltipUtils.RemoveOnGlobalLayoutListener(_outerInstance._popupWindow.ContentView, this);
                    return;
                }

                if (!_outerInstance._rootView.IsShown)
                    _outerInstance.Dismiss();
            }
        }

        public interface IOnDismissListener
        {
            void OnDismiss(SimpleTooltip tooltip);
        }

        public interface IOnShowListener
        {
            void OnShow(SimpleTooltip tooltip);
        }
    }
}