using System;
using CoreGraphics;
using TigerApp.iOS.Pages.Profile;
using TigerApp.Shared;
using UIKit;

namespace TigerApp.iOS.Tutorial
{
    public class ProfileTutorialViewController : TutorialViewController<ProfileViewController>
    {
        public ProfileTutorialViewController(ProfileViewController vc) : base(vc)
        {
        }

        public override void OnTutorialFinished()
        {
            var flagStore = AD.Resolver.Resolve<IFlagStoreService>();
            flagStore.Set(nameof(Constants.Flags.PROFILE_PAGE_TUTORIAL_SHOWN));
        }

        public override void SetupTutorial(ProfileViewController parentViewController)
        {
            var giftButtonFrame = parentViewController.GiftCommand.Frame;
            var editButtonFrame = parentViewController.EditProfileCommand.Frame;
            var profileImageFrame = parentViewController.ProfileImage.Frame;

            var secondBadge = parentViewController.CollectionView.VisibleCells[2];
            var badgeCollectionViewFrame = secondBadge.Frame;
            badgeCollectionViewFrame = parentViewController.View.ConvertRectFromView(badgeCollectionViewFrame, secondBadge.Superview);

            AddTutorialStep(new TutorialStep
            {
                Text = "Clica sul tuo avatar\nper sceglierne un altro!",
                Point = new CGPoint(profileImageFrame.GetMidX(), profileImageFrame.GetMaxY()),
                Orientation = TutorialBubbleOrientation.TOP_LEFT
            });

            AddTutorialStep(new TutorialStep
            {
                Text = "Gioca con Tiger per\nvincere i tuoi badge!",
                Point = new CGPoint(badgeCollectionViewFrame.GetMidX(), badgeCollectionViewFrame.GetMinY()),
                Orientation = TutorialBubbleOrientation.BOTTOM_RIGHT
            });

            AddTutorialStep(new TutorialStep
            {
                Text = "Scopri i tuoi coupon\ne bruciali in store",
                Point = new CGPoint(giftButtonFrame.GetMaxX(), giftButtonFrame.GetMaxY()),
                Orientation = TutorialBubbleOrientation.TOP_LEFT
            });

            AddTutorialStep(new TutorialStep
            {
                Text = "Completa e verifica i\ntuoi dati per completare\nuna missione!",
                Point = new CGPoint(editButtonFrame.GetMinX(), editButtonFrame.GetMaxY()),
                Orientation = TutorialBubbleOrientation.TOP_RIGHT
            });
        }
    }
}

