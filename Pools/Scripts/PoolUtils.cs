using System;
using System.Collections;
using Debri.Common;
using UnityEngine;
using UnityEngine.Pool;

namespace Debri.Pools
{
  /// <remarks>
  /// Many methods of this class are designed to work with pools from <see cref="GlobalPools"/>.
  /// </remarks>
  public static class PoolUtils
  {
    /// <summary>
    /// Gets an item from the pool and adds a release handler to it.
    /// </summary>
    /// <typeparam name="TItem">The type of item to get.</typeparam>
    /// <param name="pool">The pool to get an item from.</param>
    /// <param name="onRelease">The release handler to add to the item.</param>
    /// <returns>The item from the pool.</returns>
    public static TItem Get<TItem>(this IObjectPool<TItem> pool, Action onRelease)
      where TItem : Component
    {
      TItem item = pool.Get();
      item.gameObject.GetOrAddComponent<Agent>().SetReleaseHandler(onRelease);
      return item;
    }

    /// <summary>
    /// Releases multiple items at once.
    /// </summary>
    /// <typeparam name="TItem">The type of items to release.</typeparam>
    /// <param name="pool">The pool to release items to.</param>
    /// <param name="items">The items to release.</param>
    public static void Release<TItem>(this IObjectPool<TItem> pool, params TItem[] items)
      where TItem : class
    {
      foreach (TItem item in items)
        pool.Release(item);
    }

    /// <summary>
    /// Releases an item after a delay.
    /// </summary>
    /// <param name="pool">The pool to release the item to.</param>
    /// <param name="item">The item to release.</param>
    /// <param name="delay">The delay before releasing the item.</param>
    /// <remarks>
    /// This method uses coroutine that runs on the game object. It's works only if the item is active.
    /// </remarks>
    public static void ReleaseDelayed(this IObjectPool<GameObject> pool, GameObject item, float delay) =>
      item.StartCoroutine(WaitThenRelease(pool, item, delay));

    private static IEnumerator WaitThenRelease(IObjectPool<GameObject> pool, GameObject instance, float delay)
    {
      yield return new WaitForSeconds(delay);
      pool.Release(instance);
    }

    private class Agent : MonoBehaviour, IPoolItemReleaseHandler
    {
      private Action _releaseHandler;

      public void SetReleaseHandler(Action value) =>
        _releaseHandler = value;

      void IPoolItemReleaseHandler.OnRelease()
      {
        if (_releaseHandler is null) return;

        _releaseHandler.Invoke();
        _releaseHandler = null;
      }
    }
  }
}