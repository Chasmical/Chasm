#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NET45_OR_GREATER
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

#if !NETCOREAPP3_0_OR_GREATER
#pragma warning disable CS8767
#endif

namespace Chasm.Collections
{
    public sealed class ConcurrentWeakDictionary<TKey, TValue>
        : IDictionary, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull where TValue : class
    {
        private readonly ConcurrentDictionary<TKey, WeakReference<TValue>> _dict;

        public ConcurrentWeakDictionary()
            => _dict = [];
        public ConcurrentWeakDictionary(IEqualityComparer<TKey>? comparer)
            => _dict = new(comparer);

        [Pure] public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
        {
            if (_dict.TryGetValue(key, out WeakReference<TValue>? weak))
                return weak.TryGetTarget(out value);
            value = default;
            return false;
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue? result = default;

            _dict.AddOrUpdate(
                key,
                k => new WeakReference<TValue>(result = valueFactory(k)),
                (k, weak) =>
                {
                    if (!weak.TryGetTarget(out result))
                        weak.SetTarget(result = valueFactory(k));
                    return weak;
                }
            );

            return result!;
        }
        public TValue GetOrAdd(TKey key, TValue value)
        {
            // TODO: refactor
            return GetOrAdd(key, _ => value);
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue? result = default;

            _dict.AddOrUpdate(
                key,
                k => new WeakReference<TValue>(result = addValueFactory(k)),
                (k, weak) =>
                {
                    if (weak.TryGetTarget(out TValue? prevValue))
                    {
                        result = updateValueFactory(k, prevValue);
                        if (ReferenceEquals(result, prevValue)) return weak;
                    }
                    else
                    {
                        result = addValueFactory(k);
                    }
                    weak.SetTarget(result);
                    return weak;
                }
            );

            return result!;
        }
        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            // TODO: refactor
            return AddOrUpdate(key, _ => addValue, updateValueFactory);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            bool added = false;

            _dict.AddOrUpdate(
                key,
                _ =>
                {
                    added = true;
                    return new WeakReference<TValue>(value);
                },
                (__, weak) =>
                {
                    if (!weak.TryGetTarget(out _))
                    {
                        added = true;
                        weak.SetTarget(value);
                    }
                    return weak;
                }
            );

            return added;
        }
        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            try
            {
                _dict.AddOrUpdate(
                    key,
                    _ => throw new InvalidOperationException(),
                    (_, weak) =>
                    {
                        if (
                            !weak.TryGetTarget(out TValue? prevValue) ||
                            !EqualityComparer<TValue>.Default.Equals(prevValue, comparisonValue)
                        ) throw new InvalidOperationException();

                        if (!ReferenceEquals(prevValue, newValue))
                            weak.SetTarget(newValue);
                        return weak;
                    }
                );
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        public bool TryRemove(TKey key, [NotNullWhen(true)] out TValue? value)
        {
            if (_dict.TryRemove(key, out WeakReference<TValue>? weak))
                return weak.TryGetTarget(out value);
            value = default;
            return false;
        }

        public void Clear()
            => _dict.Clear();

        [Pure] public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, WeakReference<TValue>> entry in _dict)
                if (entry.Value.TryGetTarget(out TValue? value))
                    yield return new KeyValuePair<TKey, TValue>(entry.Key, value);
        }
        [Pure] IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        [Pure] IDictionaryEnumerator IDictionary.GetEnumerator()
            => GetEnumerator().ToDictionaryEnumerator();

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue? value)) return value;
                throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
            }
            set
            {
                if (!_dict.TryGetValue(key, out WeakReference<TValue>? weak))
                    _dict[key] = new WeakReference<TValue>(value);
                else weak.SetTarget(value);
            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = GetEnumerator())
                    while (enumerator.MoveNext()) count++;
                return count;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                List<TKey> results = [];
                foreach (KeyValuePair<TKey, TValue> entry in this)
                    results.Add(entry.Key);
                return new ReadOnlyCollection<TKey>(results);
            }
        }
        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> results = [];
                foreach (KeyValuePair<TKey, TValue> entry in this)
                    results.Add(entry.Value);
                return new ReadOnlyCollection<TValue>(results);
            }
        }

        private const string Arg_WrongType = "The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.";

        [Pure] private static TKey CastKeyOrThrow(object key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            if (key is TKey tKey) return tKey;
            throw new ArgumentException(string.Format(Arg_WrongType, key, typeof(TKey)));
        }
        [Pure] private static TValue CastValueOrThrow(object? value)
        {
            if (value is null) return null!;
            return value as TValue ?? throw new ArgumentException(string.Format(Arg_WrongType, value, typeof(TValue)));
        }

        bool IDictionary.IsFixedSize => false;
        bool IDictionary.IsReadOnly => false;
        ICollection IDictionary.Keys => (ICollection)Keys;
        ICollection IDictionary.Values => (ICollection)Values;
        [Pure] bool IDictionary.Contains(object key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            return key is TKey tKey && TryGetValue(tKey, out _);
        }
        void IDictionary.Add(object key, object? value)
            => ((IDictionary<TKey, TValue>)this).Add(CastKeyOrThrow(key), CastValueOrThrow(value));
        void IDictionary.Remove(object key)
        {
            if (key is TKey tKey) TryRemove(tKey, out _);
        }
        object? IDictionary.this[object key]
        {
            get => this[CastKeyOrThrow(key)];
            set => this[CastKeyOrThrow(key)] = CastValueOrThrow(value);
        }

        [Pure] bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
            => TryGetValue(key, out _);
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new ArgumentException($"An item with the same key has already been added. Key: {key}");
        }
        bool IDictionary<TKey, TValue>.Remove(TKey key)
            => TryRemove(key, out _);
        [Pure] bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
            => TryGetValue(key, out _);
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;
        void ICollection.CopyTo(Array array, int index)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));

            if (array is KeyValuePair<TKey, TValue>[] pairs)
                ((ICollection<KeyValuePair<TKey, TValue>>)this).CopyTo(pairs, index);
            else if (array is DictionaryEntry[] entries)
            {
                foreach (KeyValuePair<TKey, TValue> entry in this)
                    entries[index++] = new DictionaryEntry(entry.Key, entry.Value);
            }
            else if (array is object[] objects)
            {
                foreach (KeyValuePair<TKey, TValue> entry in this)
                    objects[index++] = entry;
            }
            else
            {
                foreach (KeyValuePair<TKey, TValue> entry in this)
                    array.SetValue(entry, index++);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
        [Pure] bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> entry)
            => TryGetValue(entry.Key, out TValue? value) && EqualityComparer<TValue>.Default.Equals(value, entry.Value);
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> entry)
            => ((IDictionary<TKey, TValue>)this).Add(entry.Key, entry.Value);
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> entry)
            => TryRemove(entry.Key, out _);
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            foreach (KeyValuePair<TKey, TValue> entry in this)
                array[index++] = entry;
        }

    }
}
#endif
