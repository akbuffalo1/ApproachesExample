using System;
using Android.Webkit;
using Android.Content;
using Android.Util;

namespace TigerApp.Droid.UI
{
    public class GoogleFormsWebClient : WebViewClient
    {
        private GoogleFormsWebView _webView;
        public GoogleFormsWebClient(GoogleFormsWebView webView){
            _webView = webView;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            view.LoadUrl(url);
            return true;
        }

        public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            if (!url.Equals(_webView.FormsUrl))
                _webView.FormsSubmitted = url.EndsWith("/formResponse",StringComparison.CurrentCulture);
        }

    }

    public class GoogleFormsWebView:WebView
    {
        public event EventHandler<bool> OnSubmit;
        public event EventHandler<string> OnUrlLoaded;
        public string FormsUrl {
            get;
            set;
        }

        public GoogleFormsWebView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _init("");
        }

        public GoogleFormsWebView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs,defStyleAttr,defStyleRes)
        {
            _init("");
        }

        public GoogleFormsWebView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            _init("");
        }

        public GoogleFormsWebView(Context context, IAttributeSet attrs, int defStyleAttr, bool privateBrowsing) : base(context, attrs, defStyleAttr, privateBrowsing)
        {
            _init("");
        }

        public GoogleFormsWebView(Context context, string formsUrl):base(context)
        {
            _init(formsUrl);
        }

        private void _init(string formsUrl) {
            SetWebViewClient(new GoogleFormsWebClient(this));
            FormsUrl = formsUrl;
            Settings.JavaScriptEnabled = true;
        }

        public void OpenForm() {
            LoadUrl(FormsUrl);
        }

        public override void LoadUrl(string url)
        {
            base.LoadUrl(url);
            OnUrlLoaded?.Invoke(this,url);
        }

        private bool _formsSubmitted;
        public bool FormsSubmitted { 
            get {
                return _formsSubmitted;
            }
            set {
                _formsSubmitted = value;
                OnSubmit?.Invoke(this,value);
            }
        }
    }
}
