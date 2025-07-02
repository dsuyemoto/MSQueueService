using LoggerHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSMQHandlerService
{
    public class Helpers
    {
        public static string Base64ConvertFrom(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return null;

            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }

        public static string Base64ConvertTo(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return null;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
        public static string Base64ConvertTo(byte[] byteArray)
        {
            if (byteArray == null) return null;

            return Convert.ToBase64String(byteArray);
        }
        public static string[][] DictionaryToArray(Dictionary<string, string> dictionary)
        {
            var arrayList = new List<string[]>();

            if (dictionary == null) return arrayList.ToArray();

            foreach (var pair in dictionary)
                arrayList.Add(new string[] { pair.Key, pair.Value });

            return arrayList.ToArray();
        }

        public static object[][] DictionaryToArray(Dictionary<string, object> dictionary)
        {
            var arrayList = new List<object[]>();

            if (dictionary == null) return arrayList.ToArray();

            foreach (var pair in dictionary)
                arrayList.Add(new object[] { pair.Key, pair.Value });

            return arrayList.ToArray();
        }

        public static Dictionary<string, string> ArrayToDictionary(string[][] arrays)
        {
            var dictionary = new Dictionary<string, string>();

            if (arrays == null) return dictionary;

            foreach (var array in arrays)
                if (array.Length == 2)
                    dictionary.Add(array[0], array[1]);

            return dictionary;
        }

        public static Dictionary<string, string> ArrayToDictionary(object[][] arrays)
        {
            var dictionary = new Dictionary<string, string>();

            if (arrays == null) return dictionary;

            foreach (var array in arrays)
                if (array.Length == 2)
                    if (array[1] is string)
                        dictionary.Add((string)array[0], (string)array[1]);

            return dictionary;
        }
    }
}
