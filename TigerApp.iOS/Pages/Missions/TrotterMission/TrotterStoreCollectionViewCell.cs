using AD.iOS;
using CoreGraphics;
using Foundation;
using System;
using TigerApp.iOS.Utils;
using TigerApp.Shared.Models;
using UIKit;

namespace TigerApp.iOS.Pages.Missions.TrotterMission
{
    public partial class TrotterStoreCollectionViewCell : BaseCollectionViewCell<Store>
    {
        private const string Name = nameof(TrotterStoreCollectionViewCell);

        public static readonly UINib Nib = UINib.FromName(Name, NSBundle.MainBundle);
        public static readonly NSString Key = new NSString(Name);
        public static readonly NSString ReusableIdentifier = (NSString)Name;

        public static CGSize Size;
        public static float CityLabelFontSize;
        public static float StreetLabelFontSize;

        static TrotterStoreCollectionViewCell()
        {
            DeviceHelper.OnIphone5(() =>
            {
                Size = new CGSize(100, 120);
                CityLabelFontSize = 26;
                StreetLabelFontSize = 16;
            });

            DeviceHelper.OnIphone6P(() =>
            {
                Size = new CGSize(140, 160);
                CityLabelFontSize = 36;
                StreetLabelFontSize = 20;
            });

            DeviceHelper.OnIphone6(() =>
            {
                Size = new CGSize(120, 140);
                CityLabelFontSize = 30;
                StreetLabelFontSize = 18;
            });
        }

        public override void Bind(Store datum)
        {
            if (datum == null)
            {
                pinImage.Highlighted = true;
                cityLabel.Text = "...";
                cityLabel.Highlighted = true;
                streetLabel.Text = string.Empty;
            }
            else
            {
                pinImage.Highlighted = false;
                cityLabel.Highlighted = false;
                // TODO: Change this
                cityLabel.Text = datum.Location.City.Name;
                //to: cityLabel.Text = datum.location.region.city;
                streetLabel.Text = $"{datum.Address}";
            }
        }

        private void Initialize()
        {
            cityLabel.Font = Fonts.TigerBasic.WithSize(CityLabelFontSize);
            streetLabel.Font = Fonts.FrutigerRegular.WithSize(StreetLabelFontSize);
            dividerView.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("store_loc_05").Scale(new CGSize(5, 35)));
            ContentView.BackgroundColor = UIColor.Clear;
            BackgroundColor = UIColor.Clear;
            BackgroundView = null;
        }

        [Export("initWithFrame:")]
        public TrotterStoreCollectionViewCell(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            Initialize();
        }

        protected TrotterStoreCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}