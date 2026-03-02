using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Debri.Common.Editor.Internals
{
  internal static class FinalizeOnPlayModeExitProcessor
  {
    private static Finalizer[] _finalizers;

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
      _finalizers = TypeCache.GetMethodsWithAttribute<FinalizeOnPlayModeExitMethodAttribute>()
        .Select(method => new Finalizer(method)).ToArray();
      if (_finalizers.Length == 0) return;

      EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
    }

    private static void HandlePlayModeStateChanged(PlayModeStateChange state)
    {
      if (state is not PlayModeStateChange.ExitingPlayMode) return;

      bool isDomainReloadEnabled =
        !(EditorSettings.enterPlayModeOptionsEnabled
          && EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload));

      foreach (Finalizer finalizer in _finalizers)
        finalizer.Invoke(isDomainReloadEnabled);
    }

    private class Finalizer
    {
      private readonly MethodInfo _method;
      private readonly bool _invokeIfDomainReloadEnabled;

      public Finalizer(MethodInfo method)
      {
        _method = method;
        _invokeIfDomainReloadEnabled = method.GetCustomAttribute<FinalizeOnPlayModeExitMethodAttribute>().InvokeIfDomainReloadEnabled;
      }

      public void Invoke(bool isDomainReloadEnabled)
      {
        if (isDomainReloadEnabled && !_invokeIfDomainReloadEnabled) return;

        try
        {
          _method.Invoke(null, null);
        }
        catch (Exception exception)
        {
          Debug.LogException(exception);
        }
      }
    }
  }
}