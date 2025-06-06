using System;
using System.Collections.Generic;

public class MultiMap<K, V> : SortedDictionary<K, List<V>>
{
    private readonly List<V> Empty = new List<V>();

    public void Add(K k, V v)
    {
        List<V> list;
        this.TryGetValue(k, out list);
        if (list == null)
        {
            list = new List<V>();
            this.Add(k, list);
        }
        list.Add(v);
    }

    public bool Remove(K key, V value)
    {
        List<V> list;
        this.TryGetValue(key, out list);
        if (list == null)
        {
            return false;
        }
        if (!list.Remove(value))
        {
            return false;
        }
        if (list.Count == 0)
        {
            this.Remove(key);
        }
        return true;
    }

    /// <summary>
    /// 不返回内部的list,copy一份出来
    /// </summary>
    public V[] GetAll(K key)
    {
        List<V> list;
        this.TryGetValue(key, out list);
        if (list == null)
        {
            return Array.Empty<V>();
        }
        return list.ToArray();
    }

    /// <summary>
    /// 返回内部的list
    /// </summary>
    public new List<V> this[K key]
    {
        get
        {
            this.TryGetValue(key, out List<V> list);
            return list ?? Empty;
        }
    }

    public V GetOne(K key)
    {
        List<V> list;
        this.TryGetValue(key, out list);
        if (list != null && list.Count > 0)
        {
            return list[0];
        }
        return default;
    }

    public bool Contains(K key, V value)
    {
        List<V> list;
        this.TryGetValue(key, out list);
        if (list == null)
        {
            return false;
        }
        return list.Contains(value);
    }
}