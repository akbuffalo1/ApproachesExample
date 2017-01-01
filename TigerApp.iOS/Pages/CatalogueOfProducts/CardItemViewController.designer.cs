// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace TigerApp.iOS.Pages.CatalogueOfProducts
{
    [Register ("CardItemViewController")]
    partial class CardItemViewController
    {
        [Outlet]
        UIKit.UIButton backButton { get; set; }

        [Outlet]
        UIKit.UIButton DislikeButton { get; set; }

        [Outlet]
        UIKit.UIButton facebookShare { get; set; }

        [Outlet]
        UIKit.UIImageView ImageUrl { get; set; }

        [Outlet]
        UIKit.UIButton LikeButton { get; set; }

        [Outlet]
        UIKit.UILabel ProductPriceLabel { get; set; }

        [Outlet]
        UIKit.UILabel ProductTitleLabel { get; set; }

        [Outlet]
        UIKit.UIImageView PuntoImage { get; set; }

        [Outlet]
        UIKit.UIButton tutorialButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (backButton != null) {
                backButton.Dispose ();
                backButton = null;
            }

            if (DislikeButton != null) {
                DislikeButton.Dispose ();
                DislikeButton = null;
            }

            if (facebookShare != null) {
                facebookShare.Dispose ();
                facebookShare = null;
            }

            if (ImageUrl != null) {
                ImageUrl.Dispose ();
                ImageUrl = null;
            }

            if (LikeButton != null) {
                LikeButton.Dispose ();
                LikeButton = null;
            }

            if (ProductPriceLabel != null) {
                ProductPriceLabel.Dispose ();
                ProductPriceLabel = null;
            }

            if (ProductTitleLabel != null) {
                ProductTitleLabel.Dispose ();
                ProductTitleLabel = null;
            }

            if (PuntoImage != null) {
                PuntoImage.Dispose ();
                PuntoImage = null;
            }

            if (tutorialButton != null) {
                tutorialButton.Dispose ();
                tutorialButton = null;
            }
        }
    }
}