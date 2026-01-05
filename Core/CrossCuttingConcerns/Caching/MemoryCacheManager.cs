using Core.Utilities.IoC;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions; // Regex kütüphanesi kalsa da olur ama aşağıda kullanmadık

namespace Core.CrossCuttingConcerns.Caching.Microsoft
{
    public class MemoryCacheManager : ICacheManager
    {
        private IMemoryCache _memoryCache;

        public MemoryCacheManager()
        {
            _memoryCache = ServiceTool.ServiceProvider.GetService<IMemoryCache>();
        }

        public void Add(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration));
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }

        public bool IsAdd(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            // 1. ADIM: MemoryCache'in iç yapısına (CoherentState) ulaşmaya çalış
            var coherentStateField = typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);
            var coherentState = coherentStateField?.GetValue(_memoryCache);

            // 2. ADIM: Kayıtların tutulduğu listeyi (Entries) bulmaya çalış
            var entriesCollection = coherentState?.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var entriesField = coherentState?.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance)
                            ?? coherentState?.GetType().GetField("_allEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            var cacheEntriesCollection = entriesCollection?.GetValue(coherentState) ?? entriesField?.GetValue(coherentState);

            // Eski .NET sürümleri için fallback
            if (cacheEntriesCollection == null)
            {
                var entriesFieldLegacy = typeof(MemoryCache).GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
                cacheEntriesCollection = entriesFieldLegacy?.GetValue(_memoryCache);
            }

            if (cacheEntriesCollection == null) return;

            var keysToRemove = new List<object>();

            // 3. ADIM: Koleksiyonu gez ve eşleşenleri listeye ekle
            // BURADA Regex YERİNE Contains KULLANIYORUZ - KESİN ÇÖZÜM

            if (cacheEntriesCollection is System.Collections.IDictionary dictionary)
            {
                foreach (System.Collections.DictionaryEntry item in dictionary)
                {
                    // Anahtarın içinde pattern (örn: "Get") geçiyor mu diye basitçe bakıyoruz
                    if (item.Key.ToString().Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        keysToRemove.Add(item.Key);
                    }
                }
            }
            else
            {
                // Dynamic olarak deniyoruz
                try
                {
                    foreach (var item in (dynamic)cacheEntriesCollection)
                    {
                        var keyProperty = item.GetType().GetProperty("Key");
                        var key = keyProperty?.GetValue(item);

                        if (key != null && key.ToString().Contains(pattern, StringComparison.OrdinalIgnoreCase))
                        {
                            keysToRemove.Add(key);
                        }
                    }
                }
                catch { /* Hata yutulur */ }
            }

            // 4. ADIM: Bulunanları sil
            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }
        }
    }
}