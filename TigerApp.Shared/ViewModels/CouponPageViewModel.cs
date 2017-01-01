using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI.Fody.Helpers;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;

namespace TigerApp.Shared.ViewModels
{
    public interface ICouponPageViewModel : IViewModelBase
    {
        List<Coupon> Coupons { get; }
        List<Coupon> SpecialCoupons { get; }
        bool ShouldShowTutorial { get; }
        void GetCoupons();
    }

    public class CouponPageViewModel : ReactiveViewModel, ICouponPageViewModel
    {
        [Reactive]
        public List<Coupon> Coupons
        {
            get; private set;
        }

        [Reactive]
        public List<Coupon> SpecialCoupons
        {
            get; private set;
        }

        public bool ShouldShowTutorial => !FlagStore.IsSet(Constants.Flags.COUPON_PAGE_TUTORIAL_SHOWN);

        private readonly ICouponApiService _couponApi;

        public CouponPageViewModel(ICouponApiService couponApi)
        {
            _couponApi = couponApi;
        }

        public void GetCoupons()
        {
            _couponApi.GetCoupons()
                      .SubscribeOnce(coupons => {
                          SpecialCoupons = coupons != null ? coupons?.Where((Coupon c) => c.Special)?.ToList() : new List<Coupon>();
                       });
            _couponApi.GetDisposableCoupons()
                      .SubscribeOnce(coupons =>
                      {
                          Coupons = coupons != null ? coupons.OrderBy((Coupon coupon) => coupon.Amount).Reverse().ToList() : new List<Coupon>();
                      });
        }
    }
}
