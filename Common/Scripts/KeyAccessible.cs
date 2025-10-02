using System.Collections.Generic;
using UnityEngine;

namespace Debri.Common
{
  /// <summary>
  /// Base class for components that can be accessed by a unique key. 
  /// Ensures single registration per key and provides resolver functionality.
  /// </summary>
  /// <typeparam name="TBehaviour">Type of component implementing key accessibility</typeparam>
  /// <typeparam name="TKey">Type of key used for registration</typeparam>
  [DefaultExecutionOrder(-10)]
  public abstract partial class KeyAccessible<TBehaviour, TKey> : MonoBehaviour
    where TBehaviour : KeyAccessible<TBehaviour, TKey>
  {
    [Tooltip("Controls whether this component should be registered by its key.")]
    [SerializeField] private bool _availableByKey = true;

    [Tooltip("Unique key used to register this component.")]
    [SerializeField] private TKey _key;

    private static readonly Dictionary<TKey, TBehaviour> _behaviours = new();

#if UNITY_EDITOR
    static KeyAccessible()
    {
      UnityEditor.EditorApplication.playModeStateChanged += state =>
      {
        if (state is UnityEditor.PlayModeStateChange.ExitingPlayMode)
          _behaviours.Clear();
      };
    }
#endif

    protected virtual void Awake()
    {
      if (_availableByKey)
        TryAssociateWithKey();
    }

    private void TryAssociateWithKey()
    {
      if (_behaviours.TryGetValue(_key, out TBehaviour behaviour))
      {
        Debug.LogError($"{_key} is already associated with {behaviour.name}", this);
        return;
      }

      _behaviours[_key] = (TBehaviour)this;
      destroyCancellationToken.Register(() => _behaviours.Remove(_key));
    }
  }
}