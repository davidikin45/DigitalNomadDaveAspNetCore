using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Dynamic
{
    public static class DynamicHelper
    {
        public static object GetValue(object dyn, string propName)
        {
            if (dyn is ExpandoObject)
            {
                var dict = dyn as IDictionary<string, object>;
                return dict[propName];
            }
            else
            {
                // Warning: this is rather expensive, and should be cached in a real app
                var GetterSite = CallSite<Func<CallSite, object, object>>.Create(
                        Binder.GetMember(CSharpBinderFlags.None, propName,
                            dyn.GetType(),
                            new CSharpArgumentInfo[] {
                             CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                            }));
                return GetterSite.Target(GetterSite, dyn);
            }
        }

        public static void SetValue(object dyn, string propName, object val)
        {
            if (dyn is ExpandoObject)
            {
                var dict = dyn as IDictionary<string, object>;
                dict[propName] = val;
            }
            else
            {
                // Warning: this is rather expensive, and should be cached in a real app
                var SetterSite = CallSite<Func<CallSite, object, object, object>>.Create(
                     Binder.SetMember(CSharpBinderFlags.None, propName,
                        dyn.GetType(),
                        new CSharpArgumentInfo[] {
                         CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                         CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant |
                                               CSharpArgumentInfoFlags.UseCompileTimeType, null)
                    }));
                SetterSite.Target(SetterSite, dyn, val);
            }
        }
    }
}
