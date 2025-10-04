using System;
using System.Collections;
using System.Collections.Generic;

namespace Debri.Collections
{
  public class Repository<TItem> : IRepository<TItem>
  {
    private readonly HashSet<TItem> _items = new();

    public event Action<TItem> OnAdded;
    public event Action<TItem> OnRemoved;

    public void Add(TItem item)
    {
      if (!_items.Add(item))
        throw new ArgumentException("Item already exists");

      OnAdded?.Invoke(item);
    }

    public void Remove(TItem item)
    {
      if (!_items.Remove(item))
        throw new ArgumentException("Item does not exist");

      OnRemoved?.Invoke(item);
    }

    public IEnumerator<TItem> GetEnumerator() =>
      _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
      GetEnumerator();
  }
}