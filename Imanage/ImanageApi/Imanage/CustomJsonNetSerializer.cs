using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;
using System;

namespace Imanage
{
    public class CustomJsonNetSerializer : IRestSerializer
    {
        public string[] SupportedContentTypes { get; } =
        {
            "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
        };

        public DataFormat DataFormat { get; } = DataFormat.Json;

        public string ContentType { get; set; } = "application/json";

        public string Serialize(object obj)
        {
            var result = "{";
            var properties = obj.GetType().GetProperties();
            for (var i = 0; i< properties.Length; i++)
            {
                if (properties[i].GetValue(obj, null) != null)
                {
                    var separator = string.Empty;
                    if (i != properties.Length - 1)
                        separator = ",";
                    var objvalue = properties[i].GetValue(obj, null);
                    if (objvalue == null)
                    {
                        result = result + "\"" + properties[i].Name.ToLower() + "\"" + separator;
                    }
                    else if (objvalue is string)
                    {
                        result = result + "\"" + properties[i].Name.ToLower() 
                            + "\":\"" + objvalue.ToString() + "\"" + separator;
                    }
                    else if (objvalue is DateTime)
                    {
                        var value = JsonConvert.SerializeObject(
                            objvalue,
                            new JsonSerializerSettings()
                            {
                                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ"
                            });
                        result = result + "\"" + properties[i].Name.ToLower() + "\":" + value + separator;
                    }
                    else if (objvalue is int)
                    {
                        result = result + "\"" + properties[i].Name.ToLower() + "\":" + objvalue + separator;
                    }
                    else if (objvalue is object)
                    {
                        result = result + "\"" + properties[i].Name.ToLower() + "\":" + Serialize(objvalue) + separator;
                    }
                }
            }

            return result + "}";
        }

        public T Deserialize<T>(IRestResponse response) =>
            JsonConvert.DeserializeObject<T>(response.Content);

        [Obsolete]
        public string Serialize(Parameter parameter)
        {
            return Serialize(parameter.Value);
        }
    }
}
