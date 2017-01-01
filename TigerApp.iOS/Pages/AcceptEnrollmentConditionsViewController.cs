using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    public partial class AcceptEnrollmentConditionsViewController : BaseReactiveViewController<IAcceptEnrollmentConditionsViewModel>
    {
        public AcceptEnrollmentConditionsViewController()
        {
            ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        partial void onAccetoClick(Foundation.NSObject sender)
        {
            PresentViewController(new ExpHome.ExpHomeViewController(), true, null);
        }
    }
}