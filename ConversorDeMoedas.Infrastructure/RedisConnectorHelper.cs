﻿using ConversorDeMoedas.Infrastructure.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;

namespace ConversorDeMoedas.Infrastructure
{
    public class RedisConnectorHelper : IRedisConnectorHelper
    {
        private IDistributedCache cache;

        public RedisConnectorHelper(IDistributedCache cache)
        {
             this.cache = cache;
        }
        public T Get<T>(String cacheKey)
        {
            return Deserialize<T>(cache.Get(cacheKey));
        }
        public byte[] Get(String cacheKey)
        {
            return cache.Get(cacheKey);
        }
        public String GetString(String cacheKey)
        {
            return cache.GetString(cacheKey);
        }
        public void Set(String cacheKey, object cacheValue)
        {
            cache.Set(cacheKey, Serialize(cacheValue));
        }
        public void Set(String cacheKey, object cacheValue, Int32 TempoEmMinutos)
        {
             DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions();
             distributedCacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(TempoEmMinutos));

            cache.Set(cacheKey, Serialize(cacheValue), distributedCacheEntryOptions);
        }
        public void SetString(String cacheKey, String cacheValue)
        {
             cache.SetString(cacheKey, cacheValue);
        }
        public void SetString(String cacheKey, String cacheValue, Int32 TempoEmMinutos)
        {
             DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions();
             distributedCacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(TempoEmMinutos));

             cache.SetString(cacheKey, cacheValue, distributedCacheEntryOptions);
        }
        private static byte[] Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            BinaryFormatter objBinaryFormatter = new BinaryFormatter();
            using (MemoryStream objMemoryStream = new MemoryStream())
            {
                objBinaryFormatter.Serialize(objMemoryStream,obj);
                byte[] objDataAsByte = objMemoryStream.ToArray();
                return objDataAsByte;
            }
        }
        private static T Deserialize<T>(byte[] bytes)
        {
            BinaryFormatter objBinaryFormatter = new BinaryFormatter();
            if (bytes == null)
                return default(T);

            using (MemoryStream objMemoryStream = new MemoryStream(bytes))
            {
                T result = (T)objBinaryFormatter.Deserialize(objMemoryStream);
                return result;
            }
        }
    }
}
