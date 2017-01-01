using AD;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using TigerApp.Shared.Models;
using UIKit;

namespace TigerApp.iOS.Pages.Profile
{
    public class BadgeCollectionSource : UICollectionViewDataSource
    {
        public readonly List<Badge> Badges;

        public BadgeCollectionSource()
        {
            Badges = new List<Badge>();
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(BadgeViewCell.ReusableIdentifier, indexPath);
            return cell as UICollectionViewCell;
        }

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => Badges.Count();
    }
}