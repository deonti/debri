using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Debri.Pools.Internals
{
  internal readonly struct GameObjectPool : IObjectPool<GameObject>, IPrewarmable
  {
    private readonly GameObject _prototype;
    private readonly Transform _container;
    private readonly List<GameObjectPoolItem> _items;

    public GameObjectPool(GameObject prototype)
    {
      _prototype = prototype;
      _container = _prototype.transform.parent ? _prototype.transform.parent : GlobalPools.DefaultPoolItemsParent;
      _items = new List<GameObjectPoolItem>();
    }

    public PooledObject<GameObject> Get(out GameObject instance) =>
      new(instance = Get(), this);

    public GameObject Get()
    {
      GameObjectPoolItem item;
      if (_items.Count == 0)
        item = InstantiateItem();
      else
      {
        item = _items[^1];
        _items.Remove(item);
      }

      item.ProcessGet();
      return item.gameObject;
    }

    public void Release(GameObject instance)
    {
      if (!instance) return;

      var item = instance.GetComponent<GameObjectPoolItem>();
      item.ProcessRelease();
      _items.Add(item);
    }

    public void Remove(GameObjectPoolItem item) =>
      _items.Remove(item);

    public void Clear()
    {
      foreach (GameObjectPoolItem item in _items.ToArray())
        Object.Destroy(item.gameObject);

      if (_items.Count != 0)
      {
        Debug.LogError("Not all items were removed by destruction.");
        _items.Clear();
      }
    }

    public int CountInactive =>
      _items.Count;

    private GameObjectPoolItem InstantiateItem() =>
      GameObjectPoolItem.Instantiate(this, _prototype, _container);

    void IPrewarmable.Prewarm(int count)
    {
      for (int i = CountInactive; i < count; i++)
        _items.Add(InstantiateItem());
    }
  }
}