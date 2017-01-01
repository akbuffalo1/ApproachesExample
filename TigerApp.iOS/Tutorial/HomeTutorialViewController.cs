using System;
using TigerApp.iOS.Pages;
using TigerApp.Shared;
using UIKit;
using CoreGraphics;

namespace TigerApp.iOS.Tutorial
{
    public class HomeTutorialViewController : TutorialViewController<HomeViewController>
    {
        public HomeTutorialViewController(HomeViewController vc) : base(vc)
        {
        }

        public override void OnTutorialFinished()
        {
            var flagStore = AD.Resolver.Resolve<IFlagStoreService>();
            flagStore.Set(nameof(Constants.Flags.HOME_TUTORIAL_SHOWN));
        }

        public override void SetupTutorial(HomeViewController parentViewController)
        {
            var registerSmsButton = parentViewController._signInRegisterCommand;
            var bottomToolbarFrame = parentViewController.bottomToolbar.Frame;
            bottomToolbarFrame = parentViewController.View.ConvertRectFromView(bottomToolbarFrame, _parentViewController.bottomToolbar.Superview);

            var leftButtonFrame = new CGRect(bottomToolbarFrame.GetMinX(), bottomToolbarFrame.GetMidY(), 50, bottomToolbarFrame.Height);
            var rightButtonFrame = new CGRect(bottomToolbarFrame.GetMaxX() - 50, bottomToolbarFrame.GetMidY(), 50, bottomToolbarFrame.Height);
            var registerButtonFrame = registerSmsButton.Frame;

            registerButtonFrame = parentViewController.View.ConvertRectFromView(registerButtonFrame, registerSmsButton.Superview);

            AddTutorialStep(new TutorialStep
            {
                Orientation = TutorialBubbleOrientation.BOTTOM_RIGHT,
                Point = new CGPoint(registerButtonFrame.GetMaxX() - 50, registerButtonFrame.GetMidY()),
                Text = "Ciao! Iscriviti al\nprogramma Tiger per\niniziare a collezionare\ni premi!"
            });

            AddTutorialStep(new TutorialStep
            {
                Orientation = TutorialBubbleOrientation.BOTTOM_LEFT,
                Point = new CGPoint(leftButtonFrame.GetMidX(), leftButtonFrame.GetMinY()),
                Text = "Scropri qui le ultime\nnovità Tiger!"
            });

            AddTutorialStep(new TutorialStep
            {
                Orientation = TutorialBubbleOrientation.BOTTOM_RIGHT,
                Point = new CGPoint(rightButtonFrame.GetMidX(), rightButtonFrame.GetMinY()),
                Text = "Scropri gli store Tiger\npiù vicino a te!"
            });
        }
    }
}
