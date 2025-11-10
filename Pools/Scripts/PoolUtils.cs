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
  }
}