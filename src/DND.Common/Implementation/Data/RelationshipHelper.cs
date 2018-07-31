using DND.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Data
{
    public static class RelationshipHelper
    {
        //https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/implement-value-objects
        //1. Never have Many-To-Many relationships. Create a join Entity so the relationship becomes 1-To-Many and Many-To-1.
        //2. Never have 1-To-1 Composition ENTITY Relationships!!! For Composition relationships use Value types instead if required. In EF6 extend from BaseValueObject. In EF Core decorate value type with [OwnedAttribute]. Not sure if this attribute can be applied on base class. ValueTypes are included by default.
        //3. Use collections only for Composition/Owned relationships(1-To-Many, child cannot exist independent of the parent, ) not Aggregation/Associated relationshiships(child can exist independently of the parent, reference relationship). Never use a Collection for Navigation purposes!!!!
        //4. Use Complex properties for Aggregation relationships only (Many-To-1, child can exist independently of the parent, reference relationship). e.g Navigation purposes
        public static List<string> GetAllCompositionAndAggregationRelationshipPropertyIncludes(bool compositionRelationshipsOnly, Type type, string path = null)
        {
            List<string> includesList = new List<string>();

            List<Type> excludeTypes = new List<Type>()
            {
                typeof(DateTime),
                typeof(String),
                typeof(byte[]),
                typeof(DbGeography)
           };

            IEnumerable<PropertyInfo> properties = type.GetProperties().Where(p => p.CanWrite && !p.PropertyType.IsValueType && !excludeTypes.Contains(p.PropertyType) && ((!compositionRelationshipsOnly && !p.PropertyType.IsCollection()) || (p.PropertyType.IsCollection() && type != p.PropertyType.GetGenericArguments().First()))).ToList();

            foreach (var p in properties)
            {
                var includePath = !string.IsNullOrWhiteSpace(path) ? path + "." + p.Name : p.Name;

                includesList.Add(includePath);

                Type propType = null;
                if (p.PropertyType.IsCollection())
                {
                    propType = type.GetGenericArguments(p.Name).First();
                }
                else
                {
                    propType = p.PropertyType;
                }

                includesList.AddRange(GetAllCompositionAndAggregationRelationshipPropertyIncludes(compositionRelationshipsOnly, propType, includePath));
            }

            return includesList;
        }
    }
}
