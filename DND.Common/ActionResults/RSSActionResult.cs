using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.AspNetCore.Http;

namespace DND.Common.ActionResults
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

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/rss+xml";

            if (_feed != null)
            {
                var xmlWriter = CreateXmlWriter(response);
                _feed.WriteTo(xmlWriter);
                await xmlWriter.FlushAsync();
            }
        }

        private XmlWriter CreateXmlWriter(HttpResponse response)
        {
            return XmlWriter.Create(response.Body, new XmlWriterSettings()
            {
                Async = true,
                Encoding = Encoding.UTF8,
                Indent = true
            });
        }
    }
}
