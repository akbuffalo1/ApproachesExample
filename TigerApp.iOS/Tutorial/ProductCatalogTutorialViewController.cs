using Foundation;
using System;
using TigerApp.iOS.Utils;
using UIKit;
using TigerApp.Shared;

namespace TigerApp.iOS.Tutorial
{
    [Register("ProductCatalogTutorialViewController")]
    public class ProductCatalogTutorialViewController : UIViewController
    {
        private readonly Action OnTutorialIscrivimiClicked;
        private readonly Action OnDismiss;

        public ProductCatalogTutorialViewController(Action onTutorialIscrivimiClicked = null, Action onDismiss = null)
        {
            OnDismiss = onDismiss;
            OnTutorialIscrivimiClicked = onTutorialIscrivimiClicked;
            ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
            View.BackgroundColor = Colors.SemiTransparentBlack;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var mainStack = UICommon.CreateStackView();
            mainStack.LayoutMargins = new UIEdgeInsets(0, 5, 0, 5);
            mainStack.LayoutMarginsRelativeArrangement = true;

            var img1 = UIImage.FromBundle("product_catalog_popup01");

            nfloat img1AspectRatio = img1.Size.Height / img1.Size.Width;

            var image1 = new UIImageView();
            image1.Image = img1;
            image1.ContentMode = UIViewContentMode.ScaleAspectFit;
            image1.TranslatesAutoresizingMaskIntoConstraints = false;

            var img2 = UIImage.FromBundle("product_catalog_popup02");

            nfloat img2AspectRatio = img2.Size.Height / img2.Size.Width;

            var image2 = new UIImageView();
            image2.Image = img2;
            image2.TranslatesAutoresizingMaskIntoConstraints = false;
            image2.ContentMode = UIViewContentMode.ScaleAspectFit;
            image2.UserInteractionEnabled = true;
            image2.AddGestureRecognizer(new UITapGestureRecognizer(() => { DismissViewController(true, null); }));

            var img3 = UIImage.FromBundle("product_catalog_popup03");

            nfloat img3AspectRatio = img3.Size.Height / img3.Size.Width;

            var image3 = new UIImageView();
            image3.Image = img3;
            image3.TranslatesAutoresizingMaskIntoConstraints = false;
            image3.ContentMode = UIViewContentMode.ScaleAspectFit;
            image3.UserInteractionEnabled = true;
            image3.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, () =>
                {
                    OnTutorialIscrivimiClicked?.Invoke();
                });
            }));

            var horizontalStack = UICommon.CreateStackView(UILayoutConstraintAxis.Horizontal, UIStackViewDistribution.FillEqually, UIStackViewAlignment.Bottom, 10);

            horizontalStack.AddArrangedSubview(image2);
            horizontalStack.AddArrangedSubview(image3);

            mainStack.AddArrangedSubview(image1);
            mainStack.AddArrangedSubview(horizontalStack);

            View.Add(mainStack);

            View.AddConstraints(new NSLayoutConstraint[]
            {
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(mainStack, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, 0),

                NSLayoutConstraint.Create(image1, NSLayoutAttribute.Height, NSLayoutRelation.Equal, image1, NSLayoutAttribute.Width, img1AspectRatio, 0),
                NSLayoutConstraint.Create(image2, NSLayoutAttribute.Height, NSLayoutRelation.Equal, image2, NSLayoutAttribute.Width, img2AspectRatio, 0),
                NSLayoutConstraint.Create(image3, NSLayoutAttribute.Height, NSLayoutRelation.Equal, image3, NSLayoutAttribute.Width, img3AspectRatio, 0)
            });
        }

        public override void DismissViewController(bool animated, Action completionHandler)
        {
            var flagStore = AD.Resolver.Resolve<IFlagStoreService>();
            flagStore.Set(Constants.Flags.PRODUCTS_CATALOGUE_POPUP_SHOWN);
            base.DismissViewController(animated, () =>
            {
                completionHandler?.Invoke();
                OnDismiss?.Invoke();
            });
        }
    }
}