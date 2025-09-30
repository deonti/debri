using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Debri.Pools
{
  /// <summary>
  /// Implements methods for working with global pools.
  /// </summary>
  /// <remarks>
  /// This class and its dependent logic are experimental and can be changed at any time.
  /// </remarks>
  public static class GlobalPools
  {
    private static readonly Dictionary<Object, IObjectPool<GameObject>> _poolsMap = new();

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void ResetOnEnterPlayMode() =>
      _poolsMap.Clear();
#endif

    /// <summary>
    /// Gets or creates a pool of game objects.
    /// </summary>
    /// <param name="prototype">Prototype of pool items.</param>
    /// <returns>Pool associated with the specified prototype.</returns>
    /// <remarks>
    /// Every prototype is associated with a single pool.
    /// </remarks>
    public static IObjectPool<GameObject> Get(GameObject prototype) =>
      _poolsMap.TryGetValue(prototype, out IObjectPool<GameObject> pool)
        ? pool
        : _poolsMap[prototype] = NewGameObjectPool(prototype);

    /// <summary>
    /// Gets or creates a pool of components.
    /// </summary>
    /// <typeparam name="TComponent">Type of pool items.</typeparam>
    /// <param name="prototype">Prototype of pool items.</param>
    /// <returns>Pool associated with the specified prototype.</returns>
    /// <remarks>
    /// Technically, it returns the same pool as calling <see cref="Get(GameObject)"/> with the prototype's game object.
    /// </remarks>
    public static IObjectPool<TComponent> Get<TComponent>(TComponent prototype)
      where TComponent : Component =>
      new ComponentPoolWrapper<TComponent>(Get(prototype.gameObject));

    /// <summary>
    /// Gets or creates a pool of game objects or a default value if the specified prototype is null.
    /// </summary>
    /// <param name="prototype">Prototype of pool items.</param>
    /// <param name="defaultValue">Default pool to return if the specified prototype is null.</param>
    /// <returns>Pool associated with the specified prototype or the default pool.</returns>
    public static IObjectPool<GameObject> GetOrDefault(GameObject prototype, IObjectPool<GameObject> defaultValue = null) =>
      prototype ? Get(prototype) : defaultValue;

    /// <summary>
    /// Gets or creates a pool of components or a default value if the specified prototype is null.
    /// </summary>
    /// <typeparam name="TComponent">Type of pool items.</typeparam>
    /// <param name="prototype">Prototype of pool items.</param>
    /// <param name="defaultValue">Default pool to return if the specified prototype is null.</param>
    /// <returns>Pool associated with the specified prototype or the default pool.</returns>
    public static IObjectPool<TComponent> GetOrDefault<TComponent>(TComponent prototype, IObjectPool<TComponent> defaultValue = null)
      where TComponent : Component =>
      prototype ? Get(prototype) : defaultValue;

    private static IObjectPool<GameObject> NewGameObjectPool(GameObject prototype) =>
      new GameObjectPool(prototype);

    private readonly struct GameObjectPool : IObjectPool<GameObject>
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
          instance = Object.Instantiate(_prototype, _prototype.transform.parent);
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

    private readonly struct ComponentPoolWrapper<TComponent> : IObjectPool<TComponent> where TComponent : Component
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
    }
  }
}