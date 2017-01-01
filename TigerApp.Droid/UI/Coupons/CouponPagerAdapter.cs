using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using TigerApp.Shared.Models;

namespace TigerApp.Droid.UI.Coupons
{
    class CouponPagerAdapter : PagerAdapter
    {
        private List<Coupon> _models;
        private Context _context;
        private Action _onClick;

        public CouponPagerAdapter(Context context, List<Coupon> models, Action onClick)
        {
            _context = context;
            _models = models;
            _onClick = onClick;
        }

        public override int Count { get { return _models.Count; } }

        public override Java.Lang.Object InstantiateItem(ViewGroup collection, int position)
        {
            var image = new ImageView(_context);
            image.SetImageResource(Resource.Drawable.coupon_13_button);
            image.LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent
            );
            image.Click += delegate { _onClick(); };
            collection.AddView(image);
            return image;
        }

        public override void DestroyItem(ViewGroup collection, int position, Java.Lang.Object view)
        {
            collection.RemoveView((View)view);
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
        {
            return view == ((ImageView)objectValue);
        }
    }
}