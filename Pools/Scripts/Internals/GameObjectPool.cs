using System;
using System.Collections.Generic;
using Debri.Common;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Debri.Pools.Internals
{
  internal class GameObjectPool : IObjectPool<GameObject>, IPrewarmable
  {
    private readonly GameObject _prototype;
    private readonly Transform _container;
    private readonly List<GameObjectPoolItem> _items;
    private bool _isContainerDestroyed;

    public GameObjectPool(GameObject prototype, Transform container)
    {
      if (!container)
        throw new InvalidOperationException("Container must be specified");

      _prototype = prototype;
      _container = container;
      _items = new List<GameObjectPoolItem>();
      _container.gameObject.SubscribeToOnDestroy(() => _isContainerDestroyed = true);
    }

    public PooledObject<GameObject> Get(out GameObject instance) =>
      new(instance = Get(), this);

    public GameObject Get()
    {
      if (_isContainerDestroyed)
        throw new InvalidOperationException($"Can't return pool item {_prototype.name} because container has been destroyed");

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
      if (_isContainerDestroyed)
      {
        Object.Destroy(item.gameObject);
        return;
      }

      item.transform.SetParent(_container, worldPositionStays: false);
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

    public void LogWarning(object message) =>
      Debug.LogWarning(message, _prototype);
  }
}