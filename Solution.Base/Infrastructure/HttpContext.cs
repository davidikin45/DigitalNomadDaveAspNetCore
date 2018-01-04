using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Infrastructure
{
    public static class HttpContext
    {

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get { return StaticProperties.HttpContextAccessor.HttpContext; }
        }

        public static T GetInstance<T>()
        {
            return (T)Current.RequestServices.GetService(typeof(T));
        }

        public static string MapPath(string path)
        {
            return Server.MapWwwPath(path);
        }

    }
}
