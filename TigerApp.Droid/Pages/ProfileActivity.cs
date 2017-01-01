using AD;
using AD.Views.Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using TigerApp.Droid.Services.Platform;
using TigerApp.Droid.UI.ToolTips;
using TigerApp.Shared;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;
using Xamarin.Facebook;
using Xamarin.Facebook.Share;
using Xamarin.Facebook.Share.Widget;

namespace TigerApp.Droid.Pages
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : BaseReactiveActivity<IProfileViewModel>
    {
        private ADImageView _imgProfilePlaceholder;
        private TextView _txtProfileNickname;
        private TextView _txtProfileLevel;
        private TextView _txtProfileLevelName;
        private ImageView _btnProfileGift;
        private ImageView _btnProfileCheck;
        private RecyclerView _listProfileBadges;
        private ImageView _btnProfileUp;
        private BadgeAdapter _badgesAdapter;
        private Dialog _cardDialog;
        private Badge _selectedBadge;

        private FacebookCallback<SharerResult> _shareCallback;
        private ShareDialog _shareDialog;
        private ICallbackManager callbackManager { get; set; }

        public ProfileActivity()
        {
            this.WhenActivated(dispose =>
            {
                InitListeners(dispose);

                dispose(this.OneWayBind(ViewModel, vm => vm.Username, v => v._txtProfileNickname.Text));
                dispose(this.OneWayBind(ViewModel, vm => vm.Level, v => v._txtProfileLevel.Text));
                dispose(this.OneWayBind(ViewModel, vm => vm.Status, v => v._txtProfileLevelName.Text));

                dispose(ViewModel.WhenAnyValue(vm => vm.ProfileImageUrl).Subscribe(imageUrl =>
                {
                    if (imageUrl.Equals(ProfileViewModel.PLACEHOLDER_PROFILE_IMAGE_URL))
                        _imgProfilePlaceholder.SetImageResource(Resource.Drawable.profile_01_placeholder);
                    else
                        _imgProfilePlaceholder.ImageUrl = AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + imageUrl;
                }));

                dispose(ViewModel.WhenAnyValue(vm => vm.Badges).Where(badges => badges != null).Subscribe(badges =>
                {
                    _badgesAdapter.Update(badges);

                    if (ViewModel.ShouldShowTutorial)
                    {
                        ShowTutorial();
                    }
                }));
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_profile);

            _btnProfileGift = FindViewById<ImageView>(Resource.Id.btnProfileGift);
            _btnProfileCheck = FindViewById<ImageView>(Resource.Id.btnProfileCheck);
            _btnProfileUp = FindViewById<ImageView>(Resource.Id.btnProfileUp);

            _imgProfilePlaceholder = FindViewById<ADImageView>(Resource.Id.imgProfilePlaceholder);
            _txtProfileNickname = FindViewById<TextView>(Resource.Id.txtProfileNickname);
            _txtProfileLevel = FindViewById<TextView>(Resource.Id.txtProfileLevel);
            _txtProfileLevelName = FindViewById<TextView>(Resource.Id.txtProfileLevelName);
            _listProfileBadges = FindViewById<RecyclerView>(Resource.Id.listProfileBadges);
            var layoutManager = new LinearLayoutManager(this, GridLayoutManager.Horizontal, false);
            _listProfileBadges.SetLayoutManager(layoutManager);
            _badgesAdapter = new BadgeAdapter();
            _listProfileBadges.SetAdapter(_badgesAdapter);
            _badgesAdapter.ItemSelected += (s, e) =>
            {
                _selectedBadge = e;
                CardFactory.ShowBadgeCardDialog(this, e);
            };

            OnSwipeUp += () => { Finish(); };
            OnSwipeLeft += () => { StartNewActivity(typeof(EditProfileActivity), TransitionWay.RL); };
            OnSwipeRight += () => { StartCouponPage(); };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        void InitListeners(Action<IDisposable> dispose)
        {
            dispose(Observable.FromEventPattern(sub => _btnProfileGift.Click += sub, unSub => _btnProfileGift.Click -= unSub)
                    .Subscribe(args =>
                    {
                        StartCouponPage();
                    }));
            dispose(Observable.FromEventPattern(sub => _btnProfileCheck.Click += sub, unSub => _btnProfileCheck.Click -= unSub)
                    .Subscribe(args =>
                    {
                        StartNewActivity(typeof(EditProfileActivity), TransitionWay.RL);
                    }));
            dispose(Observable.FromEventPattern(sub => _btnProfileUp.Click += sub, unSub => _btnProfileUp.Click -= unSub)
                    .Subscribe(args =>
                    {
                        Finish();
                    }));
            dispose(Observable.FromEventPattern(sub => _imgProfilePlaceholder.Click += sub, unSub => _imgProfilePlaceholder.Click -= unSub)
                    .Subscribe(args =>
                    {
                        StartNewActivity(typeof(WallOfAvatarActivity), TransitionWay.Default);
                    }));
        }

        private void StartCouponPage()
        {
            Intent intent = new Intent(this, typeof(CouponPageActivity));
            intent.PutExtra(CouponPageActivity.IN_DATA, true);
            StartNewActivity(intent, TransitionWay.LR);
        }

        private void ShowTutorial()
        {
            var tips = new List<SimpleTooltip>()
            {
                 new TooltipBuilder(this)
                {
                    AnchorView = this._imgProfilePlaceholder,
                    Text = Resources.GetString(Resource.String.tut_profile_s1),
                    Gravity = GravityFlags.Bottom,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                 new TooltipBuilder(this)
                {
                    AnchorView = this._listProfileBadges,
                    Text = Resources.GetString(Resource.String.tut_profile_s2),
                    Gravity = GravityFlags.Top,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                  new TooltipBuilder(this)
                {
                    AnchorView = this._btnProfileGift,
                    Text = Resources.GetString(Resource.String.tut_profile_s3),
                    Gravity = GravityFlags.Bottom,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build(),
                   new TooltipBuilder(this)
                {
                    AnchorView = this._btnProfileCheck,
                    Text = Resources.GetString(Resource.String.tut_profile_s4),
                    Gravity = GravityFlags.Bottom,
                }.SetContentView(Resource.Layout.back_default_tooltip, Resource.Id.txtToolTip)
                .Build()
            };

            ShowTipsAndSetFlagWhenFinish(tips, FlagStore, Constants.Flags.PROFILE_PAGE_TUTORIAL_SHOWN);
        }

        class BadgeViewHolder : RecyclerView.ViewHolder
        {
            public ADImageView Image { get; private set; }

            public BadgeViewHolder(View itemView, Action<int> onClick) : base(itemView)
            {
                Image = itemView.FindViewById<ADImageView>(Resource.Id.imgBadgeItemPlaceholder);

                itemView.Click += delegate { onClick(AdapterPosition); };
            }
        }

        class BadgeAdapter : RecyclerView.Adapter
        {
            private IEnumerable<Badge> _badges;

            public event EventHandler<Badge> ItemSelected;

            public BadgeAdapter(IEnumerable<Badge> badges = null)
            {
                _badges = badges;
            }

            public override int ItemCount { get { return _badges?.Count() ?? 0; } }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                BadgeViewHolder vh = holder as BadgeViewHolder;
                var badge = _badges.ElementAt(position);
                if (string.IsNullOrEmpty(badge.ImageUrl) || badge.ImageUrl.Equals("no.png"))
                    vh.Image.SetImageResource(Resource.Drawable.badge_blank);
                else
                    vh.Image.ImageUrl = badge.ImageUrl;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_profile_badge, parent, false);
                BadgeViewHolder vh = new BadgeViewHolder(itemView, OnClick);
                return vh;
            }

            public void Update(IEnumerable<Badge> badges)
            {
                _badges = badges;
                NotifyDataSetChanged();
            }

            private void OnClick(int position)
            {
                var badge = _badges.ElementAt(position);

                if (!string.IsNullOrEmpty(badge.Slug))
                {
                    ItemSelected?.Invoke(this, badge);
                }
            }
        }
    }
}