using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ModelHelpers
    {
        public static string[][] DictionaryToArray(Dictionary<string, string> dictionary)
        {
            if (dictionary == null) return null;

            var arrayList = new List<string[]>();

            if (dictionary == null) return arrayList.ToArray();

            foreach (var pair in dictionary)
                arrayList.Add(new string[] { pair.Key, pair.Value });

            return arrayList.ToArray();
        }

        public static Dictionary<string, string> ArrayToDictionary(string[][] arrays)
        {
            if (arrays == null) return null;

            var dictionary = new Dictionary<string, string>();

            if (arrays == null) return dictionary;

            foreach (var array in arrays)
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