using System;
using JetBrains.Annotations;

namespace Debri.Common.Editor
{
  /// <summary>
  /// Marks a method to be invoked when the editor is about to exit play mode.
  /// </summary>
  /// <seealso cref="UnityEditor.InitializeOnLoadMethodAttribute"/>
  /// <seealso cref="UnityEngine.RuntimeInitializeOnLoadMethodAttribute"/>
  [MeansImplicitUse]
  [AttributeUsage(AttributeTargets.Method)]
  public class FinalizeOnPlayModeExitMethodAttribute : Attribute
  {
    public readonly bool InvokeIfDomainReloadEnabled;

    /// <param name="invokeIfDomainReloadEnabled">
    /// If true, the method will be invoked even if the domain reload is enabled.
    /// Otherwise, the method will be invoked only if the domain reload is disabled.
    /// </param>
    /// <seealso cref="UnityEditor.EnterPlayModeOptions.DisableDomainReload"/>
    public FinalizeOnPlayModeExitMethodAttribute(bool invokeIfDomainReloadEnabled = false) =>
      InvokeIfDomainReloadEnabled = invokeIfDomainReloadEnabled;
  }
}