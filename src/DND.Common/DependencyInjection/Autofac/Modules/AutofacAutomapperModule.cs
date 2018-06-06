using Autofac;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions;
using DND.Common.Automapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DependencyInjection.Autofac.Modules
{
    public class AutofacAutomapperModule : Module
    {
       
        public Func<System.Reflection.Assembly, Boolean> Filter;

        protected override void Load(ContainerBuilder builder)
        {
            var config = new MapperConfiguration(cfg => {
                //!!!!!!IMPORTANT to map delegates with AutoMapper
                cfg.AddExpressionMapping();
                new AutoMapperConfiguration(cfg, Filter);
            });

            builder.RegisterInstance(config).As<MapperConfiguration>();
            builder.Register(ctx => config).As<IConfigurationProvider>();
            builder.Register(ctx => new ExpressionBuilder(config)).As<IExpressionBuilder>();
            builder.Register(c => config.CreateMapper()).As<IMapper>().SingleInstance();

       
        }
    }


}
