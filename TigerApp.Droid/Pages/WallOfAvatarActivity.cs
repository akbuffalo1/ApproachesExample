#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AD.Views.Android;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using TigerApp.Droid.UI;
using TigerApp.Droid.Utils;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;
using GLocation = Android.Locations.Location;

#endregion

namespace TigerApp.Droid.Pages
{
    [Activity (NoHistory = true)]
    public class WallOfAvatarActivity : BaseReactiveActivity<IWallOfAvatarViewModel>
    {
        private AvatarsAdapter _adapter;
        private RecyclerView _avatarList;
        private RecyclerView.LayoutManager _layoutManager;
        private Button _btnConfirm;
        protected List<AvatarModelWrapper> Avatars { get; private set; }
        private AvatarModelWrapper _selectedAvatar;
        private Type callerActivityType; 
        private ProgressBar pbProgress;
        private bool _openFromProfile;
        
        public WallOfAvatarActivity()
        {
            Avatars = new List<AvatarModelWrapper>();

            this.WhenActivated(dispose => { 
                dispose(ViewModel.WhenAnyValue(vm => vm.Avatars).Where(avatrs => avatrs != null).Subscribe(avatars =>
                {
                    _openFromProfile = ViewModel.Profile.Avatar != null;
                    CreateAvatarModel(avatars);
                }));

                dispose(ViewModel.WhenAnyValue(vm => vm.IsBusy).Subscribe(isBusy => pbProgress.Visibility = isBusy ? ViewStates.Visible : ViewStates.Gone));
                dispose(this.BindCommand(ViewModel, x => x.UpdateAvatar, x => x._btnConfirm));
                dispose(ViewModel.WhenAnyValue(vm => vm.UpdateFinished).Where(finished => finished).Subscribe(finished =>
                {
                    if (String.IsNullOrEmpty(ViewModel.Profile.NickName))
                        StartNewActivity(typeof(NicknameEnrollmentActivity), TransitionWay.RL);
                    else if (!_openFromProfile)
                        StartNewActivity(typeof(ExpHomeActivity), TransitionWay.RL);
                    Finish();
                }));

                ViewModel.GetUserAvatars();
            });
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_avatar_wall);

            _btnConfirm = FindViewById<Button>(Resource.Id.btnConfirm);

            _avatarList = FindViewById<RecyclerView>(Resource.Id.avatarList);
            pbProgress = FindViewById<ProgressBar>(Resource.Id.pbProgress);
            _layoutManager = new GridLayoutManager(this, 2);
            _avatarList.AddItemDecoration(new GridSpacingItemDecoration(2, (int)Resources.GetDimension(Resource.Dimension.avatar_list_items_spacing)));
            _avatarList.SetLayoutManager(_layoutManager);
            ((SimpleItemAnimator)_avatarList.GetItemAnimator()).SupportsChangeAnimations = false;
            _adapter = new AvatarsAdapter(Avatars, this);
            _avatarList.SetAdapter(_adapter);
            _btnConfirm.Enabled = false;
        }

        private void CreateAvatarModel(List<Avatar> avatars)
        {
            Avatars.Clear();
            avatars = avatars.OrderBy(a => a.Name).Take(6).ToList();
            var avatarModelWrappers = avatars.Select(avatar => new AvatarModelWrapper(avatar, ViewModel.Profile, ViewModel));
            Avatars = avatarModelWrappers.ToList();
            Avatars.ForEach(a => _selectedAvatar = a.Selected?a:_selectedAvatar);
            _adapter.Update(Avatars);
            _adapter.OnItemClik += OnItemClick;
        }

        private void OnItemClick(AvatarModelWrapper avatarModel)
        {
            if (_selectedAvatar == avatarModel)
            {
                _selectedAvatar.Selected = false;
                _adapter.NotifyItemChanged(Avatars.IndexOf(_selectedAvatar));
                _btnConfirm.Enabled = false;
                _selectedAvatar = null;
                return;
            }

            if (_selectedAvatar != null)
            {
                _selectedAvatar.Selected = false;
                _adapter.NotifyItemChanged(Avatars.IndexOf(_selectedAvatar));
            }

            _selectedAvatar = avatarModel;
            _selectedAvatar.Selected = true;
            _adapter.NotifyItemChanged(Avatars.IndexOf(_selectedAvatar));
            _btnConfirm.Enabled = true;

            ViewModel.SelectedAvatarId = avatarModel.Selected ? avatarModel.Model.Id : string.Empty;
        }

