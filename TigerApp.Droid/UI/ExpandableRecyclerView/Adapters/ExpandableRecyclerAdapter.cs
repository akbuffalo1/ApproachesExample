using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Newtonsoft.Json;
using TigerApp.Droid.UI.ExpandableRecyclerView.ClickListeners;
using TigerApp.Droid.UI.ExpandableRecyclerView.Models;
using TigerApp.Droid.UI.ExpandableRecyclerView.ViewHolders;

namespace TigerApp.Droid.UI.ExpandableRecyclerView.Adapters
{
    public abstract class ExpandableRecyclerAdapter<PVH, CVH> : RecyclerView.Adapter, IParentItemClickListener
        where PVH : ParentViewHolder
        where CVH : ChildViewHolder
    {
        private const int TypeParent = 0;
        private const int TypeChild = 1;
        private const string StableIdMap = "ExpandableRecyclerAdapter.StableIdMap";
        private const string StableIdList = "ExpandableRecyclerAdapter.StableIdList";

        public const int CustomAnimationViewNotSet = -1;
        public const long DefaultRotateDurationMs = 200;
        public const long CustomAnimationDurationNotSet = -1;
        private readonly ExpandableRecyclerAdapterHelper _adapterHelper;

        protected Context _context;
        private IExpandCollapseListener _expandCollapseListener;
        protected List<object> _itemList;
        protected List<IParentObject> _parentItemList;

        private Dictionary<long, bool> _stableIdMap;

        public override int ItemCount
        {
            get { return _itemList.Count; }
        }
       

        public bool ParentAndIconExpandOnClick { get; set; } = false;

        #region IParentItemClickListener implementation

        public void OnParentItemClickListener(int position)
        {
            if (_itemList[position] is IParentObject)
            {
                var parentObject = (IParentObject) _itemList[position];
                ExpandParent(parentObject, position);
            }
        }

        #endregion

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup viewGroup, int viewType)
        {
            if (viewType == TypeParent)
            {
                var pvh = OnCreateParentViewHolder(viewGroup);
                pvh.ParentItemClickListener = this;

                return pvh;
            }
            if (viewType == TypeChild)
            {
                return OnCreateChildViewHolder(viewGroup);
            }
            throw new ArgumentException("Invalid ViewType found");
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (_adapterHelper.GetHelperItemAtPosition(position) is ParentWrapper)
            {
                var parentViewHolder = (PVH) holder;

                parentViewHolder.SetMainItemClickToExpand();
                parentViewHolder.Expanded = ((ParentWrapper) _adapterHelper.GetHelperItemAtPosition(position)).Expanded;
                OnBindParentViewHolder(parentViewHolder, position, _itemList[position]);
            }
            else if (_itemList[position] == null)
            {
                throw new NullReferenceException("Incorrect ViewHolder found");
            }
            else
            {
                OnBindChildViewHolder((CVH) holder, position, _itemList[position]);
            }
        }

        private Dictionary<long, bool> GenerateStableIdMapFromList(List<object> itemList)
        {
            var parentObjectHashMap = new Dictionary<long, bool>();
            for (var i = 0; i < itemList.Count; i++)
            {
                if (itemList[i] != null)
                {
                    var parentWrapper = (ParentWrapper) _adapterHelper.GetHelperItemAtPosition(i);
                    parentObjectHashMap.Add(parentWrapper.StableId, parentWrapper.Expanded);
                }
            }

            return parentObjectHashMap;
        }

        private List<object> GenerateObjectList(List<IParentObject> parentObjectList)
        {
            var objectList = new List<object>();
            foreach (var parentObject in parentObjectList)
            {
                objectList.Add(parentObject);
            }

            return objectList;
        }

        public override int GetItemViewType(int position)
        {
            if (_itemList[position] is IParentObject)
            {
                return TypeParent;
            }
            if (_itemList[position] == null)
            {
                throw new NullReferenceException("Null object added");
            }
            return TypeChild;
        }

        public void AddExpandCollapseListener(IExpandCollapseListener expandCollapseListener)
        {
            _expandCollapseListener = expandCollapseListener;
        }

