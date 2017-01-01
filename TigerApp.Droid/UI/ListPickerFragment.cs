using Android.App;
using Android.OS;
using System;
using System.Collections.Generic;

namespace TigerApp.Droid.UI
{
    public class ListPickerFragment : DialogFragment
    {
        private List<string> _items;
        private Action<string> _itemSelectedAction = delegate { };

        public static ListPickerFragment NewInstance(List<string> items, Action<string> onItemSelected)
        {
            ListPickerFragment fragment = new ListPickerFragment(items, onItemSelected);
            return fragment;
        }

        public ListPickerFragment(List<string> items, Action<string> onItemSelected)
        {
            _items = items;
            _itemSelectedAction = onItemSelected;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            builder.SetItems(_items.ToArray(), (s, e) =>
            {
                _itemSelectedAction(_items[e.Which]);
                Dismiss();
            });
            return builder.Create();
        }
    }
}