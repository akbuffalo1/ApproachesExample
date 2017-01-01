#region using

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using TigerApp.Droid.UI.ExpandableRecyclerView.Adapters;
using TigerApp.Droid.UI.ExpandableRecyclerView.Models;
using TigerApp.Shared.Models;

#endregion

namespace TigerApp.Droid.UI.Stores
{
    public class StoresListAdapter : ExpandableRecyclerAdapter<StoreParentViewHolder, StoreChildViewHolder>
    {
        public event Action<StoreListChildItemModel> OnChildClickEvent;

        private readonly LayoutInflater _inflater;

        public StoresListAdapter(Context context, List<IParentObject> itemList) : base(context, itemList)
        {
            _inflater = LayoutInflater.From(context);
        }

        #region implemented abstract members of ExpandableRecyclerAdapter

        public override StoreParentViewHolder OnCreateParentViewHolder(ViewGroup parentViewGroup)
        {
            var view = _inflater.Inflate(Resource.Layout.item_store_parent, parentViewGroup, false);
            return new StoreParentViewHolder(view);
        }

        public override StoreChildViewHolder OnCreateChildViewHolder(ViewGroup childViewGroup)
        {
            var view = _inflater.Inflate(Resource.Layout.item_store_child, childViewGroup, false);
            return new StoreChildViewHolder(view);
        }

        public override void OnBindParentViewHolder(StoreParentViewHolder parentViewHolder, int position,
            object parentObject)
        {
            var storeModel = (StoreListItemModel) parentObject;
            parentViewHolder.txtCaption.Text = storeModel.Title;
        }

        public override void OnBindChildViewHolder(StoreChildViewHolder childViewHolder, int position,
            object childObject)
        {
            var storeModel = (StoreListChildItemModel) childObject;
            childViewHolder.txtCaption.Text = storeModel.Title;
            childViewHolder.txtAddress.Text = storeModel.Contact;
            childViewHolder.txtInfo.Text = storeModel.WorkingHours;

            childViewHolder.view.Click += (sender, args) => { OnChildItemClick(storeModel); };
        }

        private void OnChildItemClick(StoreListChildItemModel storeModel)
        {
            OnChildClickEvent?.Invoke(storeModel);
        }

        #endregion
    }
}