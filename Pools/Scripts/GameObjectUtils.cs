using System;
using Debri.Common;
using Debri.Pools.Internals;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Debri.Pools
{
  public static class GameObjectUtils
  {
    /// <summary>
    /// Releases the instance to the pool or destroys it if it was not created from a pool.
    /// </summary>
    /// <param name="instance">The instance to release or destroy.</param>
    public static void ReleaseOrDestroy(this GameObject instance)
    {
      if (instance.TryGetSourcePool(out IObjectPool<GameObject> pool))
        pool.Release(instance);
      else
        Object.Destroy(instance);
    }

    /// <summary>
    /// Adds a handler that will be called once when the next release of the instance occurs.
    /// </summary>
    /// <param name="instance">The instance to add the handler to.</param>
    /// <param name="handler">The handler to call when the next release occurs.</param>
    /// <exception cref="ArgumentException">Thrown if the instance is not pooled.</exception>
    public static void ScheduleReleaseHandler(this GameObject instance, Action handler)
    {
      if (!instance.TryGetComponent(out GameObjectPoolItem poolItem))
        throw new ArgumentException($"Instance {instance.name} is not pooled. Release handler will not be added.");

      poolItem.ScheduleReleaseHandler(handler, callByDestroy: false);
    }

    /// <summary>
    /// Adds a handler that will be called once when the instance is next released to the pool or destroyed.
    /// </summary>
    /// <param name="instance">The instance to add the handler to.</param>
    /// <param name="handler">The handler to call when the next release or destruction occurs.</param>
    public static void ScheduleReleaseOrDestroyHandler(this GameObject instance, Action handler)
    {
      if (!instance.TryGetComponent(out GameObjectPoolItem poolItem))
      {
        instance.SubscribeToOnDestroy(handler);
        return;
      }

      poolItem.ScheduleReleaseHandler(handler, callByDestroy: true);
    }

    /// <summary>
    /// Gets the pool from which the instance was created, if it is pooled.
    /// </summary>
    /// <param name="instance">The instance to get the source pool for.</param>
    /// <param name="pool">When this method returns, contains the pool from which the instance was created, if found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the instance was created from a pool; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetSourcePool(this GameObject instance, out IObjectPool<GameObject> pool) =>
      (pool = instance.TryGetComponent(out GameObjectPoolItem poolItem) ? poolItem.Owner : null) is not null;
  }
}