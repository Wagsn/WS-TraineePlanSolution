#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：WS.Text
* 项目描述 ：
* 类 名 称 ：SafeMap
* 类 描 述 ：
* 所在的域 ：DESKTOP-KA4M82K
* 命名空间 ：WS.Text
* 机器名称 ：DESKTOP-KA4M82K 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：wagsn
* 创建时间 ：2018/11/28 11:52:08
* 更新时间 ：2018/11/28 11:52:08
* 版 本 号 ：v1.0.0.0
//----------------------------------------------------------------*/
#endregion
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WS.Text
{
    /// <summary>
    /// 安全的映射表，不存在的Key返回默认的Value，string返回string.Empty
    /// </summary>
    public class SafeMap<TValue>
    {
        /// <summary>
        /// 获取Keys
        /// </summary>
        public IEnumerable<string> Keys
        {
            get
            {
                return kvs.Keys;
            }
        }

        private Dictionary<string, TValue> kvs = new Dictionary<string, TValue>();

        /// <summary>
        /// 采用JSON字符串初始化对象
        /// 未测试
        /// </summary>
        /// <param name="json"></param>
        public static SafeMap<TV> New<TV>(string json)
        {
            SafeMap<TV> map = new SafeMap<TV>();

            JObject jo = (JObject)JsonConvert.DeserializeObject(json);
            foreach (var prop in jo)
            {
                map.kvs.Add(prop.Key, prop.Value.Value<TV>());
            }
            return map;
        }

        /// <summary>
        /// 通过映射表（字典）来创建SafeMap
        /// </summary>
        /// <typeparam name="TV">保存的类型</typeparam>
        /// <param name="pairs">映射表（字典）</param>
        /// <returns></returns>
        public static SafeMap<TV> New<TV>([Required]Dictionary<string, TV> pairs)
        {
            SafeMap<TV> map = new SafeMap<TV>
            {
                kvs = pairs
            };
            return map;
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public TValue this[string index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                Set(index, value);
            }
        }

        /// <summary>
        /// 获取Key对应的值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue Get(string index)
        {
            if (kvs.ContainsKey(index))
            {
                return kvs[index];
            }
            else
            {
                return default(TValue);
            }
        }

        /// <summary>
        /// 不安全的获取
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue UnSafeGet(string index)
        {
            return kvs[index];
        }

        /// <summary>
        /// 设置Key对应的值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Set(string index, TValue value)
        {
            kvs[index] = value;
        }

        /// <summary>
        /// 包含索引
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public bool ContainsKey(string index)
        {
            return kvs.ContainsKey(index);
        }
    }
}
