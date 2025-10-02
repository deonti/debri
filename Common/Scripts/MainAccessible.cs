using UnityEngine.Scripting;

namespace Debri.Common
{
  /// <summary>
  /// Specialized <see cref="KeyAccessible{TBehaviour,TKey}"/> implementation for main singleton-like components.
  /// Uses an internal Keys enum with a single "Main" entry as the default key.
  /// </summary>
  /// <typeparam name="TBehaviour">Type of behaviour implementing main accessibility</typeparam>
  public abstract class MainAccessible<TBehaviour> : KeyAccessible<TBehaviour, MainAccessible<TBehaviour>.Keys>
    where TBehaviour : MainAccessible<TBehaviour>
  {
    /// <summary>
    /// Enum defining available keys for main accessible components.
    /// </summary>
    public enum Keys
    {
      /// <summary>
      /// Default main key for singleton-like components.
      /// </summary>
      [Preserve]
      Main
    }
  }
}