using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure
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

        public static object GetInstance(Type t)
        {
            return Current.RequestServices.GetService(t);
        }

        public static string MapPath(string path)
        {
            return Server.MapWwwPath(path);
        }

    }
}
