using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QueueServiceWebApp.Controllers
{
    public class ControllerHelpers
    {
        public static bool CheckIfBase64(string messageIdBase64)
        {
            if (string.IsNullOrEmpty(messageIdBase64)) return false;

            var messageId = Base64ConvertFrom(messageIdBase64);
            if (messageId == null) return false;

            return true;
        }

        public static string Base64ConvertFrom(string base64)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }

        public static string Base64ConvertTo(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public static object MapFormDataProperties(Type mappedClass, NameValueCollection collection)
        {
            var instance = Activator.CreateInstance(mappedClass);
            var formkeys = collection.AllKeys;
            var properties = instance.GetType().GetProperties();
            var formKeyList = new List<string>();
            foreach (var formKey in formkeys)
                formKeyList.Add(formKey);
            var  propertyMatched = false;

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    if (formKeyList.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        instance.GetType()
                            .GetProperty(property.Name, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public)
                            .SetValue(instance, collection[property.Name]);
                        
                        propertyMatched = true;
                    }
                }
                else if (property.PropertyType == typeof(string[]))
                {
                   if (formKeyList.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        instance.GetType()
                            .GetProperty(property.Name, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public)
                            .SetValue(instance, collection[property.Name].Split(','));
                        
                        propertyMatched = true;
                    }
                }
                else
                {
                    var subClass = MapFormDataProperties(property.PropertyType, collection);
                    if (subClass != null) propertyMatched = true;

                    instance.GetType().GetProperty(property.Name).SetValue(instance, subClass);
                }
            }

            if (propertyMatched) return instance;

            return null;
        }
    }
}