using TigerApp.iOS.Pages.ExpHome;
using UIKit;

namespace TigerApp.iOS.Services.Tutorial
{
    public class ExpHomeLevel1TutorialViewController : UIViewController
    {
        private readonly ExpHomeViewController homeViewCtrl;

        public ExpHomeLevel1TutorialViewController(ExpHomeViewController homeVc)
        {
            ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            homeViewCtrl = homeVc;
            View.BackgroundColor = new UIColor(0, 0, 0, 0.5F);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var step1 = new UIImageView(UIImage.FromBundle("exp_home_l1_bonus_message"))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            View.Add(step1);

            step1.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.8F).Active = true;
            step1.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor, -50).Active = true;
            step1.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;

            View.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
        }
    }
}