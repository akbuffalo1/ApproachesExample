#region using

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TigerApp.Shared.Models;

#endregion

namespace TigerApp.Droid.UI.SwipeCards
{
    //TODO refactor to RecyclerView.Adapter
    public abstract class CardStackAdapter : ArrayAdapter<Product>
    {
        public abstract void BindView(int position, View convertView, ViewGroup parent);

        public CardStackAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public CardStackAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
        }

        public CardStackAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
        }

        public CardStackAdapter(Context context, int textViewResourceId, Product[] objects) : base(context, textViewResourceId, objects)
        {
        }

        public CardStackAdapter(Context context, int resource, int textViewResourceId, Product[] objects) : base(context, resource, textViewResourceId, objects)
        {
        }

        public CardStackAdapter(Context context, int textViewResourceId, IList<Product> objects) : base(context, textViewResourceId, objects)
        {
        }

        public CardStackAdapter(Context context, int resource, int textViewResourceId, IList<Product> objects) : base(context, resource, textViewResourceId, objects)
        {
        }
    }
}