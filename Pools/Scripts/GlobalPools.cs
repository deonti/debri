using System.Collections.Generic;
using Debri.Pools.Internals;
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
#if DEBRI_POOLS_SUPPRESS_WARNINGS
    internal const bool SuppressWarnings = true;
#else
    internal const bool SuppressWarnings = false;
#endif

    internal static Transform DefaultPoolItemsParent;
    private static readonly Dictionary<(Object, Object), IObjectPool<GameObject>> _poolsMap = new();

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void ResetOnEnterPlayMode() =>
      _poolsMap.Clear();
#endif

    private static Transform _persistentContainer;

    /// <summary>
    /// Gets or creates a pool of game objects.
    /// </summary>
    /// <param name="prototype">Prototype of pool items.</param>
    /// <returns>Pool associated with the specified prototype.</returns>
    /// <remarks>
    /// Every prototype is associated with a single pool.
    /// </remarks>
    public static IObjectPool<GameObject> Get(GameObject prototype) =>
      InternalGet(prototype, prototype.transform.parent ? prototype.transform.parent : DefaultPoolItemsParent);

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
    /// Gets or creates a pool of game objects with persistent container.
    /// </summary>
    /// <param name="prototype">Prototype of pool items.</param>
    /// <returns>Pool associated with the specified prototype.</returns>
    /// <remarks>
    /// Every prototype is associated with a single pool.
    /// </remarks>
    public static IObjectPool<GameObject> GetPersistent(GameObject prototype) =>
      InternalGet(prototype, GetPersistentContainer());

    /// <summary>
    /// Gets or creates a pool of components with persistent container.
    /// </summary>
    /// <typeparam name="TComponent">Type of pool items.</typeparam>
    /// <param name="prototype">Prototype of pool items.</param>
    /// <returns>Pool associated with the specified prototype.</returns>
    /// <remarks>
    /// Technically, it returns the same pool as calling <see cref="GetPersistent(GameObject)"/> with the prototype's game object.
    /// </remarks>
    public static IObjectPool<TComponent> GetPersistent<TComponent>(TComponent prototype)
      where TComponent : Component =>
      new ComponentPoolWrapper<TComponent>(GetPersistent(prototype.gameObject));

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

    /// <summary>
    /// Gets or creates a pool of game objects with persistent container or a default value if the specified prototype is null.
    /// </summary>
    /// <param name="prototype">Prototype of pool items.</param>
    /// <param name="defaultValue">Default pool to return if the specified prototype is null.</param>
    /// <returns>Pool associated with the specified prototype or the default pool.</returns>
    public static IObjectPool<GameObject> GetPersistentOrDefault(GameObject prototype, IObjectPool<GameObject> defaultValue = null) =>
      prototype ? GetPersistent(prototype) : defaultValue;

    /// <summary>
    /// Gets or creates a pool of components with persistent container or a default value if the specified prototype is null.
    /// </summary>
    /// <typeparam name="TComponent">Type of pool items.</typeparam>
    /// <param name="prototype">Prototype of pool items.</param>
    /// <param name="defaultValue">Default pool to return if the specified prototype is null.</param>
    /// <returns>Pool associated with the specified prototype or the default pool.</returns>
    public static IObjectPool<TComponent> GetPersistentOrDefault<TComponent>(TComponent prototype, IObjectPool<TComponent> defaultValue = null)
      where TComponent : Component =>
      prototype ? new ComponentPoolWrapper<TComponent>(GetPersistent(prototype.gameObject)) : defaultValue;

    private static IObjectPool<GameObject> InternalGet(GameObject prototype, Transform container) =>
      _poolsMap.TryGetValue((prototype, container), out IObjectPool<GameObject> pool)
        ? pool
        : _poolsMap[(prototype, container)] = NewGameObjectPool(prototype, container);

    private static IObjectPool<GameObject> NewGameObjectPool(GameObject prototype, Transform container) =>
      new GameObjectPool(prototype, container);

    private static Transform GetPersistentContainer()
    {
      if (_persistentContainer)
        return _persistentContainer;

      var gameObject = new GameObject("Persistent Pool Items");
      _persistentContainer = gameObject.transform;
      Object.DontDestroyOnLoad(gameObject);

      return _persistentContainer;
    }
  }
}