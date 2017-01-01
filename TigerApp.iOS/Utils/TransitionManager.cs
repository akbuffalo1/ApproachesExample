using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace TigerApp.iOS
{
    public class HorizontalTransition : UIViewControllerAnimatedTransitioning
    {
        public enum TransitionDirection { LEFT, RIGHT }
        public TransitionDirection Direction { get; private set; }

        public HorizontalTransition(TransitionDirection direction)
        {
            Direction = direction;
        }

        public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var container = transitionContext.ContainerView;

            var fromView = transitionContext.GetViewFor(UITransitionContext.FromViewKey);
            var fromViewVc = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);

            var toView = transitionContext.GetViewFor(UITransitionContext.ToViewKey);
            var toViewVc = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

            fromView.Layer.ShouldRasterize = true;
            toView.Layer.ShouldRasterize = true;

            var isReverse = fromViewVc.IsBeingDismissed;

            if (isReverse)
            {
                fromView.Frame = transitionContext.GetFinalFrameForViewController(toViewVc);
            }
            else {
                toView.Frame = transitionContext.GetFinalFrameForViewController(toViewVc);
            }

            // add the both views to our view controller
            container.AddSubview(toView);
            container.AddSubview(fromView);

            var isLeft = Direction == TransitionDirection.LEFT;
            var directionMultiplier = (isLeft ? 1 : -1);

            var offScreenRight = CGAffineTransform.MakeTranslation(-directionMultiplier * container.Frame.Width, 0);
            var offScreenLeft = CGAffineTransform.MakeTranslation(directionMultiplier * container.Frame.Width, 0);

            if (isReverse)
            {
                toView.Transform = offScreenLeft;
            }
            else {
                toView.Transform = offScreenRight;
            }

            var duration = TransitionDuration(transitionContext);

            UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseIn, () =>
            {
                if (isReverse)
                {
                    fromView.Transform = offScreenRight;
                    toView.Transform = CGAffineTransform.MakeIdentity();
                }
                else {
                    fromView.Transform = offScreenLeft;
                    toView.Transform = CGAffineTransform.MakeIdentity();
                }
            }, (finished) =>
            {
                fromView.Layer.ShouldRasterize = false;
                toView.Layer.ShouldRasterize = false;
                transitionContext.CompleteTransition(finished);
            });
        }

        public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return 0.3;
        }

        public static HorizontalTransition Left => new HorizontalTransition(TransitionDirection.LEFT);
        public static HorizontalTransition Right => new HorizontalTransition(TransitionDirection.RIGHT);
    }

    public class VerticalTransition : UIViewControllerAnimatedTransitioning
    {
        public enum TransitionDirection { TOP, BOTTOM }
        public TransitionDirection Direction { get; private set; }

        public VerticalTransition(TransitionDirection direction)
        {
            Direction = direction;
        }

        public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var container = transitionContext.ContainerView;

            var fromView = transitionContext.GetViewFor(UITransitionContext.FromViewKey);
            var fromViewVc = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);

            var toView = transitionContext.GetViewFor(UITransitionContext.ToViewKey);
            var toViewVc = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

            fromView.Layer.ShouldRasterize = true;
            toView.Layer.ShouldRasterize = true;

            var isReverse = fromViewVc.IsBeingDismissed;

            if (isReverse)
            {
                fromView.Frame = transitionContext.GetFinalFrameForViewController(toViewVc);
            }
            else {
                toView.Frame = transitionContext.GetFinalFrameForViewController(toViewVc);
            }

            // add the both views to our view controller
            container.AddSubview(toView);
            container.AddSubview(fromView);

            var isUp = Direction == TransitionDirection.TOP;
            var directionMultiplier = (isUp ? 1 : -1);

            var offScreenDown = CGAffineTransform.MakeTranslation(0, -directionMultiplier * container.Frame.Height);
            var offScreenUp = CGAffineTransform.MakeTranslation(0, directionMultiplier * container.Frame.Height);

            if (isReverse)
            {
                toView.Transform = offScreenUp;
            }
            else {
                toView.Transform = offScreenDown;
            }

            var duration = TransitionDuration(transitionContext);

            UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseIn, () =>
            {
                if (isReverse)
                {
                    fromView.Transform = offScreenDown;
                    toView.Transform = CGAffineTransform.MakeIdentity();
                }
                else {
                    fromView.Transform = offScreenUp;
                    toView.Transform = CGAffineTransform.MakeIdentity();
                }
            }, (finished) =>
            {
                fromView.Layer.ShouldRasterize = false;
                toView.Layer.ShouldRasterize = false;
                transitionContext.CompleteTransition(finished);
            });
        }

        public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return 0.3;
        }

        public static VerticalTransition Top => new VerticalTransition(TransitionDirection.TOP);
        public static VerticalTransition Bottom => new VerticalTransition(TransitionDirection.BOTTOM);
    }

    public class TransitionManager : UIViewControllerTransitioningDelegate
    {
        private readonly UIViewControllerAnimatedTransitioning _animator;

        public static TransitionManager Left => new TransitionManager(HorizontalTransition.Left);
        public static TransitionManager Right => new TransitionManager(HorizontalTransition.Right);
        public static TransitionManager Top => new TransitionManager(VerticalTransition.Top);
        public static TransitionManager Bottom => new TransitionManager(VerticalTransition.Bottom);

        public TransitionManager(UIViewControllerAnimatedTransitioning animator)
        {
            _animator = animator;
        }

        public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
        {
            return _animator;
        }

        public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
        {
            return _animator;
        }
    }
}