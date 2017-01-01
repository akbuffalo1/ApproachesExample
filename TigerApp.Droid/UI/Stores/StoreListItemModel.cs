#region using

using System;
using System.Collections.Generic;
using System.Linq;
using TigerApp.Droid.UI.ExpandableRecyclerView.Models;
using TigerApp.Shared.Models;

#endregion

namespace TigerApp.Droid.UI.Stores
{
    public class StoreListItemModel : IParentObject
    {
        public StoreListItemModel(string title, List<StoreListChildItemModel> stores)
        {
            Title = title;
            Stores = stores;
        }

        public string Title { get; set; }

        public List<StoreListChildItemModel> Stores
        {
            get { return _stores; }
            set
            {
                _stores = value;
                _childList = Stores.Cast<object>().ToList();
            }
        }

        private List<object> _childList;
        private List<StoreListChildItemModel> _stores;

        public List<object> ChildObjectList
        {
            get { return _childList; }
        }
    }
}