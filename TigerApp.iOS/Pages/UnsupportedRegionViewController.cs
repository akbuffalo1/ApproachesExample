using Foundation;
using TigerApp.iOS.Utils;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages
{
    public partial class UnsupportedRegionViewController : BaseReactiveViewController<IUnsupportedRegionViewModel>
    {
        public override void ViewDidLoad()
        {
            bottomLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(32));
            base.View.AddGestureRecognizer(new UITapGestureRecognizer(OnViewClicked));
            CloseButton.TouchUpInside += (sender, e) => {
                DismissViewController(true, null);
            };
        }

        private void OnViewClicked()
        {
            //DismissViewController(true, null);
        }

        partial void onBackButtonClick(NSObject sender)
        {
            DismissViewController(true, null);
        }
    }
}
