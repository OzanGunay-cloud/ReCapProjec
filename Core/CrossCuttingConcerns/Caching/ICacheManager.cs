namespace Core.CrossCuttingConcerns.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        object Get(string key);
        void Add(string key, object value, int duration);
        bool IsAdd(string key); // Cache'de var mı?
        void Remove(string key); // Belirli bir cache'i sil
        void RemoveByPattern(string pattern); // Belirli bir desene uyanları sil (Örn: Get ile başlayanlar)
    }
}