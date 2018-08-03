using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Services
{
    public interface ICookieService
    {
        string Get(string key);
        void Set(string key, string value, int? expireTime);
        void Remove(string key);
    }
}
