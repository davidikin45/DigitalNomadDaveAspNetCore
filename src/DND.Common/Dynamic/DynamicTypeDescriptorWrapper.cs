using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DND.Common.Dynamic
{
    //https://weblogs.asp.net/bleroy/fun-with-c-4-0-s-dynamic
    public class DynamicTypeDescriptorWrapper : ICustomTypeDescriptor
    {
        private IDynamicMetaObjectProvider _dynamic;

        public DynamicTypeDescriptorWrapper(IDynamicMetaObjectProvider dynamicObject)
        {
            _dynamic = dynamicObject;
        }

        public DynamicTypeDescriptorWrapper()
        {
            _dynamic = new ExpandoObject();
        }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return new AttributeCollection();
        }

        public string GetClassName()
        {
            return "dynamic";
        }

        public string GetComponentName()
        {
            return "Dynamic";
        }

        public TypeConverter GetConverter()
        {
            return null;
        }

        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(new EventDescriptor[] { });
        }

        public EventDescriptorCollection GetEvents()
        {
            return new EventDescriptorCollection(new EventDescriptor[] { });
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        private Dictionary<string, DynamicPropertyDescriptor> properties = new Dictionary<string, DynamicPropertyDescriptor>();
        public PropertyDescriptorCollection GetProperties()
        {
            var meta = _dynamic.GetMetaObject(Expression.Constant(_dynamic));
            var memberNames = meta.GetDynamicMemberNames();

            var props = new PropertyDescriptorCollection(new PropertyDescriptor[] { });

            foreach (var memberName in memberNames)
            {
                if (!properties.ContainsKey(memberName))
                {
                    var newProperty = new DynamicPropertyDescriptor(_dynamic, memberName);
                    properties.Add(memberName, newProperty);
                }

                props.Add(properties[memberName]);
            }

            return props;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _dynamic;
        }

        #endregion

        public void AddProperty(string property, object value)
        {
            if(!(_dynamic is IDictionary<string, object>))
            {
                throw new Exception("Can only add properties to Expando Objects");
            }

            var dict = _dynamic as IDictionary<string, object>;
            AddProperty(dict, property, value);
        }

        private void AddProperty(IDictionary<string, object> dict, string property, object value)
        {
            dict.Add(property, value);
        }

        public void RemoveProperty(string property)
        {
            if (!(_dynamic is IDictionary<string, object>))
            {
                throw new Exception("Can only remove properties from Expando Objects");
            }

            var dict = _dynamic as IDictionary<string, object>;
            RemoveProperty(dict, property);
        }

        private void RemoveProperty(IDictionary<string, object> dict, string property)
        {
            dict.Remove(property);
        }

        public object this[string key]
        {
            get
            {
                return GetProperties().Find(key, true).GetValue(_dynamic);
            }
            set
            {
                var property = GetProperties().Find(key, true);

                if (IsCollection(property.PropertyType))
                {
                    if(!(property.PropertyType.GetGenericArguments()[0] == typeof(FormFile) && !(value is FormFile)))
                    {
                        var convertedValue = Convert.ChangeType(value, property.PropertyType.GetGenericArguments()[0]);
                        var collection = property.GetValue(_dynamic);
                        var genericCollectionType = typeof(ICollection<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                        var addMethod = genericCollectionType.GetMethod("Add");
                        addMethod.Invoke(collection, new object[] { convertedValue });
                    }
                }
                else if(property.PropertyType == typeof(DateTime))
                {
                    if(!String.IsNullOrWhiteSpace(value.ToString()))
                    {
                        var convertedValue = Convert.ChangeType(value, property.PropertyType);
                        property.SetValue(_dynamic, convertedValue);
                    }
                    else
                    {
                        property.SetValue(_dynamic, new DateTime());
                    }
                }
                else if (property.PropertyType == typeof(FormFile))
                {
                    if(!string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        property.SetValue(_dynamic, value);
                    }
                }
                else
                {
                    var convertedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(_dynamic, convertedValue);
                }
            }
        }

        private bool IsCollection(Type type)
        {
            return type.GetInterfaces().Where(x => x.GetTypeInfo().IsGenericType).Any(x => x.GetGenericTypeDefinition() == typeof(ICollection<>) && !x.GetGenericArguments().Contains(typeof(Byte)));
        }

        public bool ContainsProperty(string property)
        {
            return GetProperties().Find(property, true) != null;
        }

        public void AddAttribute<TAttribute>(string propertyName, TAttribute attribute) where TAttribute : Attribute
        {
            var property = (DynamicPropertyDescriptor)GetProperties()[propertyName];
            properties.Remove(propertyName);
            var newPropertyWithAttribute = new DynamicPropertyDescriptor(property, new Attribute[] { attribute });
            properties.Add(propertyName, newPropertyWithAttribute);
        }
    }
}