        private class AvatarViewHolder : RecyclerView.ViewHolder
        {
            public readonly ADImageView AvatarHolder;

            public AvatarViewHolder(View itemView, Action<int> onClick) : base(itemView)
            {
                AvatarHolder = itemView.FindViewById<ADImageView>(Resource.Id.avatarHolder);
                itemView.Click += (sender, e) =>
                {
                    onClick(AdapterPosition);
                };
            }
        }

        private class AvatarsAdapter : RecyclerView.Adapter
        {
            private List<AvatarModelWrapper> _avatars;
            private Color _selectedColor;
            private Color _defaultColor;

            public event Action<AvatarModelWrapper> OnItemClik;

            public AvatarsAdapter(List<AvatarModelWrapper> avatars, Context context)
            {
                _avatars = avatars;
               
                _defaultColor = context.Resources.GetColor(Resource.Color.avatar_holder_default_back);
                _selectedColor = context.Resources.GetColor(Resource.Color.avatar_holder_selected_back);
            }

            public void Update(List<AvatarModelWrapper> avatars)
            { 
                _avatars = avatars;
                NotifyDataSetChanged();
            }

            public override int ItemCount
            {
                get { return _avatars?.Count() ?? 0; }
            }

            private void OnItemClickInternal(int position)
            {
                var m = _avatars.ElementAt(position);
                OnItemClik?.Invoke(m);
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var vh = holder as AvatarViewHolder;
                var modelWrapper = _avatars.ElementAt(position);
                var avatar = vh.AvatarHolder;

                if (modelWrapper.Selected)
                    avatar.Background.SetColorFilter(_selectedColor, PorterDuff.Mode.Multiply);
                else
                    avatar.Background.SetColorFilter(_defaultColor, PorterDuff.Mode.Multiply);

                avatar.PivotX = avatar.Width * 0.5f;
                avatar.PivotY = avatar.Height * 0.5f;
                avatar.Rotation = modelWrapper.Rotation;

                avatar.ImageUrl = AD.Resolver.Resolve<AD.IHttpServerConfig>().BaseAddress + modelWrapper.Model.ImageUrl;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var avatarView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.item_avatar, parent, false);

                var vh = new AvatarViewHolder(avatarView, OnItemClickInternal);

                return vh;
            }
        }
    }

    public class AvatarModelWrapper
    {
        public Avatar Model;
        public bool Selected;
        public int ResId;
        public int Rotation;
        public AvatarModelWrapper(Avatar avatar, UserProfile profile, IWallOfAvatarViewModel viewModel)
        {
            Model = avatar;
            if (profile?.Avatar != null)
            {
                Selected = profile.Avatar.Id == avatar.Id;
                if(Selected)
                    viewModel.SelectedAvatarId = avatar.Id;
            }
            Rotation = RandomUtil.RandomInt(-4, 4);
        }
    }

    public class GridSpacingItemDecoration : RecyclerView.ItemDecoration
    {
        private int _spanCount;
        private int _spacing;

        public GridSpacingItemDecoration(int spanCount, int spacing)
        {
            _spanCount = spanCount;
            _spacing = spacing;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            int position = parent.GetChildAdapterPosition(view); // item position
            int column = position % _spanCount; // item column

            outRect.Left = _spacing - column * _spacing / _spanCount; // _spacing - column * ((1f / _spanCount) * _spacing)
            outRect.Right = (column + 1) * _spacing / _spanCount; // (column + 1) * ((1f / _spanCount) * _spacing)

            if (position < _spanCount)
            {
                // top edge
                outRect.Top = _spacing;
            }
            outRect.Bottom = _spacing; // item bottom
        }
    }
}