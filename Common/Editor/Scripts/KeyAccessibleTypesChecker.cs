using System;
using System.Linq;
using UnityEngine;

namespace Debri.Common.Editor
{
  public static class KeyAccessibleTypesChecker
  {
    [UnityEditor.InitializeOnLoadMethod]
    private static void CheckTypes()
    {
      Type baseTypeDefinition = typeof(MainAccessible<>).BaseType!.GetGenericTypeDefinition();
      foreach (Type behaviourType in UnityEditor.TypeCache.GetTypesDerivedFrom(baseTypeDefinition)
                 .Where(type => type is
                 {
                   IsAbstract: false,
                   IsGenericType: false
                 }))
      {
        Type ancestorType = behaviourType;
        while (ancestorType is { IsGenericType: false } || ancestorType!.GetGenericTypeDefinition() != baseTypeDefinition)
          ancestorType = ancestorType.BaseType;

        Type ancestorBehaviourType = ancestorType.GenericTypeArguments[0];
        if (behaviourType != ancestorBehaviourType)
          Debug.LogError(
            $"Incompatible types: {behaviourType.Name} and {ancestorBehaviourType.Name}. " +
            $"Check declaration of {behaviourType.FullName}, it should inherit from KeyAccessible<{behaviourType.Name}, TKey>.");
      }
    }
  }
}