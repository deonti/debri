using UnityEngine;
using UnityEngine.Pool;

namespace Debri.Pools
{
  /// <summary>
  /// Interface for handling pool item creation.
  /// </summary>
  /// <remarks>
  /// Implement this interface in your own component to handle pool item creation.
  /// </remarks>
  public interface IPoolItemCreateHandler
  {
    /// <summary>
    /// Called when an item is created by one of the pools from <see cref="GlobalPools"/>.
    /// </summary>
    /// <param name="owner">The pool that created the item.</param>
    void OnCreate(IObjectPool<GameObject> owner)
    {
    }
  }
}