using System;
using System.Collections.Generic;
using System.Linq;
using AD.Views.Android;
using Android.App;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using TigerApp.Shared.Models;
using TigerApp.Shared.Services.API;

namespace TigerApp.Droid.Pages.MissionList
{
    class SpacingItemDecoration : RecyclerView.ItemDecoration
    {
        private int space;

        public SpacingItemDecoration(int space)
        {
            this.space = space;
        }

        public override void GetItemOffsets(Android.Graphics.Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            var lm = parent.GetLayoutManager() as LinearLayoutManager;
            var isVertical = lm.Orientation == LinearLayoutManager.Vertical;

            if (isVertical)
            {
                outRect.Bottom = space;
            }
            else {
                outRect.Bottom = space;
                outRect.Right = space;
            }
        }
    }

    class MissionViewHolder : RecyclerView.ViewHolder
    {
        public ADImageView LeftIcon
        {
            get;
            set;
        }

        public ImageView RightIcon
        {
            get;
            set;
        }

        public TextView PrizePoints
        {
            get;
            set;
        }

        public TextView Description
        {
            get;
            set;
        }

        public MissionViewHolder(View contentView) : base(contentView)
        {
            LeftIcon = contentView.FindViewById<ADImageView>(Resource.Id.missionsListItemLeftIcon);
            PrizePoints = contentView.FindViewById<TextView>(Resource.Id.missionsListItemPrizeText);
            Description = contentView.FindViewById<TextView>(Resource.Id.missionsListItemDescriptionText);
            RightIcon = contentView.FindViewById<ImageView>(Resource.Id.missionsListItemRightIcon);
        }
    }

    public class MissionsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<Objective> Click;

        public List<Objective> Missions
        {
            get;
            set;
        } = new List<Objective>();

        public override int ItemCount
        {
            get
            {
                if (Missions == null)
                    return 0;
                return Missions.Count;
            }
        }

        public MissionsAdapter(List<Objective> missions)
        {
            Missions = missions;
        }

        private View _lastClickedView = null;

        private void _itemClickDelegate(View sender, Objective mission) {
            if (!sender.Equals(_lastClickedView)) {
                _lastClickedView = sender;
                Click?.Invoke(sender, mission);
            }
        }

        private StateListDrawable SetImageButtonState(int index)
        {
            var states = new StateListDrawable();

            var indexString = (index + 1).ToString().PadLeft(2, '0');
            var packageName = Application.Context.PackageName;
            var resources = Application.Context.Resources;

            var uncheckedMissionImage = ResourcesCompat.GetDrawable(resources, resources.GetIdentifier($"mission_{indexString}", "drawable", packageName), null);
            var checkedMissionImage = ResourcesCompat.GetDrawable(resources, resources.GetIdentifier($"mission_{indexString}b", "drawable", packageName), null);

            states.AddState(new int[] { -Android.Resource.Attribute.StateEnabled }, uncheckedMissionImage);
            states.AddState(new int[] { Android.Resource.Attribute.StateEnabled }, checkedMissionImage);

            return states;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as MissionViewHolder;
            var mission = Missions[position];

            vh.ItemView.Click += (sender, e) => {
                if(position == vh.AdapterPosition)
                    _itemClickDelegate(vh.ItemView,Missions[vh.AdapterPosition]);
            };

            var drawableStates = SetImageButtonState(position);

            vh.LeftIcon.ImageUrl = AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + mission.ImageUrl;

            vh.RightIcon.Enabled = mission.Completed;
            vh.PrizePoints.Enabled = false;
            vh.LeftIcon.Enabled = false;
            vh.Description.Enabled = false;

            vh.PrizePoints.Text = string.Format("{0}pp",mission.PrizeUnits);
            vh.Description.Text = mission.Description;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_mission, parent, false);
            var vh = new MissionViewHolder(itemView);
            return vh;
        }
    }
}