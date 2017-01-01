using System.Collections.Generic;
using TigerApp.Droid.UI.ExpandableRecyclerView.Models;

namespace TigerApp.Droid.UI.ExpandableRecyclerView.Adapters
{
    public class ExpandableRecyclerAdapterHelper
    {
        private const int InitialStableId = 0;
        private static int _currentId;

        public ExpandableRecyclerAdapterHelper(List<object> itemList)
        {
            _currentId = InitialStableId;
            HelperItemList = GenerateHelperItemList(itemList);
        }

        public List<object> HelperItemList { get; }

        public object GetHelperItemAtPosition(int position)
        {
            return HelperItemList[position];
        }

        public List<object> GenerateHelperItemList(List<object> itemList)
        {
            var parentWrapperList = new List<object>();
            foreach (var item in itemList)
            {
                if (item is IParentObject)
                {
                    var parentWrapper = new ParentWrapper(item, _currentId);
                    _currentId++;
                    parentWrapperList.Add(parentWrapper);
                }
                else
                {
                    parentWrapperList.Add(item);
                }
            }

            return parentWrapperList;
        }
    }
}