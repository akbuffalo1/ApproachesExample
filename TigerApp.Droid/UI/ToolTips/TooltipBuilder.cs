#region using

using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using System;
using TigerApp.Shared;

#endregion

namespace TigerApp.Droid.UI.ToolTips
{
    public class TooltipBuilder
    {
        private static readonly int DefaultTextAppearanceRes = Resource.Style.simpletooltip_default;
        private static readonly int DefaultBackgroundColorRes = Resource.Color.simpletooltip_background;
        private static readonly int DefaultTextColorRes = Resource.Color.simpletooltip_text;
        private static readonly int DefaultArrowColorRes = Resource.Color.simpletooltip_arrow;
        private static readonly int DefaultMarginRes = Resource.Dimension.simpletooltip_margin;
        private static readonly int DefaultPaddingRes = Resource.Dimension.simpletooltip_padding;
        private static readonly int DefaultAnimationPaddingRes = Resource.Dimension.simpletooltip_animation_padding;
        private static readonly int DefaultAnimationDurationRes = Resource.Integer.simpletooltip_animation_duration;
        private static readonly int DefaultSideMarginRes = Resource.Dimension.simpletooltip_side_margin;

        internal readonly Context Context;

        public View AnchorView;
        public bool Animated;
        public long AnimationDuration;
        public float AnimationPadding = -1;
        public int ArrowColor;
        public View RootView;
        public float Adding = -1;
        public Drawable ArrowDrawable;
        public int BackgroundColor;
        public View ContentView;
        public bool DismissOnInsideTouch = true;
        public bool DismissOnOutsideTouch = true;
        public bool DrawOval = true;
        public GravityFlags Gravity = GravityFlags.Bottom;
        public float Margin = -1;
        public float MaxWidth;
        public bool Modal = true;
        public SimpleTooltip.IOnDismissListener OnDismissListener;
        public SimpleTooltip.IOnShowListener OnShowListener;
        public bool ShowArrow = true;
        public string Text = "";
        public int TextColor;
        public Rect SidesMargin;

        internal int TextViewId = Android.Resource.Id.Text1;
        public bool TransparentOverlay = false;

        public TooltipBuilder(Context context)
        {
            Context = context;
        }

        public virtual SimpleTooltip Build()
        {
            ValidateArguments();

            if (BackgroundColor == 0)
                BackgroundColor = SimpleTooltipUtils.GetColor(Context, DefaultBackgroundColorRes);

            if (TextColor == 0)
                TextColor = SimpleTooltipUtils.GetColor(Context, DefaultTextColorRes);

            if (ContentView == null)
            {
                var tv = new TextView(Context);
                SimpleTooltipUtils.SetTextAppearance(tv, DefaultTextAppearanceRes);
                tv.SetBackgroundColor(new Color(BackgroundColor));
                tv.SetTextColor(new Color(TextColor));
                ContentView = tv;
            }
            if (ArrowColor == 0)
                ArrowColor = SimpleTooltipUtils.GetColor(Context, DefaultArrowColorRes);

            if (Margin < 0)
                Margin = Context.Resources.GetDimension(DefaultMarginRes);

            if (Adding < 0)
                Adding = Context.Resources.GetDimension(DefaultPaddingRes);

            if (AnimationPadding < 0)
                AnimationPadding = Context.Resources.GetDimension(DefaultAnimationPaddingRes);

            if (AnimationDuration == 0)
                AnimationDuration = Context.Resources.GetInteger(DefaultAnimationDurationRes);

            if (SidesMargin == null)
            {
                var mar = (int)Context.Resources.GetDimension(DefaultSideMarginRes);
                SidesMargin = new Rect(mar, mar, mar, mar);
            }

            return new SimpleTooltip(this);
        }

        internal virtual void ValidateArguments()
        {
            if (Context == null)
            {
                throw new ArgumentException("Context not specified.");
            }
            if (AnchorView == null && RootView == null)
            {
                throw new ArgumentException("Anchor view or RootView not specified.");
            }
        }

        public virtual TooltipBuilder SetContentView(TextView textView)
        {
            ContentView = textView;
            TextViewId = 0;
            return this;
        }

        public virtual TooltipBuilder SetContentView(View contentView, int textViewId)
        {
            ContentView = contentView;
            TextViewId = textViewId;
            return this;
        }

        public virtual TooltipBuilder SetContentView(int contentViewId, int textViewId)
        {
            var inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            ContentView = inflater.Inflate(contentViewId, null, false);
            TextViewId = textViewId;
            return this;
        }

        public virtual TooltipBuilder SetContentView(int contentViewId)
        {
            var inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            ContentView = inflater.Inflate(contentViewId, null, false);
            TextViewId = 0;
            return this;
        }

        public TooltipBuilder CreateCouponBubble(string nickname, string message, int amount, bool isSpecial = false)
        {
            var inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            ContentView = inflater.Inflate(isSpecial ? Resource.Layout.tooltip_coupon_special : Resource.Layout.tooltip_coupon, null, false);

            if(isSpecial)
                ContentView.FindViewById<TextView>(Resource.Id.txt_tooltip_coupon_title).Text = string.Format(Constants.Strings.CouponTitle,nickname);
            ContentView.FindViewById<TextView>(Resource.Id.txt_tooltip_coupon_message).Text = message;

            if (!isSpecial)
            {
                ContentView.FindViewById<ImageView>(Resource.Id.img_tooltip_coupon_diamond).SetImageResource(amount == 3 ? Resource.Drawable.mechanics_01 :
                    amount == 8 ? Resource.Drawable.mechanics_04 : amount == 14 ? Resource.Drawable.mechanics_05 :
                    amount == 21 ? Resource.Drawable.mechanics_06 : Resource.Drawable.mechanics_07);

                if (amount != 3)
                {
                    ContentView.FindViewById(Resource.Id.v_tooltip_coupon_top_left).Visibility = ViewStates.Gone;
                    ContentView.FindViewById(Resource.Id.v_tooltip_coupon_top_right).Visibility = ViewStates.Gone;
                }
            }

            return this;
        }
    }
}