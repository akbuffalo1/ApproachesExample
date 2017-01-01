using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using ReactiveUI;
using AD.Core.Reactive;
using TigerApp.Shared.ViewModels;

namespace TigerApp.Droid.Pages
{
	[Activity(Label = "TwitterTestActivity", MainLauncher = true)]
	public class TwitterTestActivity : BaseReactiveActivity<ITwitterTestViewModel>
	{
		private Button _btnDoLogin, _btnDoLogout, _btnSearch;
		private ImageView _imgProfile;
		private ProgressBar _pbProgress;

		public TwitterTestActivity()
		{
			this.WhenActivated(registerDisposable =>
			{
				ViewModel
					.WhenAnyValue(vm => vm.Error)
					.Where(error => !string.IsNullOrEmpty(error))
					.Subscribe(error => Toast.MakeText(this, error, ToastLength.Long).Show())
					.DisposeWith(registerDisposable);

				ViewModel
					.WhenAnyValue(vm => vm.IsLoaderShowing)
					.ObserveOnUI()
					.Subscribe(HasToShow => _pbProgress.Visibility = HasToShow ? ViewStates.Visible : ViewStates.Invisible)
					.DisposeWith(registerDisposable);

				ViewModel
					.WhenAnyValue(vm => vm.UserData)
					.Where(data => null != data)
					.SubscribeOnce(data => Glide.With(this).Load((string)data["profile_image_url"]).Into(_imgProfile));

				this.BindCommand(ViewModel, vm => vm.GetProfileCommand, vc => vc._btnSearch).DisposeWith(registerDisposable);
				this.BindCommand(ViewModel, vm => vm.LogInCommand, vc => vc._btnDoLogin).DisposeWith(registerDisposable);
				this.BindCommand(ViewModel, vm => vm.LogOutCommand, vc => vc._btnDoLogout).DisposeWith(registerDisposable);
			});
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_twitter_test);

			_btnDoLogin = FindViewById<Button>(Resource.Id.btnDoLogin);
			_btnDoLogout = FindViewById<Button>(Resource.Id.btnDoLogout);
			_btnSearch = FindViewById<Button>(Resource.Id.btnSearch);
			_imgProfile = FindViewById<ImageView>(Resource.Id.imgProfile);
			_pbProgress = FindViewById<ProgressBar>(Resource.Id.pbProgress);
		}

		protected override void OnStart()
		{
			base.OnStart();
			ViewModel.CheckInitialized();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}