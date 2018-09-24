using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace TRAC.Intranet.Services
{
    public class CacheLayer
    {
        static readonly ObjectCache cache = MemoryCache.Default;
        public static T Get<T>(string key) where T : class
        {
            try
            {
                return (T)cache[key];
            }
            catch
            {
                return null;
            }
        }
        public static void Add<T>(T objectToCache, string key) where T : class
        {
            cache.Add(key, objectToCache, DateTime.Now.AddHours(6));
        }
    }
}
