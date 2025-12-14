using System;
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
    [SerializeField] private Transform _defaultPoolItemsParent;
    [SerializeField] private bool _suppressWarnings;

    private static GlobalPoolsSettings _instance;

    private void Awake()
    {
      if (_instance)
        throw new InvalidOperationException("Only one GlobalPoolsController can exist at a time.");

      _instance = this;

      GlobalPools.DefaultPoolItemsParent = _defaultPoolItemsParent;
      GlobalPools.SuppressWarnings = _suppressWarnings;
    }

    private void OnDestroy()
    {
      _instance = null;
      GlobalPools.DefaultPoolItemsParent = null;
      GlobalPools.SuppressWarnings = false;
    }
  }
}