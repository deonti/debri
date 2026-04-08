using System;
using UnityPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoopSystem;

namespace Debri.PlayerLoop.Internal
{
  internal static class UnityPlayerLoopSystemUtils
  {
    public static void AddSubsystemTo<TParentSystem>(this ref UnityPlayerLoopSystem rootSystem, UnityPlayerLoopSystem subsystem)
    {
      Type parentSystemType = typeof(TParentSystem);
      if (!TryAddSubsystemTo(ref rootSystem, parentSystemType, subsystem))
        throw new Exception($"System of type {parentSystemType.Name} not found");
    }

    public static bool TryRemoveSubsystemFrom<TParentSystem>(this ref UnityPlayerLoopSystem rootSystem, Type subsystemType) =>
      TryRemoveSubsystemFrom(ref rootSystem, typeof(TParentSystem), subsystemType);

    private static bool TryAddSubsystemTo(ref UnityPlayerLoopSystem currentSystem, Type parentSystemType, UnityPlayerLoopSystem subsystem)
    {
      if (currentSystem.type == parentSystemType)
      {
        if (currentSystem.subSystemList is null)
          currentSystem.subSystemList = new UnityPlayerLoopSystem[1];
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

    private static bool TryRemoveSubsystemFrom(ref UnityPlayerLoopSystem currentSystem, Type parentSystemType, Type subsystemType)
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