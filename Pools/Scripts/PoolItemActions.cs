using UnityEngine;
using UnityEngine.Pool;

namespace Debri.Pools
{
  /// <summary>
  /// Provides access to actions on pool item.
  /// </summary>
  [AddComponentMenu("Debri/Pools/Pool Item Actions")]
  [DisallowMultipleComponent]
  public class PoolItemActions : MonoBehaviour, IPoolItemCreateHandler
  {
    private IObjectPool<GameObject> _owner;

    /// <summary>
    /// Releases item back to pool.
    /// </summary>
    /// <param name="item">Item to release. </param>
    public void Release(GameObject item) =>
      _owner.Release(item);

    void IPoolItemCreateHandler.OnCreate(IObjectPool<GameObject> owner) =>
      _owner = owner;
  }
}