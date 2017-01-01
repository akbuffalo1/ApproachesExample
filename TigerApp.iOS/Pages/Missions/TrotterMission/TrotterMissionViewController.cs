using AD;
using AD.iOS;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using System.Linq;
using TigerApp.iOS.Utils;
using TigerApp.Shared.ViewModels;
using UIKit;
using System.Reactive.Linq;
using System;

namespace TigerApp.iOS.Pages.Missions.TrotterMission
{
    public partial class TrotterMissionViewController : BaseReactiveViewController<TigerTrotterMissionViewModel>, IUICollectionViewDelegate
    {
        private TrotterStoreCollectionSource collectionSource { get; set; }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return TrotterStoreCollectionViewCell.Size;
        }

        [Export("collectionView:willDisplayCell:forItemAtIndexPath:")]
        public void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, Foundation.NSIndexPath indexPath)
        {
            cell.WithType<TrotterStoreCollectionViewCell>((c) => c.Bind(collectionSource.Stores.ElementAt(indexPath.Row)));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            titleLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerCandy.WithSize(32), textAlignment: UITextAlignment.Left, lineHeight: 33);

            DeviceHelper.OnIphone5(() =>
            {
                descriptionLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(26), lineHeight: 24);
            });

            DeviceHelper.OnIphone6(() =>
            {
                descriptionLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(32), lineHeight: 30);
            });

            DeviceHelper.OnIphone6P(() =>
            {
                descriptionLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(36), lineHeight: 35);
            });

            SetupStoresCollectionView();

            backButton.TouchUpInside += delegate { DismissViewController(true, null); };
        }

        private void SetupStoresCollectionView()
        {
            storesCollectionView.Bounces = false;
            storesCollectionView.AllowsSelection = false;
            storesCollectionView.BackgroundView = null;
            storesCollectionView.RegisterNibForCell(TrotterStoreCollectionViewCell.Nib, TrotterStoreCollectionViewCell.ReusableIdentifier);
        }

        private void SetupStoresCollectionSource()
        {
            collectionSource = new TrotterStoreCollectionSource();
            storesCollectionView.Delegate = this;
            storesCollectionView.DataSource = collectionSource;
            var vmStores = ViewModel.StoreList.ToList();

            for (var i = vmStores.Count; i < 5; i += 1)
            {
                vmStores.Add(null);
            }

            collectionSource.Stores = vmStores;
            storesCollectionView.ReloadData();
        }

        public TrotterMissionViewController()
        {
            this.WhenActivated(dis =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.StoreList).Where(stores => stores != null).Subscribe((stores) =>
                {
                    SetupStoresCollectionSource();
                }));
                ViewModel.GetStoresCheckIn();
            });
        }
    }
}