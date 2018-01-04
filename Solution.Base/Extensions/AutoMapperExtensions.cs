using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Extensions
{
    public static class AutomapperExtensions
    {
        public static TDestination MapTo<TDestination>(this object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public static IEnumerable<TDestination> MapTo<TDestination>(this IEnumerable<object> source)
        {
            List<TDestination> list = new List<TDestination>();
            foreach (object o in source)
            {
                list.Add(o.MapTo<TDestination>());
            }
            return list;
        }

        public static MemberInfo GetDestinationMappedProperty(this IMapper mapper, Type sourceType, Type destinationType, string sourcePropertyName)
        {
            MemberInfo sourceProperty = sourceType.GetProperty(sourcePropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return GetDestinationMappedProperty(mapper, sourceType, destinationType, sourceProperty);
        }

        public static MemberInfo GetDestinationMappedProperty(this IMapper mapper, Type sourceType, Type destinationType, MemberInfo sourceProperty)
        {
            var map = mapper.ConfigurationProvider.FindTypeMapFor(sourceType, destinationType);

            PropertyMap propmap = map
            .GetPropertyMaps()
            .SingleOrDefault(m =>
                m.SourceMember != null &&
                m.SourceMember.MetadataToken == sourceProperty.MetadataToken);

            if (propmap == null)
            {
                throw new Exception(
                    string.Format(
                    "Can't map selector. Could not find a PropertyMap for {0}", sourceProperty.Name));
            }

            return propmap.DestinationProperty;
        }
    }
}
