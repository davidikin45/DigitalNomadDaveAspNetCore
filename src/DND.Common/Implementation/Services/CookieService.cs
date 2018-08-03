using DND.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Services
{
    public class CookieService : ICookieService
    {
        private IHttpContextAccessor _httpContextAccessor;
        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Get(string key)
        {
            if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(key))
                return _httpContextAccessor.HttpContext.Request.Cookies[key];

            return "";
        }

        public void Remove(string key)
        {
           _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
        }

        public void Set(string key, string value, int? expireDays)
        {
            CookieOptions option = new CookieOptions();
            if (expireDays.HasValue)
                option.Expires = DateTime.Now.AddDays(expireDays.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }
    }
}
