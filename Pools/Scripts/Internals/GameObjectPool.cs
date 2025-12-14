using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Debri.Pools.Internals
{
  internal class GameObjectPool : IObjectPool<GameObject>, IPrewarmable
  {
    private readonly GameObject _prototype;
    private Transform _container;
    private readonly List<GameObjectPoolItem> _items;

    public GameObjectPool(GameObject prototype)
    {
      _prototype = prototype;
      _container = _prototype.transform.parent ? _prototype.transform.parent : GlobalPools.DefaultPoolItemsParent;
      _items = new List<GameObjectPoolItem>();

      if (_container == GlobalPools.DefaultPoolItemsParent)
        GlobalPools.OnDefaultPoolItemsParentChanged += () => _container = GlobalPools.DefaultPoolItemsParent;
      SceneManager.sceneUnloaded += RemoveInvalids;
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

    private void RemoveInvalids(Scene _) =>
      _items.RemoveAll(item => !item);

    void IPrewarmable.Prewarm(int count)
    {
      for (int i = CountInactive; i < count; i++)
        _items.Add(InstantiateItem());
    }

    public void LogWarning(object message) =>
      Debug.LogWarning(message, _prototype);
  }
}