        private void ExpandParent(IParentObject parentObject, int position)
        {
            var parentWrapper = (ParentWrapper) _adapterHelper.GetHelperItemAtPosition(position);
            if (parentWrapper == null)
            {
                return;
            }

            if (parentWrapper.Expanded)
            {
                parentWrapper.Expanded = false;

                if (_expandCollapseListener != null)
                {
                    var expandedCountBeforePosition = GetExpandedItemCount(position);
                    _expandCollapseListener.OnRecyclerViewItemCollapsed(position - expandedCountBeforePosition);
                }

                // Was Java HashMap put, need to replace the value
                _stableIdMap[parentWrapper.StableId] = false;
                //_stableIdMap.Add(parentWrapper.StableId, false);
                var childObjectList = ((IParentObject) parentWrapper.ParentObject).ChildObjectList;
                if (childObjectList != null)
                {
                    for (var i = childObjectList.Count - 1; i >= 0; i--)
                    {
                        var pos = position + i + 1;
                        _itemList.RemoveAt(pos);
                        _adapterHelper.HelperItemList.RemoveAt(pos);
                        NotifyItemRemoved(pos);
                    }
                }
            }
            else
            {
                parentWrapper.Expanded = true;

                if (_expandCollapseListener != null)
                {
                    var expandedCountBeforePosition = GetExpandedItemCount(position);
                    _expandCollapseListener.OnRecyclerViewItemExpanded(position - expandedCountBeforePosition);
                }

                // Was Java HashMap put, need to replace the value
                _stableIdMap[parentWrapper.StableId] = true;
                //_stableIdMap.Add(parentWrapper.StableId, true);
                var childObjectList = ((IParentObject) parentWrapper.ParentObject).ChildObjectList;
                if (childObjectList != null)
                {
                    for (var i = 0; i < childObjectList.Count; i++)
                    {
                        var pos = position + i + 1;
                        _itemList.Insert(pos, childObjectList[i]);
                        _adapterHelper.HelperItemList.Insert(pos, childObjectList[i]);
                        NotifyItemInserted(pos);
                    }
                }
            }
        }

        private int GetExpandedItemCount(int position)
        {
            if (position == 0)
                return 0;

            var expandedCount = 0;
            for (var i = 0; i < position; i++)
            {
                var obj = _itemList[i];
                if (!(obj is IParentObject))
                    expandedCount++;
            }

            return expandedCount;
        }

        public Bundle OnSaveInstanceState(Bundle savedInstanceStateBundle)
        {
            savedInstanceStateBundle.PutString(StableIdMap, JsonConvert.SerializeObject(_stableIdMap));

            return savedInstanceStateBundle;
        }

        public void OnRestoreInstanceState(Bundle savedInstanceStateBundle)
        {
            if (savedInstanceStateBundle == null)
                return;

            if (!savedInstanceStateBundle.ContainsKey(StableIdMap))
                return;

            _stableIdMap =
                JsonConvert.DeserializeObject<Dictionary<long, bool>>(savedInstanceStateBundle.GetString(StableIdMap));
            var i = 0;

            while (i < _adapterHelper.HelperItemList.Count)
            {
                if (_adapterHelper.GetHelperItemAtPosition(i) is ParentWrapper)
                {
                    var parentWrapper = (ParentWrapper) _adapterHelper.GetHelperItemAtPosition(i);

                    if (_stableIdMap.ContainsKey(parentWrapper.StableId))
                    {
                        parentWrapper.Expanded = _stableIdMap[parentWrapper.StableId];
                        if (parentWrapper.Expanded)
                        {
                            var childObjectList = ((IParentObject) parentWrapper.ParentObject).ChildObjectList;
                            if (childObjectList != null)
                            {
                                for (var j = 0; j < childObjectList.Count; j++)
                                {
                                    i++;
                                    _itemList.Insert(i, childObjectList[j]);
                                    _adapterHelper.HelperItemList.Insert(i, childObjectList[j]);
                                }
                            }
                        }
                    }
                    else
                    {
                        parentWrapper.Expanded = false;
                    }
                }
                i++;
            }

            NotifyDataSetChanged();
        }

        public abstract PVH OnCreateParentViewHolder(ViewGroup parentViewGroup);

        public abstract CVH OnCreateChildViewHolder(ViewGroup childViewGroup);

        public abstract void OnBindParentViewHolder(PVH parentViewHolder, int position, object parentObject);

        public abstract void OnBindChildViewHolder(CVH childViewHolder, int position, object childObject);

        #region Constructors

        public ExpandableRecyclerAdapter(Context context, List<IParentObject> parentItemList)
            : this(context, parentItemList, CustomAnimationViewNotSet, DefaultRotateDurationMs)
        {
        }

        public ExpandableRecyclerAdapter(Context context, List<IParentObject> parentItemList,
            int customParentAnimationViewId)
            : this(context, parentItemList, customParentAnimationViewId, DefaultRotateDurationMs)
        {
        }

        public ExpandableRecyclerAdapter(Context context, List<IParentObject> parentItemList,
            int customParentAnimationViewId, long animationDuration)
        {
            _context = context;
            _parentItemList = parentItemList;
            _itemList = GenerateObjectList(parentItemList);
            _adapterHelper = new ExpandableRecyclerAdapterHelper(_itemList);
            _stableIdMap = GenerateStableIdMapFromList(_adapterHelper.HelperItemList);
        }

        #endregion
    }
}