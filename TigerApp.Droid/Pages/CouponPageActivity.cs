
using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using TigerApp.Droid.UI.Coupons;
using TigerApp.Droid.UI.ToolTips;
using TigerApp.Shared;
using TigerApp.Shared.Models;
using TigerApp.Shared.Utils;
using TigerApp.Shared.Utils.Coupon;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "CouponPageActivity")]
    public class CouponPageActivity : BaseReactiveActivity<ICouponPageViewModel>
    {
        public const string IN_DATA = "IN_DATA";

        private DynamicLayout _couponNumbersView;
        private ViewPager _vPager;
        private RelativeLayout _couponPagerPlaceholderContainer;
        private ImageView _ivQuit;
        private RelativeLayout _tvCouponSpecialiContainer;
        private ImageView _btnInfo;
        private Action _qrDialogAction;

        public CouponPageActivity()
        {
            this.WhenActivated(dispose =>
            {
                dispose(ViewModel.WhenAnyValue(vm => vm.ShouldShowTutorial).Where(show => show == true).Subscribe((obj) =>
                {
                    ShowTutorial();
                }));
                dispose(ViewModel.WhenAnyValue(vm => vm.SpecialCoupons).Subscribe((coupons) =>
                {
                    PrepareCouponPager(coupons);
                }));
                dispose(ViewModel.WhenAnyValue(vm => vm.Coupons).Subscribe((coupons) =>
                {
                    var amount = coupons != null && coupons.Count > 0 ? (int)coupons[0].Amount : 0;
                    ((DynamicLayoutAdapter)_couponNumbersView.Adapter).NotifyStateChanged(amount);
                }));
                ViewModel.GetCoupons();
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            InitViews();
        }

        private void InitViews()
        {
            SetContentView(Resource.Layout.activity_coupon_page);

            _qrDialogAction = () =>
            {
                if (ViewModel.Coupons != null && ViewModel.Coupons.Count > 0)
                {
                    var transaction = FragmentManager.BeginTransaction();
                    transaction.SetTransition(FragmentTransit.FragmentOpen);
                    transaction.Add(Android.Resource.Id.Content, new CouponPopupFragment(ViewModel.Coupons[0].Id,(int)ViewModel.Coupons[0].Amount))
                               .AddToBackStack(null)
                               .Commit();
                }
            };

            _tvCouponSpecialiContainer = FindViewById<RelativeLayout>(Resource.Id.tvCouponSpecialiContainer);

            _btnInfo = FindViewById<ImageView>(Resource.Id.btnInfo);
            _btnInfo.Click += (sender, e) => ShowTutorial();

            _ivQuit = FindViewById<ImageView>(Resource.Id.ivQuit);
            _ivQuit.Click += (sender, e) => Finish();

            _vPager = FindViewById<ViewPager>(Resource.Id.rlCouponViewpager);
            _vPager.PageSelected += (sender, e) => ((DynamicLayoutAdapter)_couponNumbersView.Adapter).NotifyStateChanged((int)ViewModel.Coupons[0].Amount);

            _couponPagerPlaceholderContainer = FindViewById<RelativeLayout>(Resource.Id.rlCouponViewpagerPlaceholder);
            _couponNumbersView = FindViewById<DynamicLayout>(Resource.Id.couponNumbersView);
            _couponNumbersView.Adapter = new DynamicLayoutAdapter(this, CouponNumberModel);
            _couponNumbersView.Click += (sender, e) => { _qrDialogAction(); };

            OnSwipeLeft += Finish;
        }

        private void PrepareCouponPager(IEnumerable<Coupon> coupons)
        {
            if (coupons != null && coupons.Count() > 0)
            {
                _couponPagerPlaceholderContainer.Visibility = ViewStates.Gone;
                _vPager.Visibility = ViewStates.Visible;
                var couponsModel = coupons.ToList();
                _vPager.Adapter = new CouponPagerAdapter(this, couponsModel, _qrDialogAction);
                //((DynamicLayoutAdapter)_couponNumbersView.Adapter).NotifyStateChanged((int)ViewModel.Coupons[0].Amount);
            }
            else
            {
                _couponPagerPlaceholderContainer.Visibility = ViewStates.Visible;
                _vPager.Visibility = ViewStates.Gone;
                //((DynamicLayoutAdapter)_couponNumbersView.Adapter).NotifyStateChanged(-1);
            }
        }

        private void ShowTutorial()
        {
            var tips = new List<SimpleTooltip>()
            {
                new TooltipBuilder(this)
                {
                    AnchorView = this._tvCouponSpecialiContainer,
                    Text = Resources.GetString(Resource.String.tut_coupon_s1),
                    Gravity = GravityFlags.Bottom,
                }
                .SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                new TooltipBuilder(this)
                {
                    AnchorView = this._couponNumbersView,
                    Gravity = GravityFlags.Top,
                }
                .SetContentView(Resource.Layout.tips_coupon_s2)
                .Build(),
            };

            ShowTipsAndSetFlagWhenFinish(tips, FlagStore, Constants.Flags.COUPON_PAGE_TUTORIAL_SHOWN);
        }

        private static List<CouponNumberModel> CouponNumberModel => GenerateDummyCouponNumbers();

        private static List<CouponNumberModel> GenerateDummyCouponNumbers()
        {
            var couponsNumber = new List<CouponNumberModel>();
            var couponManager = new CouponNumbersManager(new FibonacciNumberCalculator(3), 1);

            couponsNumber.Add(new CouponNumberModel(
                new int[] { Resource.Drawable.coupon_06, Resource.Drawable.coupon_06a, Resource.Drawable.coupon_06b },
                couponManager.Get(1)));

            couponsNumber.Add(new CouponNumberModel(
                new int[] { Resource.Drawable.coupon_07, Resource.Drawable.coupon_07a, Resource.Drawable.coupon_07b },
                couponManager.Get(2)));

            couponsNumber.Add(new CouponNumberModel(
                new int[] { Resource.Drawable.coupon_08, Resource.Drawable.coupon_08a, Resource.Drawable.coupon_08b },
                couponManager.Get(3)));

            couponsNumber.Add(new CouponNumberModel(
                new int[] { Resource.Drawable.coupon_09, Resource.Drawable.coupon_09a, Resource.Drawable.coupon_09b },
                couponManager.Get(4)));

            couponsNumber.Add(new CouponNumberModel(
                new int[] { Resource.Drawable.coupon_10, Resource.Drawable.coupon_10a, Resource.Drawable.coupon_10a },
                couponManager.Get(5)));

            return couponsNumber;
        }
    }
}

