using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPlayerLoopSystem = UnityEngine.LowLevel.PlayerLoopSystem;

namespace Debri.PlayerLoop.Internal
{
  internal abstract class PlayerLoopSystem : IDisposable
  {
    public abstract bool HasCompatibleHandler(object subscriber);
    public abstract IDisposable Subscribe(object subscriber);
    public abstract void AddTo(ref UnityPlayerLoopSystem playerLoop);
    public abstract void RemoveFrom(ref UnityPlayerLoopSystem playerLoop);
    public abstract void Dispose();
  }

  internal abstract class PlayerLoopSystem<THandler> : PlayerLoopSystem
  {
    public override bool HasCompatibleHandler(object subscriber) =>
      subscriber is THandler;

    public override IDisposable Subscribe(object subscriber) =>
      Subscribe((THandler)subscriber);

    protected abstract IDisposable Subscribe(THandler handler);
  }

  internal class PlayerLoopSystem<THandler, TParentSystem, TSystem> : PlayerLoopSystem<THandler>
  {
    private delegate void HandlerListAction(List<THandler> handlers);

    private readonly Invoker<THandler> _invoker;
    private readonly List<THandler> _handlers = new();
    private readonly Queue<HandlerListAction> _handlersActions = new();

    public PlayerLoopSystem(Invoker<THandler> invoker) =>
      _invoker = invoker;

    public override void Dispose()
    {
      _handlers?.Clear();
      _handlersActions?.Clear();
    }

    protected override IDisposable Subscribe(THandler handler) =>
      new Subscription(this, handler);

    public override void AddTo(ref UnityPlayerLoopSystem playerLoop) =>
      playerLoop.AddSubsystemTo<TParentSystem>(new UnityPlayerLoopSystem { type = typeof(TSystem), updateDelegate = Update });

    public override void RemoveFrom(ref UnityPlayerLoopSystem playerLoop)
    {
      if (!playerLoop.TryRemoveSubsystemFrom<TParentSystem>(typeof(TSystem)))
        Debug.LogWarning($"Failed to remove sub-system \"{typeof(TSystem)}\" from \"{typeof(TParentSystem)}\"");
    }

    private void Update()
    {
      while (_handlersActions.TryDequeue(out HandlerListAction action))
        action(_handlers);

      foreach (THandler handler in _handlers)
        SafeInvoke(handler);
    }

    private void SafeInvoke(THandler handler)
    {
      try
      {
        _invoker(handler);
      }
      catch (Exception e)
      {
        Debug.LogError(e);
      }
    }

    private class Subscription : IDisposable
    {
      private readonly PlayerLoopSystem<THandler, TParentSystem, TSystem> _system;
      private readonly THandler _handler;

      public Subscription(PlayerLoopSystem<THandler, TParentSystem, TSystem> system, THandler handler)
      {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _system = system;
        _system._handlersActions.Enqueue(AddTo);
      }

      public void Dispose() =>
        _system._handlersActions.Enqueue(RemoveFrom);

      private void AddTo(List<THandler> handlers) =>
        handlers.Add(_handler);

      private void RemoveFrom(List<THandler> handlers) =>
        handlers.Remove(_handler);
    }
  }
}