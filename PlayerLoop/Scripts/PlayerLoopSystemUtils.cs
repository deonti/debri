using System;
using UnityEngine.LowLevel;

namespace Debri.PlayerLoop
{
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
    public static void AddSubsystemTo<TParentSystem>(this ref PlayerLoopSystem rootSystem, PlayerLoopSystem subsystem)
    {
      Type parentSystemType = typeof(TParentSystem);
      if (!TryAddSubsystemTo(ref rootSystem, parentSystemType, subsystem))
        throw new Exception($"System of type {parentSystemType.Name} not found");
    }

    /// <summary>
    /// Removes a subsystem from a parent system if it exists
    /// </summary>
    /// <param name="rootSystem">Root PlayerLoopSystem</param>
    /// <param name="subsystemType">Subsystem to remove</param>
    /// <typeparam name="TParentSystem">Type of the parent system</typeparam>
    /// <returns>True if the subsystem was removed</returns>
    public static bool TryRemoveSubsystemFrom<TParentSystem>(this ref PlayerLoopSystem rootSystem, Type subsystemType) =>
      TryRemoveSubsystemFrom(ref rootSystem, typeof(TParentSystem), subsystemType);

    private static bool TryAddSubsystemTo(ref PlayerLoopSystem currentSystem, Type parentSystemType, PlayerLoopSystem subsystem)
    {
      if (currentSystem.type == parentSystemType)
      {
        if (currentSystem.subSystemList is null)
          currentSystem.subSystemList = new PlayerLoopSystem[1];
        else
          Array.Resize(ref currentSystem.subSystemList, currentSystem.subSystemList.Length + 1);

        currentSystem.subSystemList[^1] = subsystem;
        return true;
      }

      if (currentSystem.subSystemList is not null)
        for (int index = 0; index < currentSystem.subSystemList.Length; index++)
          if (TryAddSubsystemTo(ref currentSystem.subSystemList[index], parentSystemType, subsystem))
            return true;

      return false;
    }

    private static bool TryRemoveSubsystemFrom(ref PlayerLoopSystem currentSystem, Type parentSystemType, Type subsystemType)
    {
      if (currentSystem.subSystemList is null)
        return false;

      if (currentSystem.type == parentSystemType)
      {
        int subsystemIndex = Array.FindIndex(currentSystem.subSystemList, system => system.type == subsystemType);
        if (subsystemIndex > -1)
        {
          for (int index = subsystemIndex; index < currentSystem.subSystemList.Length - 1; index++)
            currentSystem.subSystemList[index] = currentSystem.subSystemList[index + 1];

          Array.Resize(ref currentSystem.subSystemList, currentSystem.subSystemList.Length - 1);
          return true;
        }
      }

      for (int index = 0; index < currentSystem.subSystemList.Length; index++)
        if (TryRemoveSubsystemFrom(ref currentSystem.subSystemList[index], parentSystemType, subsystemType))
          return true;

      return false;
    }
  }
}

namespace Debri.Common
{
  using System.Runtime.CompilerServices;
  using PlayerLoop;

  [Obsolete("Use Debri.PlayerLoop.PlayerLoopSystemUtils instead.")]
  public static class PlayerLoopSystemUtils
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("Use Debri.PlayerLoop.PlayerLoopSystemUtils.AddSubSystemTo instead.")]
    public static void AddSubSystemTo<TParentSystem>(this ref PlayerLoopSystem rootSystem, PlayerLoopSystem subsystem) =>
      rootSystem.AddSubsystemTo<TParentSystem>(subsystem);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("Use Debri.PlayerLoop.PlayerLoopSystemUtils.TryRemoveSubSystemFrom instead.")]
    public static bool TryRemoveSubSystemFrom<TParentSystem>(this ref PlayerLoopSystem rootSystem, Type subsystemType) =>
      rootSystem.TryRemoveSubsystemFrom<TParentSystem>(subsystemType);
  }
}