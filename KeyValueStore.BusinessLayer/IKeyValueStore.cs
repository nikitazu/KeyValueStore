using System.Collections.Generic;

namespace KeyValueStore.BusinessLayer
{
    public interface IKeyValueStore
    {
        ICollection<string> GetKeys();
        bool TryGetValue(string key, out string value);
        void SetValue(string key, string value);
        void DeleteKey(string key);
    }
}
