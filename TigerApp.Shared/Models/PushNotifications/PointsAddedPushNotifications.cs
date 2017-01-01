using System;
namespace TigerApp.Shared.Models.PushNotifications
{
    public enum MissionType {
        Unknown,
        InviteFB,
        ShareFB,
        ProfileUpdate,
        CheckIn,
        Scan,
        CheckIn2Cities,
        TigerTrotter,
        Survey,
    }

    public class PointsAddedPushNotifications:TigerPushNotification
    {
        public const string MissionNameKey = "mission_name";
        public const string PointsKey = "x_points";
        public static string NoPointsExceptionMessage = string.Format("Unable to find {0} in notifcation json!", PointsKey);
        public PointsAddedPushNotifications(IPushNotificationSource source) : base(source) {

            int points = 0;
            var pointsString = source.Get(PointsKey);
            if (string.IsNullOrEmpty(pointsString) || !Int32.TryParse(pointsString, out points))
                throw new ArgumentException(NoPointsExceptionMessage);
            
            Points = points;
            MissionName = source.Get(MissionNameKey);
        }

        public int Points
        {
            get;
            protected set;
        }

        private string _missionName;

        public string MissionName
        {
            get {
                return _missionName;
            }
            protected set {
                _missionName = value;
                _missionType = _getMissionType();
            }
        }

        public bool IsMission
        { 
            get {
                return !string.IsNullOrEmpty(_missionName);
            }
        }

        private MissionType _missionType;
        public MissionType MissionType
        {
            get {
                return _missionType;           
            }
        }

        private MissionType _getMissionType() {

            if(!IsMission)
                return MissionType.Unknown;
                
            string name = _missionName.StartsWith("Carica uno scontrino",StringComparison.CurrentCulture) ? "Carica uno scontrino" : _missionName;

            switch (name) {
                default: 
                    return MissionType.Unknown;
                case "Invita i tuoi amici":
                    return MissionType.InviteFB;
                case "Condividi su fb il tuo prodotto preferito":
                    return MissionType.ShareFB;
                case "Aggiungi informazioni al tuo profilo":
                    return MissionType.ProfileUpdate;
                case "Fai il check-in in uno store Tiger":
                    return MissionType.CheckIn;
                case "Carica uno scontrino":
                    return MissionType.Scan;
                case "Fai il check-in in due città Tiger":
                    return MissionType.CheckIn2Cities;
                case "Vinci il Tiger Trotter Badge!":
                    return MissionType.TigerTrotter;
                case "Rispondi a un survey Tiger!":
                    return MissionType.Survey;
            }
        }

    }
}
