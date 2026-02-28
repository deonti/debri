using System;
using UnityEngine.LowLevel;

namespace Debri.Common
{
  public static class PlayerLoopSystemUtils
  {
    /// <summary> Adds a subsystem to a parent system</summary>
    /// <param name="rootSystem">Root PlayerLoopSystem</param>
    /// <param name="subSystem">Subsystem to add</param>
    /// <typeparam name="TParentSystem">Type of the parent system</typeparam>
    /// <example>
    /// <code>
    ///    var rootSystem = PlayerLoop.GetCurrentPlayerLoop();
    ///    var mySystem = new PlayerLoopSystem { type = typeof(MySystem), updateDelegate = MyUpdate };
    ///    rootSystem.AddSubSystemTo&lt;UnityEngine.PlayerLoop.PreUpdate&gt;(mySystem);
    ///    PlayerLoop.SetPlayerLoop(rootSystem);
    /// </code>
    /// </example>
    public static void AddSubSystemTo<TParentSystem>(this ref PlayerLoopSystem rootSystem, PlayerLoopSystem subSystem)
    {
      Type parentSystemType = typeof(TParentSystem);
      if (!TryAddSubSystemTo(ref rootSystem, parentSystemType, subSystem))
        throw new Exception($"System of type {parentSystemType.Name} not found");
    }

    private static bool TryAddSubSystemTo(ref PlayerLoopSystem currentSystem, Type parentSystemType, PlayerLoopSystem subSystem)
    {
      if (currentSystem.type == parentSystemType)
      {
        if (currentSystem.subSystemList is null)
          currentSystem.subSystemList = new PlayerLoopSystem[1];
        else
          Array.Resize(ref currentSystem.subSystemList, currentSystem.subSystemList.Length + 1);

        currentSystem.subSystemList[^1] = subSystem;
        return true;
      }

      if (currentSystem.subSystemList is not null)
        for (int index = 0; index < currentSystem.subSystemList.Length; index++)
          if (TryAddSubSystemTo(ref currentSystem.subSystemList[index], parentSystemType, subSystem))
            return true;

      return false;
    }
  }
}