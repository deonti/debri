using System;
using UnityEngine;

namespace Debri.Pools
{
  /// <summary>
  ///   Provides access to scene-level settings for <see cref="GlobalPools"/>.
  /// </summary>
  [AddComponentMenu("Debri/Pools/Global Pools Settings")]
  [DisallowMultipleComponent]
  public class GlobalPoolsSettings : MonoBehaviour
  {
    [SerializeField] private Transform _defaultPoolItemsParent;

    private static GlobalPoolsSettings _instance;

    private void Awake()
    {
      if (_instance)
        throw new InvalidOperationException("Only one GlobalPoolsController can exist at a time.");

      _instance = this;

      GlobalPools.DefaultPoolItemsParent = _defaultPoolItemsParent;
    }

    private void OnDestroy() =>
      _instance = null;
  }
}