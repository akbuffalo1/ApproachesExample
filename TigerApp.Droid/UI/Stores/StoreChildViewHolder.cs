#region using

using System;
using Android.Views;
using Android.Widget;
using TigerApp.Droid.UI.ExpandableRecyclerView.ViewHolders;

#endregion

namespace TigerApp.Droid.UI.Stores
{
    public class StoreChildViewHolder : ChildViewHolder
    {
        public View view;
        public TextView txtAddress;
        public TextView txtCaption;
        public TextView txtInfo;

        public StoreChildViewHolder(View itemView) : base(itemView)
        {
            view = ItemView;
            txtCaption = itemView.FindViewById<TextView>(Resource.Id.txtCaption);
            txtAddress = itemView.FindViewById<TextView>(Resource.Id.txtAddress);
            txtInfo = itemView.FindViewById<TextView>(Resource.Id.txtInfo);
        }
    }
}