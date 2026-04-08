using System;
using System.Collections.Generic;
using Debri.Common;
using UnityEngine;
using UnityEngine.Pool;

namespace Debri.PlayerLoop.Internal
{
  internal class HandlersRegistration : IDisposable
  {
    private static readonly Dictionary<Type, List<PlayerLoopSystem>> _systemsMap = new();
    private readonly Disposables _subscriptions = new();
    private readonly IObjectPool<HandlersRegistration> _pool;

    private HandlersRegistration(IObjectPool<HandlersRegistration> pool = null) =>
      _pool = pool;

    private void Initialize(object handlersOwner)
    {
      List<PlayerLoopSystem> systems = GetSystems(handlersOwner);
      if (systems.Count == 0)
      {
        Debug.LogWarning("Can't register handlers, no compatible systems found.", handlersOwner as UnityEngine.Object);
        return;
      }

      foreach (PlayerLoopSystem system in systems)
        _subscriptions.Add(system.Subscribe(handlersOwner));
    }

    public void Dispose()
    {
      _subscriptions.DisposeAll();
      _pool?.Release(this);
    }

    private static List<PlayerLoopSystem> GetSystems(object subscriber)
    {
      Type subscriberType = subscriber.GetType();
      if (_systemsMap.TryGetValue(subscriberType, out List<PlayerLoopSystem> systems))
        return systems;

      _systemsMap[subscriberType] = systems = new List<PlayerLoopSystem>();
      foreach (PlayerLoopSystem system in PlayerLoopManager.Systems)
        if (system.HasCompatibleHandler(subscriber))
          systems.Add(system);

      return systems;
    }

    public static class Pool
    {
      private static readonly ObjectPool<HandlersRegistration> _pool = new(
        createFunc: static () => new HandlersRegistration(_pool));

      public static IDisposable Get(object handlersOwner)
      {
        HandlersRegistration registration = _pool.Get();
        registration.Initialize(handlersOwner);
        return registration;
      }
    }
  }
}