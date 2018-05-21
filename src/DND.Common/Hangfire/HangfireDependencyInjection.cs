using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Hangfire
{
    public class HangfireDependencyInjectionActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public HangfireDependencyInjectionActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
