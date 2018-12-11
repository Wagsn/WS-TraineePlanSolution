using System;

using Newtonsoft.Json;

namespace WS.Text
{
    /// <summary>
    /// JSON助手（序列化工具），使用Newtonsoft.Json
    /// </summary>
    public static class JsonUtil
    {
        private static JsonSerializerSettings setting = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateFormatString = "yyyy-MM-dd HH:mm:ss.FFFFFFK",
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };

        static JsonUtil()
        {
            setting.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        /// <summary>
        /// 如果传入的对象为Null则返回空字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            if (obj == null)
                return "";

            return JsonConvert.SerializeObject(obj, setting);
        }

        /// <summary>
        /// 将JSON字符串转化成C#对象
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToObject(string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(json, type, setting);
        }

        /// <summary>
        /// 将JSON字符串转化成C#对象，通过泛型
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static TObject ToObject<TObject>(string json)
        {
            return (TObject)ToObject(json, typeof(TObject));
        }
    }
}
