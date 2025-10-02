using System;
using UnityEngine;

namespace Debri.Common
{
  partial class KeyAccessible<TBehaviour, TKey>
  {
    /// <summary>
    /// Provides behaviour resolving.
    /// </summary>
    [Serializable]
    public class Resolver
    {
      [Tooltip("Controls how the behaviour is resolved.")]
      [SerializeField] private ResolvingMode _mode;

      [Tooltip("Key to use when resolving by key.")]
      [SerializeField] private TKey _key;

      [Tooltip("Explicit behaviour reference.")]
      [SerializeField] private TBehaviour _explicitValue;

      /// <summary>
      /// Resolves behaviour instance by key.
      /// </summary>
      /// <param name="key">Key to search instance</param>
      /// <returns>Behaviour instance if found, otherwise null</returns>
      public static TBehaviour Resolve(TKey key) =>
        _behaviours.TryGetValue(key, out TBehaviour instance) ? instance : null;

      /// <summary>
      /// Resolves behaviour instance based on resolver <see cref="_mode"/>.
      /// </summary>
      /// <returns>Resolved behaviour if found, otherwise null</returns>
      public TBehaviour Resolve() => _mode switch
      {
        ResolvingMode.Explicit => _explicitValue,
        ResolvingMode.ValueByKey => Resolve(_key),
        ResolvingMode.ExplicitThenValueByKey => _explicitValue ? _explicitValue : Resolve(_key),
        _ => throw new ArgumentOutOfRangeException()
      };

      /// <summary>
      /// Defines available resolver selection strategies.
      /// </summary>
      private enum ResolvingMode
      {
        /// <summary>
        /// Tries explicit reference first, then falls back to key lookup
        /// </summary>
        ExplicitThenValueByKey,

        /// <summary>
        /// Uses only the explicitly specified component reference
        /// </summary>
        Explicit,

        /// <summary>
        /// Uses only key-based lookup from the registry
        /// </summary>
        ValueByKey
      }
    }
  }
}