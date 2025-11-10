using System;
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
      if (instance.TryGetOwner(out IObjectPool<GameObject> pool))
        pool.Release(instance);
      else
        Object.Destroy(instance);
    }

    /// <summary>
    /// Adds a handler that will be called once when the next release of the instance occurs.
    /// </summary>
    /// <param name="instance">The instance to add the handler to.</param>
    /// <param name="onRelease">The handler to call when the next release occurs.</param>
    /// <exception cref="ArgumentException">Thrown if the instance is not pooled.</exception>
    public static void ScheduleReleaseHandler(this GameObject instance, Action onRelease)
    {
      if (!instance.TryGetComponent(out GameObjectPoolItem agent))
        throw new ArgumentException($"Instance {instance.name} is not pooled. Release handler will not be added.");

      agent.ScheduleReleaseHandler(onRelease);
    }

    private static bool TryGetOwner(this GameObject instance, out IObjectPool<GameObject> pool) =>
      (pool = instance.TryGetComponent(out GameObjectPoolItem agent) ? agent.Owner : null) is not null;
  }
}