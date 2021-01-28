using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KeyValueStore.BusinessLayer
{
    public class InMemoryKeyValueStore : IKeyValueStore
    {
        private readonly ConcurrentDictionary<string, string> _store = new ConcurrentDictionary<string, string>();

        ICollection<string> IKeyValueStore.GetKeys()
        {
            return _store.Keys;
        }

        bool IKeyValueStore.TryGetValue(string key, out string value)
        {
            bool result = _store.TryGetValue(key, out string storedValue);
            value = storedValue;
            return result;
        }

        void IKeyValueStore.SetValue(string key, string value)
        {
            _store.AddOrUpdate(key, value, (oldValue, newValue) => newValue);
        }

        void IKeyValueStore.DeleteKey(string key)
        {
            _store.TryRemove(key, out string removedValue);
        }
    }
}
