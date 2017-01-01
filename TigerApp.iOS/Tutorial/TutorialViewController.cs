using System;
using TigerApp.iOS.Pages;
using TigerApp.iOS.Pages.Profile;
using TigerApp.Shared;
using UIKit;
using CoreGraphics;
using System.Linq;
using System.Collections.Generic;
using TigerApp.iOS.Views;

namespace TigerApp.iOS.Tutorial
{
    public class TutorialStep
    {
        public CGPoint Point { get; set; }
        public string Text { get; set; }
        public Action<TutorialBubble> OnPosition { get; set; }
        public TutorialBubbleOrientation Orientation { get; set; }
    }

    public abstract class TutorialViewController<TParentViewController> : UIViewController
    {
        protected Queue<TutorialStep> tutorialSteps = new Queue<TutorialStep>();
        protected TutorialBubble currentBubble;
        protected TParentViewController _parentViewController;
        public PartialTransparentView TransparentView;

        public TutorialViewController(TParentViewController parentViewController)
        {
            _parentViewController = parentViewController;
            ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            TransparentView = new PartialTransparentView();
            TransparentView.TranslatesAutoresizingMaskIntoConstraints = false;
        }

        public abstract void SetupTutorial(TParentViewController parentViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.Add(TransparentView);

            TransparentView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            TransparentView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            TransparentView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            TransparentView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

            SetupTutorial(_parentViewController);

            PopAndShowBubble();
            View.AddGestureRecognizer(new UITapGestureRecognizer(OnViewClick));
        }

        protected void AddTutorialStep(TutorialStep tutorialStep)
        {
            tutorialSteps.Enqueue(tutorialStep);
        }

        protected void OnViewClick()
        {
            PopAndShowBubble();
        }

        protected void HideCurrentBubble(Action next)
        {
            if (currentBubble == null)
            {
                next();
                return;
            }

            UIView.AnimateNotify(0.5F, () =>
            {
                currentBubble.Alpha = 0F;
            }, (finished) =>
            {
                if (finished)
                {
                    currentBubble.RemoveFromSuperview();
                    next();
                }
            });
        }

        public abstract void OnTutorialFinished();

        protected void PopAndShowBubble()
        {
            HideCurrentBubble(() =>
            {
                TransparentView.Clear();

                if (tutorialSteps.Count == 0)
                {
                    OnTutorialFinished();
                    DismissViewController(true, null);
                    return;
                }

                var bubble = tutorialSteps.Dequeue();

                currentBubble = new TutorialBubble(this, bubble);
                bubble.OnPosition?.Invoke(currentBubble);
                currentBubble.Hidden = false;

                UIView.AnimateNotify(0.5F, () =>
                {
                    currentBubble.Alpha = 1F;
                }, (finished) =>
                {
                });
            });
        }
    }
}