using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class OrderedMultipleDictionary<TKey, TValue> : IEnumerable<TValue>, IEnumerable
    {
        private List<KeyValuePair<TKey, TValue>> _list = new List<KeyValuePair<TKey, TValue>>();
        private int? _nullKeyIndex = new int?();
        private TKey _lastKey;
        private Comparison<TKey> _compare;
        private IEqualityComparer<TKey> _keyEqualityComparer;
        private IEqualityComparer<TValue> _valueEqualityComparer;
        private Dictionary<TKey, int> _keyDictionary;

        public int Count
        {
            get
            {
                return this._list.Count;
            }
        }

        public IEnumerable<TValue> this[TKey key]
        {
            get
            {
                int startIndex;
                if (this.TryGetIndex(key, out startIndex))
                {
                    for (int i = startIndex; i < this._list.Count; ++i)
                    {
                        KeyValuePair<TKey, TValue> pair = this._list[i];
                        if (!this._keyEqualityComparer.Equals(key, pair.Key))
                            break;
                        yield return pair.Value;
                    }
                }
            }
        }

        public OrderedMultipleDictionary(Comparison<TKey> sortKeyComparison)
        {
            this._compare = sortKeyComparison;
            this._keyEqualityComparer = EqualityComparer<TKey>.Default;
            this._valueEqualityComparer = EqualityComparer<TValue>.Default;
        }

        public void Add(TKey key, TValue value)
        {
            KeyValuePair<TKey, TValue> keyValuePair = new KeyValuePair<TKey, TValue>(key, value);
            if (this._compare(key, this._lastKey) < 0)
            {
                for (int index = 0; index < this._list.Count; ++index)
                {
                    if (this._compare(key, this._list[index].Key) < 0)
                    {
                        this._list.Insert(index, keyValuePair);
                        this.InvalidateKeyDictionary();
                        return;
                    }
                }
            }
            this._lastKey = key;
            this._list.Add(keyValuePair);
            this.InvalidateKeyDictionary();
        }

        public bool Remove(TValue value)
        {
            if (this._list.Count == 1)
                this.Clear();
            for (int index = 0; index < this._list.Count; ++index)
            {
                if (this._valueEqualityComparer.Equals(this._list[index].Value, value))
                {
                    this._list.RemoveAt(index);
                    this.InvalidateKeyDictionary();
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            this._list.Clear();
            this._lastKey = default(TKey);
            this.InvalidateKeyDictionary();
        }

        private void InvalidateKeyDictionary()
        {
            this._keyDictionary = null;
            this._nullKeyIndex = new int?();
        }

        private Dictionary<TKey, int> GetKeyDictionary()
        {
            Dictionary<TKey, int> dictionary = new Dictionary<TKey, int>();
            TKey y = default(TKey);
            for (int index = 0; index < this._list.Count; ++index)
            {
                KeyValuePair<TKey, TValue> keyValuePair = this._list[index];
                if (!this._keyEqualityComparer.Equals(keyValuePair.Key, y) || index == 0)
                {
                    if (keyValuePair.Key == null)
                        this._nullKeyIndex = new int?(index);
                    else
                        dictionary[keyValuePair.Key] = index;
                    y = keyValuePair.Key;
                }
            }
            return dictionary;
        }

        private bool TryGetIndex(TKey key, out int value)
        {
            if (this._keyDictionary == null)
                this._keyDictionary = this.GetKeyDictionary();
            if (key == null)
            {
                if (this._nullKeyIndex.HasValue)
                {
                    value = this._nullKeyIndex.Value;
                    return true;
                }
            }
            else if (this._keyDictionary.TryGetValue(key, out value))
                return true;
            value = 0;
            return false;
        }

        private IEnumerable<TValue> GetValues()
        {
            foreach (KeyValuePair<TKey, TValue> keyValuePair in this._list)
                yield return keyValuePair.Value;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return this.GetValues().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetValues().GetEnumerator();
        }
    }
}
