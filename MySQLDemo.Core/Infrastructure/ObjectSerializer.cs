﻿using IdentityServer4.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Core.Infrastructure
{
    public class ObjectSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        private static readonly JsonSerializer Serializer = new JsonSerializer
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        static ObjectSerializer()
        {
            Settings.Converters.Add(new NameValueCollectionConverter());
        }

        public static string ToString(object o)
        {
            return JsonConvert.SerializeObject(o, Settings);
        }

        public static T FromString<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, Settings);
        }

        public static JObject ToJObject(object o)
        {
            return JObject.FromObject(o, Serializer);
        }
    }
}
