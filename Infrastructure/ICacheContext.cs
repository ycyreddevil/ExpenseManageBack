using System;

namespace yuyu.Infrastructure
{
    /// <summary>
    /// 缓存
    /// </summary>
    public abstract class ICacheContext
    {
        public abstract T Get<T>(string key) ;

        public abstract bool Set<T>(string key, T t, DateTime expire);

        public abstract bool Remove(string key);
    }
}