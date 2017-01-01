using CoreGraphics;
using Facebook.ShareKit;
using Foundation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using TigerApp.Shared.Models;
using TigerApp.Shared.ViewModels;
using UIKit;

namespace TigerApp.iOS.Pages.MissionList
{
    public partial class MissionListViewController : BaseReactiveViewController<IMissionListViewModel>, Facebook.ShareKit.IAppInviteDialogDelegate
    {
        private UITableView missionTableView;
        protected List<Objective> Missions { get; private set; }

        private void ExpHomeButton_TouchUpInside(object sender, EventArgs e)
        {
            DismissViewController(true, null);
        }

        public MissionListViewController()
        {
            TransitioningDelegate = TransitionManager.Bottom;

            this.WhenActivated(dis =>
            {
                dis(ViewModel.WhenAnyValue(vm => vm.Missions).Subscribe(missions =>
                {
                    if (missions != null)
                    {
                        Missions = missions;
                        SetMissions();
                    }
                }));

                ViewModel.GetMissions();
            });
        }

        private void SetMissions()
        {
            missionTableView.Source = new MissionTableViewSource(Missions, this);
            missionTableView.ReloadData();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            missionTableView = new UITableView(CGRect.Empty, UITableViewStyle.Plain);
            missionTableView.RegisterNibForCellReuse(MissionTableViewCell.Nib, MissionTableViewCell.ReusableIdentifier);
            missionTableView.TranslatesAutoresizingMaskIntoConstraints = false;
            missionTableView.RowHeight = 100;
            missionTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            //missionTableView.AllowsSelection = false;

            missionViewHolder.Add(missionTableView);
            missionTableView.TopAnchor.ConstraintEqualTo(missionViewHolder.TopAnchor).Active = true;
            missionTableView.LeftAnchor.ConstraintEqualTo(missionViewHolder.LeftAnchor).Active = true;
            missionTableView.BottomAnchor.ConstraintEqualTo(missionViewHolder.BottomAnchor).Active = true;
            missionTableView.RightAnchor.ConstraintEqualTo(missionViewHolder.RightAnchor).Active = true;
            base.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            base.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            expHomeButton.TouchUpInside += ExpHomeButton_TouchUpInside;
        }

        public void DidComplete(AppInviteDialog appInviteDialog, NSDictionary results)
        {
            ViewModel.SendInviteFBActionToServer();
        }

        public void DidFail(AppInviteDialog appInviteDialog, NSError error)
        {

        }
    }
}