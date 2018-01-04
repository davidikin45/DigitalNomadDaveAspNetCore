using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services
{
    public static class Settings
    {
        public static class Skyscanner
        {
            public const string APIKey = "prtl6749387986743898559646983194";

            public const string PollBrowseRoutesURL = "http://partners.api.skyscanner.net/apiservices/browseroutes/v1.0/{market}/{currency}/{locale}/{originPlace}/{destinationPlace}/{outboundPartialDate}/{inboundPartialDate}?apiKey={apiKey}";
            public const string PollBrowseDatesURL = "http://partners.api.skyscanner.net/apiservices/browsedates/v1.0/{market}/{currency}/{locale}/{originPlace}/{destinationPlace}/{outboundPartialDate}/{inboundPartialDate}?apiKey={apiKey}";
            public const string StartLivePricesURL = "http://partners.api.skyscanner.net/apiservices/pricing/v1.0?apiKey={apiKey}";

            public const string LocationAutoSuggestByQueryURL = "http://partners.api.skyscanner.net/apiservices/autosuggest/v1.0/{market}/{currency}/{locale}/?query={query}&apiKey={apiKey}";
            public const string LocationAutoSuggestByIDURL = "http://partners.api.skyscanner.net/apiservices/autosuggest/v1.0/{market}/{currency}/{locale}/?id={id}&apiKey={apiKey}";
        }

    }
}
