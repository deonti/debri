using System;
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
    internal static Transform DefaultPoolItemsParent
    {
      get => _defaultPoolItemsParent;
      set
      {
        if (_defaultPoolItemsParent == value) return;

        _defaultPoolItemsParent = value;
        OnDefaultPoolItemsParentChanged?.Invoke();
      }
    }

    internal static event Action OnDefaultPoolItemsParentChanged;

    private static readonly Dictionary<Object, IObjectPool<GameObject>> _poolsMap = new();
    private static Transform _defaultPoolItemsParent;

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
  }
}