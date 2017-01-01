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
    [Register ("CouponCollectionViewCell")]
    partial class CouponCollectionViewCell
    {
        [Outlet]
        UIKit.UIImageView couponImage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (couponImage != null) {
                couponImage.Dispose ();
                couponImage = null;
            }
        }
    }
}