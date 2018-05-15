using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DND.Bot.Models.Common
{
    public class CommonModel
    {
        public CommonModel()
        {
            Id = "";
            Session = new Session();
            Request = new Request();
            Response = new Response();
        }

        public string Id { get; set; }
        internal Session Session { get; set; }
        internal Request Request { get; set; }
        internal Response Response { get; set; }
    }

    internal class Response
    {
        public string Text { get; set; }
        public string Ssml { get; set; }
        public string Prompt { get; set; }
        public string Event { get; set; }
        public Card Card { get; set; }
    }

    public class Card
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }

    internal class Request
    {
        public string Id { get; set; }
        public string Intent { get; set; }
        public string State { get; set; }
        public string Channel { get; set; }
        public List<KeyValuePair<string, string>> Parameters { get; set; }
    }

    internal class Session
    {
        public string Id { get; set; }
        public bool EndSession { get; set; }
    }
}