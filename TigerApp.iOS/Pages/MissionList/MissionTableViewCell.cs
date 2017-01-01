using AD.Views.iOS;
using CoreGraphics;
using Foundation;
using System;
using TigerApp.iOS.Utils;
using TigerApp.Shared.Models;
using UIKit;

namespace TigerApp.iOS.Pages.MissionList
{
    public partial class MissionTableViewCell : BaseTableViewCell<Objective>
    {
        private const string Name = nameof(MissionTableViewCell);
        public static readonly UINib Nib = UINib.FromName(Name, NSBundle.MainBundle);
        public static readonly NSString Key = new NSString(Name);

        public static readonly NSString ReusableIdentifier = (NSString)Name;

        static MissionTableViewCell()
        {
            Nib = UINib.FromName("MissionTableViewCell", NSBundle.MainBundle);
        }

        private void Initialize()
        {
            BackgroundView = new UIView { BackgroundColor = UIColor.Clear };
        }

        [Export("initWithFrame:")]
        public MissionTableViewCell(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            Initialize();
        }

        public override void Bind(Objective objective)
        {
            checkmarkImageView.Hidden = !objective.Completed;
            pointsLabel.Text = $"{objective.PrizeUnits}pp";
            missionDescriptionLabel.Text = objective.Description;

            (imageView as ADImageView).ImageUrl = AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + objective.ImageUrl;
        }

        protected MissionTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
