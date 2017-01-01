using System;
using CoreGraphics;
using Foundation;
using TigerApp.iOS.Utils;
using TigerApp.Shared.Models;
using UIKit;

namespace TigerApp.iOS.Pages.Coupons
{
    public partial class CouponCollectionViewCell : BaseCollectionViewCell<Coupon>
    {
        private const string Name = nameof(CouponCollectionViewCell);
        public static readonly UINib Nib = UINib.FromName(Name, NSBundle.MainBundle);
        public static readonly NSString Key = new NSString(Name);

        public static readonly NSString ReusableIdentifier = (NSString)Name;

        public override void Bind(Coupon datum)
        {

        }

        private void Initialize()
        {
            LayoutMargins = UIEdgeInsets.Zero;
            ContentView.LayoutMargins = UIEdgeInsets.Zero;
            BackgroundView = new UIView { BackgroundColor = UIColor.Clear };
        }

        [Export("initWithFrame:")]
        public CouponCollectionViewCell(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            Initialize();
        }

        protected CouponCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
