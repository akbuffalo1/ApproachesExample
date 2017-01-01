#region using

using System;
using System.Collections.Generic;
using TigerApp.Droid.UI.ToolTips;

#endregion

namespace TigerApp.Droid.Tutorial
{
    public class TooltipTutorialController : SimpleTooltip.IOnDismissListener
    {
        public static TooltipTutorialController ShowTips(List<SimpleTooltip> tips)
        {
            return new TooltipTutorialController(tips).Start();
        }

        public static TooltipTutorialController ShowTipsAndSetFlagWhenFinish(List<SimpleTooltip> tips,
            IFlagStoreService flagStore, string flag)
        {
            var controller = new TooltipTutorialController(tips);
            controller.OnComplete += () => { flagStore.Set(flag); };

            controller.Start();

            return controller;
        }

        private List<SimpleTooltip> _tips;
        private int _index;
        private SimpleTooltip _currentTooltip;

        public event Action OnComplete;

        public TooltipTutorialController(List<SimpleTooltip> tips)
        {
            _tips = tips;
        }

        public TooltipTutorialController Start()
        {
            ShowTip();

            return this;
        }

        public TooltipTutorialController Cancel()
        {
            if (_currentTooltip != null)
            {
                _currentTooltip.OnDismissListener = null;
                _currentTooltip.Dismiss();
            }
            return this;
        }

        private void ShowTip()
        {
            var tip = _tips[_index];
            tip.OnDismissListener = this;
            tip.Show();

            _currentTooltip = tip;
        }

        public void OnDismiss(SimpleTooltip tooltip)
        {
            _index++;
            if (_index == _tips.Count)
            {
                OnComplete?.Invoke();
                return;
            }

            ShowTip();
        }
    }
}