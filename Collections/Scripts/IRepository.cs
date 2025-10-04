using System;
using System.Collections.Generic;

namespace Debri.Collections
{
  public interface IRepository<TItem> : IEnumerable<TItem>
  {
    event Action<TItem> OnAdded;
    event Action<TItem> OnRemoved;
    void Add(TItem item);
    void Remove(TItem item);
  }
}