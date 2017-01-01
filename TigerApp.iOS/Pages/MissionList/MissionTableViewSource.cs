using AD;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using TigerApp.iOS.Pages.Missions.CitiesCheckin;
using TigerApp.iOS.Pages.Missions.TrotterMission;
using TigerApp.Shared.Models;
using UIKit;
using TigerApp.iOS.Pages.Missions.SurveyMission;

namespace TigerApp.iOS.Pages.MissionList
{
    public class MissionTableViewSource : UITableViewSource
    {
        private readonly List<Objective> _missions;
        private readonly MissionListViewController _controller;

        public override nint NumberOfSections(UITableView tableView) => 1;
        public override nint RowsInSection(UITableView tableView, nint section) => _missions.Count;

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(MissionTableViewCell.ReusableIdentifier, indexPath);
            return cell as UITableViewCell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, Foundation.NSIndexPath indexPath)
        {
            cell.WithType<MissionTableViewCell>(c => c.Bind(_missions.ElementAt(indexPath.Row)));
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            var mission = _missions[indexPath.Row];

            switch (mission.Mark)
            {
                case "add-10pt-when-user-invites-friend-on-fb": //use facebook api to invite friends
                    // TODO use real link from server app and implement invitation on server
                    Facebook.ShareKit.AppInviteContent content = new Facebook.ShareKit.AppInviteContent();
                    content.AppLinkURL = new NSUrl(Shared.Constants.FacebookAppUrl);
                    Facebook.ShareKit.AppInviteDialog.Show(_controller, content, _controller);
                    break;
                case "add-10pt-when-user-shares-product-on-fb": //open * ShareMissionViewController *.
                    _controller.PresentViewController(new ShareMissionViewController(), true, null);
                    break;
                case "add-10pt-when-user-fills-his-profile": //open * EditProfileMIssionViewController *.
                    _controller.PresentViewController(new EditProfileMissionViewController(), true, null);
                    break;
                case "add-10pt-when-user-checkin-tiger-store": //open * CheckinMissionViewController *.
                    _controller.PresentViewController(new CheckInMissionViewController(), true, null);
                    break;
                case "add-15pt-when-user-checkin-in-two-different-cities": //open * TwoCitiesCheckInViewController *.
                    _controller.PresentViewController(new TwoCitiesCheckInViewController(), true, null);
                    break;
                case "add-10pt-when-user-scan-receipt-with-more-than-10-eur": //open * ScanMissionViewController * with 10 * euros * value.
                    _controller.PresentViewController(new ScanMissionViewController(), true, null);
                    break;
                case "add-22pt-when-user-scan-receipt-with-more-than-20-eur": //open * ScanMissionViewController * with 20 * euros * value.
                    _controller.PresentViewController(new ScanMissionViewController(ScanMissionViewController.Euros.E20), true, null);
                    break;
                case "add-35pt-when-user-scan-receipt-with-more-than-30-eur": //open * ScanMissionViewController * with 30 * euros * value.
                    _controller.PresentViewController(new ScanMissionViewController(ScanMissionViewController.Euros.E30), true, null);
                    break;
                case "add-40pt-when-user-checkin-5-different-store": //open * TrotterMissionViewController *.
                    _controller.PresentViewController(new TrotterMissionViewController(), true, null);
                    break;
                case "add-10pt-when-user-complete-survey":
                    _controller.PresentViewController(new SurveyMissionViewController(), true, null);                   
                    break;
            }
        }

        public MissionTableViewSource(IEnumerable<Objective> missions, MissionListViewController controller)
        {
            _missions = missions.OrderBy(m => m.Order).ToList();
            _controller = controller;
        }
    }
}

