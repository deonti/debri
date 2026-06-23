using Debri.Pools.Internals;
using UnityEngine;

namespace Debri.Pools
{
  /// <summary>
  ///   Provides access to scene-level settings for <see cref="GlobalPools"/>.
  /// </summary>
  [AddComponentMenu("Debri/Pools/Global Pools Settings")]
  [DisallowMultipleComponent]
  [DefaultExecutionOrder(-1000)]
  public class GlobalPoolsSettings : MonoBehaviour
  {
    [SerializeField] private Transform _itemsContainer;

    private void Awake() =>
      gameObject.scene.SetSettings(this);

    internal bool TryGetContainer(out Transform container) =>
      container = _itemsContainer;
  }
}