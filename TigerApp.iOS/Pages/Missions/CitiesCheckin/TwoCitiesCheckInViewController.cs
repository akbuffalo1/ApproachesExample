using System.Collections.Generic;
using System.Reactive.Linq;
using System;
using System.Linq;
using AD.iOS;
using ReactiveUI;
using TigerApp.iOS.Utils;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.Missions.CitiesCheckin
{
    public partial class TwoCitiesCheckInViewController : BaseReactiveViewController<ITwoCitiesCheckInViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            titleLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerCandy.WithSize(32), textAlignment: UITextAlignment.Left, lineHeight: 33);

            DeviceHelper.OnIphone5(() =>
            {
                missionDescriptionLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(30), lineHeight: 25);
            });

            DeviceHelper.OnIphone6(() =>
            {
                citiesStackHeightConstraint.Constant = 120;
            });

            DeviceHelper.OnIphone6P(() =>
            {
                foreach (var sub in citiesStackView.ArrangedSubviews)
                {
                    var label = (sub as UIStackView).ArrangedSubviews[1] as UILabel;
                    label.ApplyTigerFontDefaultAttributes(Fonts.TigerCandy.WithSize(36), lineHeight: 36);
                }
                citiesStackHeightConstraint.Constant = 150;
                missionDescriptionLabel.ApplyTigerFontDefaultAttributes(Fonts.TigerBasic.WithSize(36), lineHeight: 35);
            });

            backButton.TouchUpInside += delegate { DismissViewController(true, null); };
        }

        public TwoCitiesCheckInViewController()
        {
            this.WhenActivated(dis =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.Cities).Where(cities => cities != null).Subscribe(cities => { 
                    for (var ind = 0; ind < cities.Count; ind += 1)
                    {
                        var cityName = ViewModel.Cities[ind].ToLower();
                        var substackView = citiesStackView.ArrangedSubviews[ind] as UIStackView;
                        var btn = substackView.ArrangedSubviews[0] as UIButton;
                        var lbl = substackView.ArrangedSubviews[1] as UILabel;
                        lbl.Text = cityName;
                        btn.Enabled = true;
                        lbl.Highlighted = true;
                    }
                }));
                ViewModel.GetCheckInCities();

            });
        }
    }
}