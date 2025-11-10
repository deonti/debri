using UnityEngine;

namespace Debri.Pools
{
  /// <summary>
  /// Provides access to actions on pool item.
  /// </summary>
  [AddComponentMenu("Debri/Pools/Pool Item Actions")]
  [DisallowMultipleComponent]
  public class PoolItemActions : MonoBehaviour
  {
    /// <summary>
    /// Releases item back to pool or destroys if it's not from pool.
    /// </summary>
    public void ReleaseOrDestroy() =>
      gameObject.ReleaseOrDestroy();
  }
}