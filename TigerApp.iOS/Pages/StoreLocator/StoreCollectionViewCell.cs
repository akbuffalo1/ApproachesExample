using CoreGraphics;
using Foundation;
using System;
using TigerApp.iOS.Utils;
using TigerApp.Shared.Models;
using UIKit;

namespace TigerApp.iOS.Pages.StoreLocator
{
    public partial class StoreCollectionViewCell : BaseCollectionViewCell<Store>
    {
        private const string Name = nameof(StoreCollectionViewCell);

        public static readonly UINib Nib = UINib.FromName(Name, NSBundle.MainBundle);
        public static readonly NSString Key = new NSString(Name);
        public static readonly NSString ReusableIdentifier = (NSString)Name;

        public override void Bind(Store store)
        {
            cityLabel.Text = store.Location.City.Name;
            distanceLabel.Text = store.DistanceInMeters > 999
                    ? $"{((float)store.DistanceInMeters / 1000).ToString("n1")}km"
                    : $"{store.DistanceInMeters}m";
            if (distanceLabel.Text.EndsWith(",0km", StringComparison.CurrentCulture))
                distanceLabel.Text = distanceLabel.Text.Replace(",0", string.Empty); ;
            streetLabel.Text = store.Address;
        }

        private void Initialize()
        {
            ContentView.BackgroundColor = UIColor.Clear;
            BackgroundColor = UIColor.Clear;

            BackgroundView = new UIView { BackgroundColor = UIColor.White };
            SelectedBackgroundView = new UIView { BackgroundColor = Colors.ColorFromHexString("#E9E9E9") };
        }

        [Export("initWithFrame:")]
        public StoreCollectionViewCell(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            Initialize();
        }

        protected StoreCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
