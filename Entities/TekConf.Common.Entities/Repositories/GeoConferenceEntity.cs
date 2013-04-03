using TinyMessenger;

namespace TekConf.UI.Api
{
    public class GeoConferenceEntity : ConferenceEntity
    {
        public GeoConferenceEntity(ITinyMessengerHub hub, IRepository<ConferenceEntity> repository)
            : base(hub, repository)
        {

        }

        public double Distance { get; set; }
    }
}