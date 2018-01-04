using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Extensions
{
    public static class HttpRequestExtensions
    {
        public static Uri Url(this HttpRequest httpRequest)
        { 
            return new Uri(string.Format("{0}://{1}{2}{3}", httpRequest.Scheme, httpRequest.Host, httpRequest.Path, httpRequest.QueryString));
        }
    }
}
