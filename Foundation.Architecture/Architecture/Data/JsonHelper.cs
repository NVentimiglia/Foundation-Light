// Nicholas Ventimiglia 2016-09-05
using System;
#if !UNITY
using Newtonsoft.Json;
#endif

namespace Foundation.Architecture
{
    /// <summary>
    /// Json serialization proxy. Platform agnostic.
    /// </summary>
    public class JsonHelper
    {
#if !UNITY
        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object FromJson(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static string ToJson(object instance)
        {
            return JsonConvert.SerializeObject(instance);
        }
#else
        public static T FromJson<T>(string json)
        {
            return UnityEngine.JsonUtility.FromJson<T>(json);
        }

        public static object FromJson(string json, Type type)
        {
            return UnityEngine.JsonUtility.FromJson(json, type);
        }

        public static string ToJson(object instance)
        {
            return UnityEngine.JsonUtility.ToJson(instance);
        }
#endif
    }
}
