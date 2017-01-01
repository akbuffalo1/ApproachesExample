using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Java.Util;

namespace TigerApp.Droid.UI
{
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        const string BUNDLE_KEY_DATE = nameof(BUNDLE_KEY_DATE);
        private Action<DateTimeOffset> _dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(DateTimeOffset? selected, Action<DateTimeOffset> onDateSelected)
        {
            DatePickerFragment fragment = new DatePickerFragment();
            fragment._dateSelectedHandler = onDateSelected;
            Bundle bundle = new Bundle();
            var calendar = Calendar.Instance;
            if (selected.HasValue) 
            { 
                calendar.Set(CalendarField.Year, selected.Value.Year);
                calendar.Set(CalendarField.Month, selected.Value.Month - 1);
                calendar.Set(CalendarField.DayOfMonth, selected.Value.Day);
            }
            bundle.PutSerializable(BUNDLE_KEY_DATE, calendar);
            fragment.Arguments = bundle;
            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Calendar current = (Calendar) Arguments.GetSerializable(BUNDLE_KEY_DATE);
            var year = current.Get(CalendarField.Year);
            var month = current.Get(CalendarField.Month);
            var day = current.Get(CalendarField.DayOfMonth);
            return new DatePickerDialog(Activity, AlertDialog.ThemeHoloLight, this, year, month, day);
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            _dateSelectedHandler(DateTimeOffset.Parse(string.Format("{0}/{1}/{2}", dayOfMonth, monthOfYear + 1, year)));
            _dateSelectedHandler = null;
            Dismiss();
        }
    }
}
