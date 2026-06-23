using System;
using System.Collections;
using Debri.Common.Internal;
using UnityEngine;

namespace Debri.Common
{
  public static class GameObjectUtils
  {
    /// <summary>
    /// Starts a coroutine on a game object
    /// </summary>
    /// <param name="gameObject">
    /// The game object to start the coroutine on
    /// </param>
    /// <param name="routine">
    /// The coroutine to start
    /// </param>
    /// <returns>
    /// Reference to started coroutine
    /// </returns>
    public static Coroutine StartCoroutine(this GameObject gameObject, IEnumerator routine) =>
      CoroutineRunner.StartCoroutine(gameObject, routine);

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

    /// <summary>
    /// Subscribes a handler to be called when the game object is destroyed.
    /// </summary>
    /// <param name="gameObject">
    /// The game object to observe.
    /// </param>
    /// <param name="handler">
    /// The action to invoke on destruction.
    /// </param>
    /// <returns>
    /// <see cref="IDisposable"/> token to cancel the subscription.
    /// </returns>
    public static IDisposable SubscribeToOnDestroy(this GameObject gameObject, Action handler) =>
      GetOrAddComponent<Agent>(gameObject).SubscribeToOnDestroy(handler);

    [DefaultExecutionOrder(-1001)]
    private class Agent : MonoBehaviour
    {
      private Action _destroyHandlers;

      private void OnDestroy() =>
        _destroyHandlers?.Invoke();

      public IDisposable SubscribeToOnDestroy(Action handler) =>
        new Subscription(this, handler);

      private class Subscription : IDisposable
      {
        private readonly Agent _owner;
        private readonly Action _handler;

        public Subscription(Agent owner, Action handler)
        {
          _owner = owner;
          _handler = handler;
          _owner._destroyHandlers += _handler;
        }

        public void Dispose()
        {
          if (!_owner) return;

          _owner._destroyHandlers -= _handler;
        }
      }
    }
  }
}