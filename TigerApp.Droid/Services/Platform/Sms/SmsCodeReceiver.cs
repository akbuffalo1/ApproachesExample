#region using

using System;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Telephony;
using Android.Util;
using Android.Widget;

#endregion

namespace TigerApp.Droid.Services.Platform.Sms
{
    [BroadcastReceiver(Label = "SMS Receiver", Enabled = false)]
    [IntentFilter(new[] {"android.provider.Telephony.SMS_RECEIVED"})]
    public class SmsCodeReceiver : BroadcastReceiver
    {
        private const string Tag = "SMSBroadcastReceiver";
        public static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

        public static SmsCodeReceiver Instance { get; private set; }
        public static bool Registered { get; private set; }

        public static void Register(Context context)
        {
            if (Instance == null)
                Instance = new SmsCodeReceiver();
            var filter = new IntentFilter(IntentAction);
            context.RegisterReceiver(Instance, filter);
            Registered = true;
        }

        public static void Unregister(Context context)
        {
            context.UnregisterReceiver(Instance);
            Instance = null;
            Registered = false;
        }

        public static void ResetLastCode()
        {
            Instance.LastVerificationCode = "";
        }

        public string LastVerificationCode { get; private set; }
        public event Action<string> OnSmsCodeReceived;
        private Regex _regex = new Regex(@".+:\s*([0-9A-Za-z]+)");

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                Log.Info(Tag, "Intent received: " + intent.Action);

                if (intent.Action != IntentAction) return;

                SmsMessage[] messages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);

                var sb = new StringBuilder();

                for (var i = 0; i < messages.Length; i++)
                {
                    sb.Append(string.Format("SMS From: {0}{1}Body: {2}{1}", messages[i].OriginatingAddress,
                        Environment.NewLine, messages[i].MessageBody));
                }

                Log.Info(Tag, sb.ToString());

                //TODO investigate messages count in Intent, currently we take first
                var msgBody = messages[0].MessageBody;
                Match match = _regex.Match(msgBody);
                if (match.Success)
                {
                    var code = match.Groups[1];
                    LastVerificationCode = code.Value;
                    OnSmsCodeReceived?.Invoke(LastVerificationCode);
                    Log.Info(Tag, code.Value);
                }
                else
                {
                    Toast.MakeText(context, "Verification code not found", ToastLength.Long).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
            }
        }
    }
}