using System;
using Debri.PlayerLoop.Internal;
using PlayerLoopSystem = UnityEngine.LowLevel.PlayerLoopSystem;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Debri.Common
{
  [Obsolete("Use PlayerLoopManager.")]
  public static class PlayerLoopSystemUtils
  {
    /// <summary> Adds a subsystem to a parent system</summary>
    /// <param name="rootSystem">Root PlayerLoopSystem</param>
    /// <param name="subsystem">Subsystem to add</param>
    /// <typeparam name="TParentSystem">Type of the parent system</typeparam>
    /// <example>
    /// <code>
    ///    var rootSystem = PlayerLoop.GetCurrentPlayerLoop();
    ///    var mySystem = new PlayerLoopSystem { type = typeof(MySystem), updateDelegate = MyUpdate };
    ///    rootSystem.AddSubsystemTo&lt;UnityEngine.PlayerLoop.PreUpdate&gt;(mySystem);
    ///    PlayerLoop.SetPlayerLoop(rootSystem);
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("Use PlayerLoopManager.AddSubsystem.")]
    public static void AddSubSystemTo<TParentSystem>(this ref PlayerLoopSystem rootSystem, PlayerLoopSystem subsystem) =>
      rootSystem.AddSubsystemTo<TParentSystem>(subsystem);

    /// <summary>
    /// Removes a subsystem from a parent system if it exists
    /// </summary>
    /// <param name="rootSystem">Root PlayerLoopSystem</param>
    /// <param name="subsystemType">Subsystem to remove</param>
    /// <typeparam name="TParentSystem">Type of the parent system</typeparam>
    /// <returns>True if the subsystem was removed</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("See PlayerLoopManager.AddSubsystem.")]
    public static bool TryRemoveSubSystemFrom<TParentSystem>(this ref PlayerLoopSystem rootSystem, Type subsystemType) =>
      rootSystem.TryRemoveSubsystemFrom<TParentSystem>(subsystemType);
  }
}