using UnityEngine;
using UnityEngine.Events;

namespace Debri.Pools.Triggers
{
  /// <summary>
  /// Event trigger component for handling pool item release.
  /// </summary>
  [AddComponentMenu("Debri/Pools/Triggers/Pool Item Release Event Trigger")]
  [DisallowMultipleComponent]
  public class PoolItemReleaseEventTrigger : MonoBehaviour, IPoolItemReleaseHandler
  {
    [SerializeField] private UnityEvent _onRelease;

    void IPoolItemReleaseHandler.OnRelease() =>
      _onRelease?.Invoke();
  }
}