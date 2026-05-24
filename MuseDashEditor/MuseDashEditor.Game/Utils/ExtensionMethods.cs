using System;
using System.Collections.Generic;

namespace MuseDashEditor.Game.Utils;

public static class ExtensionMethods
{
    public static TValue ComputeIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        Func<TKey, TValue> valueFactory)
    {
        TValue value = dictionary.TryGetValue(key, out var existingValue) ? existingValue : valueFactory(key);
        dictionary[key] = value;
        return value;
    }
}
