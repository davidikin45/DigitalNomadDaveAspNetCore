﻿using DND.Common.ModelMetadataCustom.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.ModelMetadataCustom.LinkAttributes
{
    [AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true,Inherited = true)]
    public class DbWhereClauseEqualsAttribute : Attribute, IDisplayMetadataAttribute
    {
        public string Property { get; set; }
        public object Equals { get; set; }

        public DbWhereClauseEqualsAttribute(string property, object equals)
        {
            Property = property;
            Equals = equals;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            Dictionary<string, List<object>> whereClauseDictionary = null;

            if(modelMetadata.AdditionalValues.ContainsKey("WhereClauseEqualsDictionary"))
            {
                whereClauseDictionary = (Dictionary<string, List<object>>)modelMetadata.AdditionalValues["WhereClauseEqualsDictionary"];
            }

            if(whereClauseDictionary ==  null)
            {
                whereClauseDictionary = new Dictionary<string, List<object>>();
            }

            if(!whereClauseDictionary.ContainsKey(Property))
            {
                whereClauseDictionary.Add(Property, new List<object>());
            }

            whereClauseDictionary[Property].Add(Equals);

            modelMetadata.AdditionalValues["WhereClauseEqualsDictionary"] = whereClauseDictionary;
        }
    }
}
