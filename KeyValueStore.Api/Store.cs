using System.Collections.Concurrent;

namespace KeyValueStore.Api;

public static class Store
{
    private static readonly ConcurrentDictionary<string, string> _items = new();

    public static string[] GetKeys() => [.. _items.Keys];

    public static bool GetValue(string key, out string? value)
    {
        return _items.TryGetValue(key, out value);
    }

    public static void SetValue(string key, string value)
    {
        _items.AddOrUpdate(key, value, (k, v) => value);
    }

    public static void RemoveValue(string key)
    {
        _items.TryRemove(key, out var _);
    }
}
