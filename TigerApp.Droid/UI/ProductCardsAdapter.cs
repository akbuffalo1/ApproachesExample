using System;
using System.Collections.Generic;
using AD.Views.Android;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using TigerApp.Droid.UI.SwipeCards;
using TigerApp.Shared.Models;

namespace TigerApp.Droid.UI
{

    public class ProductCardsAdapter : CardStackAdapter, ICardActionDispatcher
    {
        //TODO with RecycleAdapter

        public override void BindView(int position, View convertView, ViewGroup parent)
        {
            var pm = GetItem(position);
            ((ProductCard)convertView).Product = pm;
            TextView txtProductName = convertView.FindViewById<TextView>(Resource.Id.txtProductName);
            txtProductName.SetText(pm.Name, TextView.BufferType.Normal);
            TextView txtProductPrice = convertView.FindViewById<TextView>(Resource.Id.txtProductPrice);
            //TODO get and add currency sign
            txtProductPrice.SetText(pm.Price.ToString() + " €", TextView.BufferType.Normal);

            var imgPunto = convertView.FindViewById<ImageView>(Resource.Id.imgPunto);
            var imgProduct = convertView.FindViewById<ADImageView>(Resource.Id.imgProduct);
            //TODO link from model broken
            imgProduct.ImageUrl = AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + pm.Image;
            imgProduct.ErrorImagePath = "res:ic_error_loading_image";
            //            imgProduct.DefaultImagePath = "res:img_test_product";
            var btnLike = convertView.FindViewById<ImageButton>(Resource.Id.btnLike);
            var btnDislike = convertView.FindViewById<ImageButton>(Resource.Id.btnDislike);
            var btnLikeOver = convertView.FindViewById<ImageView>(Resource.Id.btnLikeOver);
            var btnDislikeOver = convertView.FindViewById<ImageView>(Resource.Id.btnDislikeOver);
            btnLikeOver.Visibility = ViewStates.Gone;
            btnLikeOver.Alpha = 0;
            btnLike.Clickable = true;
            btnDislikeOver.Visibility = ViewStates.Gone;
            btnDislikeOver.Alpha = 0;
            btnDislike.Clickable = true;
            imgPunto.Visibility = ViewStates.Invisible;

            btnLike.Invalidate();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = ((ProductCard)convertView);
            view.OnCardActionEvent += (vote, product) =>
            {
                OnCardActionEvent?.Invoke(vote, product);
            };

            view.OnCardSwipeActionEvent += (vote, product) =>
            {
                OnCardSwipeActionEvent?.Invoke(vote, product);
            };

            return convertView;
        }


        public event Action<ProductVote, Product> OnCardActionEvent;
        public event Action<ProductVote, Product> OnCardSwipeActionEvent;

        public ProductCardsAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public ProductCardsAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
        }

        public ProductCardsAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
        }

        public ProductCardsAdapter(Context context, int textViewResourceId, Product[] objects) : base(context, textViewResourceId, objects)
        {
        }

        public ProductCardsAdapter(Context context, int resource, int textViewResourceId, Product[] objects) : base(context, resource, textViewResourceId, objects)
        {
        }

        public ProductCardsAdapter(Context context, int textViewResourceId, IList<Product> objects) : base(context, textViewResourceId, objects)
        {
        }

        public ProductCardsAdapter(Context context, int resource, int textViewResourceId, IList<Product> objects) : base(context, resource, textViewResourceId, objects)
        {
        }
    }
}