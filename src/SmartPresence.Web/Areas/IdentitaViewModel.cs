using SmartPresence.Web.Infrastructure;

namespace SmartPresence.Web.Areas
{
    public class IdentitaViewModel
    {
        public static string VIEWDATA_IDENTITACORRENTE_KEY = "IdentitaUtenteCorrente";
        public static string VIEWDATA_IDENTITACORRENTE_SURNAMENAME = "IdentitaCorrenteNameSurname";

        public string EmailUtenteCorrente { get; set; }
        public string SurnameName { get; set; }

        public string GravatarUrl
        {
            get
            {
                return EmailUtenteCorrente.ToGravatarUrl(ToGravatarUrlExtension.DefaultGravatar.Identicon, null);
            }
        }
    }
}
