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

namespace TigerApp.iOS.Pages.Coupons
{
    [Register ("CouponsViewController")]
    partial class CouponsViewController
    {
        [Outlet]
        UIKit.UILabel bottomTextLabel { get; set; }


        [Outlet]
        UIKit.UICollectionView CouponCollectionView { get; set; }


        [Outlet]
        UIKit.UIView CouponCollectionViewHolder { get; set; }


        [Outlet]
        UIKit.UIImageView couponSpecialiText { get; set; }


        [Outlet]
        UIKit.UIView couponViewHolder { get; set; }


        [Outlet]
        UIKit.UIView NoCouponsView { get; set; }


        [Outlet]
        UIKit.UIStackView prizeButtonStack { get; set; }


        [Action ("OnCloseButtonClick:")]
        partial void OnCloseButtonClick (Foundation.NSObject sender);


        [Action ("onInfoButtonClick:")]
        partial void onInfoButtonClick (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (bottomTextLabel != null) {
                bottomTextLabel.Dispose ();
                bottomTextLabel = null;
            }

            if (CouponCollectionView != null) {
                CouponCollectionView.Dispose ();
                CouponCollectionView = null;
            }

            if (CouponCollectionViewHolder != null) {
                CouponCollectionViewHolder.Dispose ();
                CouponCollectionViewHolder = null;
            }

            if (couponSpecialiText != null) {
                couponSpecialiText.Dispose ();
                couponSpecialiText = null;
            }

            if (couponViewHolder != null) {
                couponViewHolder.Dispose ();
                couponViewHolder = null;
            }

            if (NoCouponsView != null) {
                NoCouponsView.Dispose ();
                NoCouponsView = null;
            }

            if (prizeButtonStack != null) {
                prizeButtonStack.Dispose ();
                prizeButtonStack = null;
            }
        }
    }
}