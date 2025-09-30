using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Debri.Pools.Triggers
{
  /// <summary>
  /// Event trigger component for handling pool item creation.
  /// </summary>
  [AddComponentMenu("Debri/Pools/Triggers/Pool Item Create Event Trigger")]
  [DisallowMultipleComponent]
  public class PoolItemCreateEventTrigger : MonoBehaviour, IPoolItemCreateHandler
  {
    [SerializeField] private UnityEvent _onCreate;

    void IPoolItemCreateHandler.OnCreate(IObjectPool<GameObject> _) =>
      _onCreate?.Invoke();
  }
}