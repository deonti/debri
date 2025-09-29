using System;
using UnityEngine;

namespace Debri.Common
{
  public static class GameObjectUtils
  {
    /// <summary>
    /// Returns component of type TComponent from game object or adds it if it doesn't exist
    /// </summary>
    /// <typeparam name="TComponent">
    /// Type of component
    /// </typeparam>
    /// <param name="gameObject">
    /// The game object to get the component from
    /// </param>
    /// <param name="onAdded">
    /// Action to invoke when component is added, it's useful for initialization
    /// </param>
    /// <returns>
    /// Existing or added component
    /// </returns>
    public static TComponent GetOrAddComponent<TComponent>(this GameObject gameObject, Action<TComponent> onAdded = null)
      where TComponent : Component
    {
      if (!gameObject.TryGetComponent(out TComponent component))
      {
        component = gameObject.AddComponent<TComponent>();
        onAdded?.Invoke(component);
      }

      return component;
    }
  }
}