namespace TigerApp.Shared
{
    public static class Constants
    {
        //TEST FB CREDENTIAL
        public const string FacebookAppId = "1722140001387064";
        public const string FacebookAppName = "bytertest";
        public const string FacebookAppUrl = "https://fb.me/1756550631279334";

        /*
         * For iOS go to Info.plist file and change fb1722140001387064 with fb1848317842065749 and FacebookAppID with the right value
         * For Droid go to Resources/values/key.xml and put the right AppId
        public const string FacebookAppId = "1848317842065749";
        public const string FacebookAppName = "TigerApp";*/

        public static class Forms
        {
            public const string SurveyUrl = "https://docs.google.com/a/byters.it/forms/d/e/1FAIpQLSfl6vix6Z_GaveHJ4qAyOFWK4KyJGeQWG4zMMY2HEope4PZ9w/viewform";
            public const string FeedbackUrl = "https://docs.google.com/forms/d/e/1FAIpQLScFg3jqqpA8hGavvfROKzzlyxo0ixAcNI70J4Oh6NvlzaQuaw/viewform";
            public const string CheckinSurvey = "https://docs.google.com/a/byters.it/forms/d/e/1FAIpQLSfZ3vRoXl7mojdtjOq-T87F86f6nX7wdFna4u4FJv4UJIYsVg/viewform?entry.1160456027={0}";
        }

        public static class Flags
        {
            public const string ASKED_FOR_NOTIFICATIONS = nameof(ASKED_FOR_NOTIFICATIONS);
            public const string ASKED_FOR_LOCATION = nameof(ASKED_FOR_LOCATION);
            public const string PRODUCTS_CATALOGUE_POPUP_SHOWN = nameof(PRODUCTS_CATALOGUE_POPUP_SHOWN);
            public const string PRODUCTS_CATALOGUE_TUTORIAL_SHOWN = nameof(PRODUCTS_CATALOGUE_TUTORIAL_SHOWN);
            public const string HOME_TUTORIAL_SHOWN = nameof(HOME_TUTORIAL_SHOWN);
            public const string EXP_PAGE_TUTORIAL_SHOWN = nameof(EXP_PAGE_TUTORIAL_SHOWN);
            public const string COUPON_PAGE_TUTORIAL_SHOWN = nameof(COUPON_PAGE_TUTORIAL_SHOWN);
            public const string PROFILE_PAGE_TUTORIAL_SHOWN = nameof(PROFILE_PAGE_TUTORIAL_SHOWN);
            public const string SURVEY_MISSION_STARTED = nameof(SURVEY_MISSION_STARTED);
        }

        public static class QRCodeSetting
        {
            public const int QRDefaultWidth = 300;
            public const int QRDefaultHeight = 300;
            public const string QRUrl = "?user={0}&coupon_id={1}";
        }

        public static class GeoFence
        {
            public const double DefaultCheckInRadius = 150;
            public const double DefaultRadius = 200;
            public const long ExpirationInHours = 12;
            public const long ExpirationInMilliseconds = ExpirationInHours * 3600 * 1000;
        }

        public static class HockeyApp
        {
            public const string DroidAppId = "4f4e93e1c2b1423b932de4c38906ac15";
            public const string DroidAppSecret = "3f5d99712358d2b8bfbf533cf3d1efae";
            public const string iOSAppId = "1d52d01508ad41ea88c2624e17e191a0";
            public const string iOSAppSecret = "2b95e54987e17c1a078a37bb44dfa15c";
        }

