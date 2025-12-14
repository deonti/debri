using UnityEngine;
using UnityEngine.Pool;

namespace Debri.Pools.Internals
{
  internal readonly struct ComponentPoolWrapper<TComponent> : IObjectPool<TComponent>, IPrewarmable where TComponent : Component
  {
    private readonly IObjectPool<GameObject> _pool;

    public ComponentPoolWrapper(IObjectPool<GameObject> pool) =>
      _pool = pool;

    public TComponent Get() =>
      _pool.Get().GetComponent<TComponent>();

    public PooledObject<TComponent> Get(out TComponent instance) =>
      new(instance = Get(), this);

    public void Release(TComponent instance) =>
      _pool.Release(instance.gameObject);

    public void Clear() =>
      _pool.Clear();

    public int CountInactive =>
      _pool.CountInactive;

    void IPrewarmable.Prewarm(int count) =>
      _pool.Prewarm(count);
  }
}