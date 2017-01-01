using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace TigerApp.Droid.UI.Coupons
{
    public class DynamicLayoutAdapter : IViewCreator
    {
        private Context _context;
        public List<CouponNumberModel> _models { get; set; }

        public DynamicLayoutAdapter(Context context, List<CouponNumberModel> models)
        {
            _context = context;
            _models = models;
        }

        public View CreateView(int position) {
            var view = new ImageView(_context);
            var model = _models[position];
            var iconView = view as ImageView;
            if (view != null) {
                var layoutParams = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent);
                layoutParams.Weight = 1f;
                layoutParams.RightMargin = 7;
                layoutParams.LeftMargin = 7;
                iconView.LayoutParameters = layoutParams;
                model.StateChange.Subscribe(newState => {
                    iconView.SetImageResource(newState.IconSrc);
                });
                return iconView;
            }
            return new ImageView(_context);
        }

        public int GetCount() {
            return _models.Count;
        }

        public void NotifyStateChanged(int amount) {
            _models.ForEach(model => model.EvaluateSource(amount));
        }
    }
}

