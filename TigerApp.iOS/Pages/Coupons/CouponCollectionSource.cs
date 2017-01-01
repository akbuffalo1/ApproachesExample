using System;
using System.Collections.Generic;
using AD;
using Foundation;
using TigerApp.Shared.Models;
using UIKit;
using System.Linq;

namespace TigerApp.iOS.Pages.Coupons
{
    public class CouponCollectionSource : UICollectionViewSource, IUICollectionViewDelegateFlowLayout
    {
        private readonly IEnumerable<Coupon> Coupons;

        public Coupon ElementAt(int i)
        {
            return Coupons.ElementAt(i);
        }

        public CouponCollectionSource(IEnumerable<Coupon> badges)
        {
            Coupons = badges;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(CouponCollectionViewCell.ReusableIdentifier, indexPath);
            return cell as UICollectionViewCell;
        }

        public override void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
        {
            cell.WithType<CouponCollectionViewCell>((c) => c.Bind(Coupons.ElementAt(indexPath.Row)));
        }

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => Coupons.Count();
    }
}