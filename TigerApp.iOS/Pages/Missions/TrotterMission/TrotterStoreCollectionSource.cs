using System;
using System.Collections.Generic;
using AD;
using Foundation;
using TigerApp.Shared.Models;
using UIKit;
using System.Linq;

namespace TigerApp.iOS.Pages.Missions.TrotterMission
{
    public class TrotterStoreCollectionSource : UICollectionViewDataSource, IUICollectionViewDelegateFlowLayout
    {
        public IEnumerable<Store> Stores { get; set; }

        public TrotterStoreCollectionSource()
        {
            Stores = new List<Store>();
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(TrotterStoreCollectionViewCell.ReusableIdentifier, indexPath);
            return cell as TrotterStoreCollectionViewCell;
        }

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => Stores.Count();
    }
}