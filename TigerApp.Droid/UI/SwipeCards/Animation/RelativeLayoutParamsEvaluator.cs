using Android.Animation;
using LayoutParams = Android.Widget.RelativeLayout.LayoutParams;
using Object = Java.Lang.Object;

namespace TigerApp.Droid.UI.SwipeCards.Animation
{
    using CardUtils = CardUtils;

    public class RelativeLayoutParamsEvaluator : Java.Lang.Object, ITypeEvaluator
    {
        public Object Evaluate(float fraction, Object startValue, Object endValue)
        {
            var start = (LayoutParams)startValue;
            var end = (LayoutParams)endValue;
            LayoutParams result = CardUtils.CloneParams(start);
            result.LeftMargin += (int)((end.LeftMargin - start.LeftMargin) * fraction);
            result.RightMargin += (int)((end.RightMargin - start.RightMargin) * fraction);
            result.TopMargin += (int)((end.TopMargin - start.TopMargin) * fraction);
            result.BottomMargin += (int)((end.BottomMargin - start.BottomMargin) * fraction);
            return result;
        }
    }

}