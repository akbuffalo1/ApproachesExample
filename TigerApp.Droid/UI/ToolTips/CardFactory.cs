using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using System;
using TigerApp.Droid.Services.Platform;
using TigerApp.Droid.Utils;
using TigerApp.Shared.Models;
using Xamarin.Facebook;
using Xamarin.Facebook.AppEvents;
using Xamarin.Facebook.Share;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share.Widget;

namespace TigerApp.Droid.UI.ToolTips
{
    public class CardFactory
    {
        public static void ShowBadgeCardDialog(Context ctx, Badge badge)
        {
            var cardDialog = new Dialog(ctx, Resource.Style.ImageTutorialStyle);
            cardDialog.SetContentView(Resource.Layout.dialog_badge_card);
            cardDialog.FindViewById<ImageButton>(Resource.Id.ibBadgeCardX).Click += delegate { cardDialog.Dismiss(); };
            cardDialog.FindViewById<ImageButton>(Resource.Id.ibBadgeCardShare).Click += delegate { ShareBadgeOnFacebook(ctx, badge.Name, badge.Description, badge.ImageUrl, cardDialog.Dismiss); };

            int resourceId = 0;
            switch (badge.Slug)
            {
                case "tiger-addicted":
                    resourceId = Resource.Drawable.badgecard_tiger_addicted;
                    break;
                case "tiger-social":
                    resourceId = Resource.Drawable.badgecard_tiger_social;
                    break;
                case "tiger-evangelist":
                    resourceId = Resource.Drawable.badgecard_tiger_evangelist;
                    break;
                case "mr-tiger":
                    resourceId = Resource.Drawable.badgecard_tiger_mrtiger;
                    break;
                case "tiger-tinder":
                    resourceId = Resource.Drawable.badgecard_tiger_tinder;
                    break;
                case "tiger-collector":
                    resourceId = Resource.Drawable.badgecard_tiger_collector;
                    break;
                case "tiger-trotter":
                    resourceId = Resource.Drawable.badgecard_tiger_trotter;
                    break;
                case "tiger-local":
                    resourceId = Resource.Drawable.badgecard_tiger_local;
                    break;
                case "tiger-weekly":
                    resourceId = Resource.Drawable.badgecard_tiger_weekly;
                    break;
                case "tiger-contributor":
                    resourceId = Resource.Drawable.badgecard_tiger_survey;
                    break;
            }
            cardDialog.FindViewById<ImageView>(Resource.Id.imgBadgeCard).SetImageResource(resourceId);
            cardDialog.Show();
        }

        public static void ShowLevelUpCardDialog(Context ctx, int level, string avatarUrl)
        {
            var cardDialog = new Dialog(ctx, Resource.Style.ImageTutorialStyle);
            cardDialog.SetContentView(Resource.Layout.dialog_level_up_card);
            cardDialog.FindViewById<ImageButton>(Resource.Id.ibLevelUpCardX).Click += delegate { cardDialog.Dismiss(); };
            cardDialog.FindViewById<ImageButton>(Resource.Id.ibLevelUpCardShare).Click += delegate { ShareBadgeOnFacebook(ctx, "", "", avatarUrl, cardDialog.Dismiss); };
            cardDialog.FindViewById<AD.Views.Android.ADImageView>(Resource.Id.imgLevelUpAvatar).ImageUrl = avatarUrl;

            int cardImageResourceId = 0;
            int textImageResourceId = 0;
            switch (level)
            {
                case 2:
                    cardImageResourceId = Resource.Drawable.cambio_livello_2;
                    textImageResourceId = Resource.Drawable.cambio_livello_2_text;
                    break;
                case 3:
                    cardImageResourceId = Resource.Drawable.cambio_livello_3;
                    textImageResourceId = Resource.Drawable.cambio_livello_3_text;
                    break;
                case 4:
                    cardImageResourceId = Resource.Drawable.cambio_livello_4;
                    textImageResourceId = Resource.Drawable.cambio_livello_4_text;
                    break;
                case 5:
                    cardImageResourceId = Resource.Drawable.cambio_livello_5;
                    textImageResourceId = Resource.Drawable.cambio_livello_5_text;
                    break;
                case 6:
                    cardImageResourceId = Resource.Drawable.cambio_livello_6;
                    textImageResourceId = Resource.Drawable.cambio_livello_6_text;
                    break;
            }

            cardDialog.FindViewById<ImageView>(Resource.Id.imgLevelUpCard).SetImageResource(cardImageResourceId);
            cardDialog.FindViewById<ImageView>(Resource.Id.imgLevelUpText).SetImageResource(textImageResourceId);
            cardDialog.Show();
        }

        private static void ShareBadgeOnFacebook(Context ctx, string title, string description, string imageUrl, Action OnShare = null)
        {
            if (!AD.Resolver.Resolve<Shared.Services.API.IProfileApiService>().IsLoggedIn)
            {
                Toast.MakeText(ctx, (ctx as Activity).Resources.GetString(Resource.String.msg_facebook_need_to_login), ToastLength.Long)
                    .Show();
                return;
            }

            var shareDialog = initShareDialog(ctx as Activity);

            var linkContent = new ShareLinkContent.Builder()
                .SetContentTitle(title)
                .SetContentDescription(description)
                .SetImageUrl(Android.Net.Uri.Parse(imageUrl))
                .SetContentUrl(Android.Net.Uri.Parse("http://it.flyingtiger.com/it-IT"))
                .JavaCast<ShareLinkContent.Builder>()
                .Build();

            var canPresentShareDialog = ShareDialog.CanShow(Java.Lang.Class.FromType(typeof(ShareLinkContent)));

            if (canPresentShareDialog)
                shareDialog.Show(linkContent);
            else
                DialogUtil.ShowAlert(ctx, "Cant share post");
        }

        private static ShareDialog initShareDialog(Activity ctx, Action OnShare = null)
        {
            FacebookSdk.SdkInitialize(ctx.ApplicationContext);
            AppEventsLogger.ActivateApp(ctx.Application);

            var callbackManager = AD.Resolver.Resolve<ICallbackManager>();
            if (callbackManager == null)
            {
                var ioc = AD.Resolver.Resolve<AD.IDependencyContainer>();
                callbackManager = CallbackManagerFactory.Create();
                ioc.Register(callbackManager);
            }

            var shareCallback = new FacebookCallback<SharerResult>
            {
                HandleSuccess = shareResult => { OnShare?.Invoke(); },
                HandleCancel = () => { },
                HandleError = shareError => { DialogUtil.ShowAlert(ctx, ctx.Resources.GetString(Resource.String.msg_facebook_post_error)); }
            };

            var shareDialog = new ShareDialog(ctx);
            shareDialog.RegisterCallback(callbackManager, shareCallback);
            return shareDialog;
        }
    }
}
