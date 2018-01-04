using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Solution.Base.ActionResults
{
    public class RSSActionResult : ActionResult
    {
        public string ContentType { get; set; }

        private readonly SyndicationFeedFormatter _feed;
        public SyndicationFeedFormatter Feed
        {
            get { return _feed; }
        }

        public RSSActionResult(SyndicationFeedFormatter feed)
        {
            _feed = feed;
        }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/rss+xml";

            if (_feed != null)
                using (var xmlWriter = new XmlTextWriter(response.Body, null))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    _feed.WriteTo(xmlWriter);
                }
        }
    }
}
