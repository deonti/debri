using UnityEngine;
using UnityEngine.Events;

namespace Debri.Pools.Triggers
{
  /// <summary>
  /// Event trigger component for handling pool item get.
  /// </summary>
  [AddComponentMenu("Debri/Pools/Triggers/Pool Item Get Event Trigger")]
  [DisallowMultipleComponent]
  public class PoolItemGetEventTrigger : MonoBehaviour, IPoolItemGetHandler
  {
    [SerializeField] private UnityEvent _onGet;

    void IPoolItemGetHandler.OnGet() =>
      _onGet?.Invoke();
  }
}