        public static class Strings
        {
            //TODO cleanup or refactor. All strings in one place
            public const string Done = "Fatto";
            public const string Cancel = "Cancella";
            public const string EnableNotificationsMessage = "Autorizza le notifiche\nper ricevere le news e\ni premi Tiger";
            public const string StoriesPageTitle = "ELENCO STORES";
            public const string HomeWelcomeMessage = "Benvenuto nel programma\nloyalty Flying Tiger\nCopenhagen Italia";
            public const string HomeVerificationMessage = "Un SMS di verifica verrà inviato al numero indicato";
            public const string HomeOrMessage = "- oppure -";
            public const string PrivacyMessage = "Lorem ipsum dolor sit amet, aliquet diam, sollicitudin eget donec. Nisl dapibus blandit. Duis libero in, vel elit sed sed viverra lorem neque, commodo pretium nascetur facilisis, orci gravida ante, massa aliquet faucibus quisque magna est. Pulvinar eget vel nunc, sed maecenas sagittis a interdum elit, lacus metus in sed vehicula, ac accumsan arcu accumsan in suspendisse. Consectetuer sed maecenas auctor, a venenatis interdum et. Orci penatibus tortor dapibus quam, eget duis neque hymenaeos posuere id mi, vitae aliquam id montes tellus dolor, amet fringilla lorem, pulvinar velit class mi. Vestibulum non est, in tellus tristique lobortis, dignissim sit proin non donec suscipit, malesuada eros vivamus magna feugiat eget, nulla id est nisl tellus.\n\nNisl rutrum in vestibulum consequat luctus torquent, amet ornare mauris a risus lorem, nullam odio elit ligula in. Suscipit lectus veritatis elit faucibus wisi, ac amet hac at, parturient vehicula velit. In arcu, class id in egestas viverra, sit quae erat risus vel, ac pellentesque. Suscipit vehicula magna, dui fusce, odio ornare velit sollicitudin neque diam posuere. Pretium nibh mattis ac praesent sit, varius vulputate maecenas. Tellus aliquam lectus lacus elit, id non tellus vitae adipiscing, nulla nullam platea cras massa purus rhoncus. Eu ut dui ac lorem, nulla lorem rhoncus eget pede pellentesque. Et sit ac, vivamus lectus quis morbi nulla, ipsum pharetra justo. Habitasse ut doloremque proin sit, nonummy volutpat ligula venenatis lorem varius neque, wisi id sit mauris eu, vivamus tincidunt gravida aliquam varius, lorem luctus urna id. Ut id velit a in accumsan. Turpis id velit tortor, lacus tincidunt pede sed. Vitae ante et sed congue praesent, in non aliquam amet.\n\nTurpis vitae natoque sed nunc, sed ipsum. Ante nulla vel lacus est consequat ac, dolores mollis, consequat auctor vel. Maecenas diam et nunc lacinia ligula, nibh quis ligula, non sem malesuada posuere congue integer aliquam, pellentesque sed non amet, sed felis morbi et non viverra imperdiet. Vestibulum amet, phasellus elit condimentum eleifend tortor porttitor nibh. Amet vitae erat quis gravida. Occaecati sem arcu vulputate nisl mattis, ut a. Nec sollicitudin orci, sagittis amet fermentum nec, dignissim id vestibulum suspendisse praesent quisque, mattis ligula. Aliquam erat. Dolores elementum enim arcu eu lacus. Facilisis tristique adipiscing, molestie sodales erat litora vivamus, id mi suspendisse, erat nibh. Sed etiam gravida, iaculis quam.";
            public const string CheckInMissionPageMessage = "Benvenuto in Tiger! Quando\narrivi da noi fai il check-in\nper guadagnare piu punti!";
            public const string CheckInMissionPageTitle = "Missione: Check-in";
            public const string CheckInMissionPageCompleteMessage = "Oops... ci sembra che\ntu sia ancora molto\nlontano dallo store.\n\n\nvieni a trovarci e riprova\nla missione!";
            public const string EditProfileMissionPageTitle = "Missione: Edit Profile";
            public const string EditProfileMissionPageMessage = "Dicci qualcosa di te per entrare\nsempre piu a far parte della\ncommunity Tiger.\nI dati verranno utilizzati per\nfarti avere ancora piu sorprese!";
            public const string ScanMissionPageMessage = "Carica uno scontrino\nsuperiore a 10 Euro per\nguadagnare ancora piu punti!";
            public const string ScanMissionPageTitle = "Scan scontrino";
            public const string ShareMissionPageMessage = "Sfoglia il catalogo e fai\nlo share del tuo prodotto\npreferito! Per te 10 punti";
            public const string ShareMissionPageTitle = "Missione: Share su FB";
            public const string ProductCataloguePopupMessage1 = "Ciao! Il tuo voto vale punti!\nNon perderli!\nIscriviti subito al\nprogramma loyalty Tiger\nper iniziare a collezionarli";
            public const string ProductCataloguePopupMessage2 = "Non Voglio\nricevere i punti\ne i premi Tiger";
            public const string ProductCatalogueNoProductsMessage = "A breve arriveranno\naltre novita da scoprire\ne votare...";
            public const string CouponTitle = "Complimenti {0}!";
            public const string ExpHomeMessageLevel1 = "Completa le missioni Tiger e\nraggiungi i 350 punti per\npasare al prossimo livello!";
            public const string ExpHomeMessageLevel2 = "Completa 4 missioni Tiger e\nraggiungi gli 800 punti per\npasare al prossimo livello!";
            public const string ExpHomeMessageLevel3 = "Completa 5 missioni Tiger e\nraggiungi gli 2000 punti per\npasare al livello 4!";
            public const string ExpHomeMessageLevel4 = "Completa 5 missioni Tiger e\nraggiungi gli 5000 punti per\npasare al livello 5!";
            public const string ExpHomeMessageLevel5 = "Completa 6 missioni Tiger e\nraggiungi gli 10000 punti per \ndiventare un Flying Tiger!";
            public const string ScanReceiptErrorMessage = "Assicurati che : \n - Lo scontrino non sia danneggiato;\n - Nella scansione vengano inquadrati correttamente l'ID della transazione, la somma spesa e la parola \"TOTALE\"";
            public const string GpsTimeoutMessage = "Non è stato possibile rilevare la tua posizione.";
        }
    }
}