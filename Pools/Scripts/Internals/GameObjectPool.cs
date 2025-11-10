using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Debri.Pools.Internals
{
  internal readonly struct GameObjectPool : IObjectPool<GameObject>
  {
    private readonly GameObject _prototype;
    private readonly Stack<GameObject> _instances;

    public GameObjectPool(GameObject prototype)
    {
      _instances = new Stack<GameObject>();
      _prototype = prototype;
    }

    public GameObject Get()
    {
      GameObject instance;
      while (_instances.TryPop(out instance) && !instance)
      {
      }

      if (!instance)
      {
        Transform parent = _prototype.transform.parent ? _prototype.transform.parent : GlobalPools.DefaultPoolItemsParent;
        instance = Object.Instantiate(_prototype, parent);
        foreach (IPoolItemCreateHandler createHandler in instance.GetComponents<IPoolItemCreateHandler>())
          createHandler.OnCreate(this);
      }

      instance.SetActive(true);

      foreach (IPoolItemGetHandler getHandler in instance.GetComponents<IPoolItemGetHandler>())
        getHandler.OnGet();

      return instance;
    }

    public PooledObject<GameObject> Get(out GameObject instance) =>
      new(instance = Get(), this);

    public void Release(GameObject instance)
    {
      if (!instance) return;

      instance.SetActive(false);

      foreach (IPoolItemReleaseHandler releaseHandler in instance.GetComponents<IPoolItemReleaseHandler>())
        releaseHandler.OnRelease();

      _instances.Push(instance);
    }

    public void Clear()
    {
      while (_instances.TryPop(out GameObject instance))
      {
        if (!instance) continue;

        Object.Destroy(instance);
      }
    }

    public int CountInactive =>
      _instances.Count;
  }
}