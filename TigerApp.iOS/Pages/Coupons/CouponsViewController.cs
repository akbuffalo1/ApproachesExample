using AD;
using Foundation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using TigerApp.iOS.Utils;
using TigerApp.Shared.Models;
using TigerApp.Shared.Utils.Coupon;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.Coupons
{
    public partial class CouponsViewController : BaseReactiveViewController<ICouponPageViewModel>, IUICollectionViewDelegate, IUICollectionViewDelegateFlowLayout
    {
        public UIButton[] prizeButtons { get; set; }
        private CouponCollectionSource CouponCollectionViewSource { get; set; }
        private INumberCalculator couponCalculator { get; set; } = new FibonacciNumberCalculator(3);
        public UIImageView CouponSpecialiText => couponSpecialiText;

        protected List<Coupon> Coupons { get; private set; }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CoreGraphics.CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width, 120);
        }

        private readonly string[] ImageSources = new string[] {
            "coupon_06",
            "coupon_07",
            "coupon_08",
            "coupon_09",
            "coupon_10",
        };

        [Export("scrollViewDidScroll:")]
        public void Scrolled(UIScrollView scrollView)
        {
            couponIndex = ((int)Math.Ceiling(CouponCollectionView.ContentOffset.X / UIScreen.MainScreen.Bounds.Width))
                .Clamp(0, Coupons.Count() - 1);
            //UpdatePrizeButtons();
        }

        private int couponIndex = -1;
        private void UpdatePrizeButtons()
        {
            if (ViewModel.Coupons == null || ViewModel.Coupons.Count == 0)
                return;
            var coupon = ViewModel.Coupons[0];

            coupon.Amount = coupon.Amount > 29 ? 29 : coupon.Amount;

            for (var i = 0; i < prizeButtons.Length; i += 1)
            {
                var value = couponCalculator.Calculate(i + 1);
                var btn = prizeButtons[i];
                string imageSource = null;

                if (value < coupon.Amount)
                {
                    imageSource = $"{ImageSources[i]}b";
                }
                else if ((int)value == (int)coupon.Amount)
                {
                    imageSource = $"{ImageSources[i]}a";
                }
                else
                {
                    imageSource = ImageSources[i];
                }

                btn.SetBackgroundImage(UIImage.FromBundle(imageSource), UIControlState.Normal);
                btn.TouchUpInside += delegate { OpenQRCodeViewController(); };
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prizeButtons = prizeButtonStack.ArrangedSubviews.Cast<UIButton>().ToArray();
            bottomTextLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(30));

            couponViewHolder.Add(NoCouponsView);
            NoCouponsView.Hidden = true;
            NoCouponsView.TranslatesAutoresizingMaskIntoConstraints = true;
            NoCouponsView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

            CouponCollectionViewHolder.Hidden = true;
            CouponCollectionViewHolder.TranslatesAutoresizingMaskIntoConstraints = true;
            CouponCollectionViewHolder.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            couponViewHolder.Add(CouponCollectionViewHolder);
            CouponCollectionView.PagingEnabled = true;
            CouponCollectionView.RegisterNibForCell(CouponCollectionViewCell.Nib, CouponCollectionViewCell.ReusableIdentifier);
            CouponCollectionView.Delegate = this;
        }

        private void RefreshCoupons()
        {
            if (Coupons == null || Coupons.Count == 0)
            {
                NoCouponsView.Hidden = false;
                return;
            }

            CouponCollectionViewHolder.Hidden = false;
            CouponCollectionViewSource = new CouponCollectionSource(Coupons);
            CouponCollectionView.DataSource = CouponCollectionViewSource;
            CouponCollectionView.ReloadData();

            CouponCollectionView.ScrollToItem(NSIndexPath.FromRowSection(Coupons.Count - 1, 0), UICollectionViewScrollPosition.CenteredHorizontally, false);
            UpdatePrizeButtons();

            couponIndex = Coupons.Count - 1;
        }

        partial void OnCloseButtonClick(NSObject sender)
        {
            if (NavigationController != null)
                NavigationController.PopViewController(true);
            else
                DismissViewController(true, null);
        }

        public CouponsViewController()
        {
            TransitioningDelegate = TransitionManager.Left;

            this.WhenActivated(dis =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.SpecialCoupons).Subscribe(coupons =>
                {
                    if (coupons != null)
                    {
                        Coupons = coupons;
                        RefreshCoupons();
                    }
                }));
                dis(ViewModel.WhenAnyValue(vm => vm.Coupons).Subscribe(coupons =>
                {
                    if (coupons != null)
                    {
                        UpdatePrizeButtons();
                    }
                }));
                ViewModel.GetCoupons();
            });
        }

        partial void onInfoButtonClick(NSObject sender)
        {
            PresentViewController(new Tutorial.CouponTutorialViewController(this), true, null);
        }

        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath) => OpenQRCodeViewController();

        private void OpenQRCodeViewController()
        {
            PresentViewController(new CouponQRCodeViewController(ViewModel.Coupons[0].Id, (int)ViewModel.Coupons[0].Amount), true, null);
        }
    }
}