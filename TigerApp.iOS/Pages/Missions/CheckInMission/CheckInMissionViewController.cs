using Foundation;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using TigerApp.iOS.Utils;
using TigerApp.Shared;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    [Register("CheckInMissionViewController")]
    public class CheckInMissionViewController : BaseReactiveViewController<ICheckInMissionViewModel>
    {
        void CompleteCommand_TouchUpInside(object sender, EventArgs e)
        {
            var locationService = AD.Resolver.Resolve<Shared.Services.Platform.ILocationService>();
            locationService.CurrentLocationAsync().ToObservable().SubscribeOnce(userLocation =>
            {
                ViewModel.TryToCompleteMission.ExecuteAsync(userLocation).SubscribeOnce(missionStatus =>
                {
                    if (missionStatus == MissionStatus.Completed)
                    {
                        PresentViewController(new ExpHome.ExpHomeViewController(), true, null);
                    }
                    else if (missionStatus == MissionStatus.Failed)
                    {
                        PresentViewController(new CheckInMissionTooFarViewController(), true, null);
                    }
                });
            });
        }

        public CheckInMissionViewController()
        {

        }

        protected override void LayoutViews()
        {
            View.BackgroundColor = UIColor.White;

            var mainStack = UICommon.CreateStackView(alignment: UIStackViewAlignment.Center);

            var topStripe = new UIView { BackgroundColor = Colors.HexF8F7F5 };
            topStripe.HeightAnchor.ConstraintEqualTo(64).Active = true;

            var navStack = UICommon.CreateStackView(axis: UILayoutConstraintAxis.Horizontal, alignment: UIStackViewAlignment.LastBaseline, spacing: 5);
            navStack.LayoutMargins = new UIEdgeInsets(5, 5, 2, 5);
            navStack.LayoutMarginsRelativeArrangement = true;

            var back = UICommon.CreateBackIcon();
            back.AddGestureRecognizer(new UITapGestureRecognizer(() => { DismissViewController(true, null); }));

            var title = UICommon.CreateLabel(Fonts.TigerCandy.WithSize(30), UIColor.Black, UITextAlignment.Center, lines: 1);
            title.Text = Constants.Strings.CheckInMissionPageTitle;

            var dummy = new UIView();
            dummy.WidthAnchor.ConstraintEqualTo(41).Active = true;
            dummy.HeightAnchor.ConstraintEqualTo(27).Active = true;

            navStack.AddArrangedSubview(back);
            navStack.AddArrangedSubview(title);
            navStack.AddArrangedSubview(dummy);

            var wrapper = new UIView();
            wrapper.TranslatesAutoresizingMaskIntoConstraints = false;

            var mainImg = UIImage.FromBundle("checkin_01");

            nfloat mainImgAspectRatio = mainImg.Size.Height / mainImg.Size.Width;

            var mainImageView = new UIImageView();
            mainImageView.Image = mainImg;
            mainImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            mainImageView.TranslatesAutoresizingMaskIntoConstraints = false;

            var pointsImg = UIImage.FromBundle("checkin_02");

            nfloat pointsImgAspectRatio = pointsImg.Size.Width / pointsImg.Size.Height;

            var pointsImageView = new UIImageView();
            pointsImageView.Image = pointsImg;
            pointsImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            pointsImageView.TranslatesAutoresizingMaskIntoConstraints = false;

            wrapper.AddSubview(mainImageView);
            wrapper.AddSubview(pointsImageView);

            wrapper.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(wrapper, NSLayoutAttribute.Height, NSLayoutRelation.Equal, mainImageView, NSLayoutAttribute.Height, 1, 5),

                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Top, 1, 5),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Width, 1, -20),
                NSLayoutConstraint.Create(mainImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, mainImageView, NSLayoutAttribute.Width, mainImgAspectRatio, 0),

                NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Height, 0.49f, 0),
                NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Trailing, 1, 0),
                NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, wrapper, NSLayoutAttribute.Bottom, 1, -20),
                NSLayoutConstraint.Create(pointsImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, pointsImageView, NSLayoutAttribute.Height, pointsImgAspectRatio, 0)
            });

            var welcomeMessage = UICommon.CreateLabel(Fonts.TigerBasic.WithSize(30), UIColor.Black);
            welcomeMessage.AttributedText = new NSAttributedString(Constants.Strings.CheckInMissionPageMessage,
                new UIStringAttributes { ParagraphStyle = new NSMutableParagraphStyle { LineSpacing = 1f, LineHeightMultiple = 0.7f, Alignment = UITextAlignment.Center } });

            var completeCommand = UICommon.CreateButton("Completa la missione");
            //PresentViewController(new CheckInMissionTooFarViewController(), true, null);
            completeCommand.TouchUpInside += CompleteCommand_TouchUpInside;

            mainStack.AddArrangedSubview(topStripe);
            mainStack.AddArrangedSubview(navStack);
            mainStack.AddArrangedSubview(wrapper);
            mainStack.AddArrangedSubview(welcomeMessage);
            mainStack.AddArrangedSubview(completeCommand);
            mainStack.AddArrangedSubview(new UIView());

            View.Add(mainStack);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, View, NSLayoutAttribute.Height, 1, 0),

                NSLayoutConstraint.Create(topStripe, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),
                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topStripe, NSLayoutAttribute.Bottom, 1, -44),
                NSLayoutConstraint.Create(navStack, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 44),

                NSLayoutConstraint.Create(title, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, navStack, NSLayoutAttribute.Bottom, 1, 8),

                NSLayoutConstraint.Create(wrapper, NSLayoutAttribute.Top, NSLayoutRelation.Equal, navStack, NSLayoutAttribute.Bottom, 1, 0),
                NSLayoutConstraint.Create(wrapper, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(welcomeMessage, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(completeCommand, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -10),
                NSLayoutConstraint.Create(completeCommand, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -20),
            });
        }
    }
}