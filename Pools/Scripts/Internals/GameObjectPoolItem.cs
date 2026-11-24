using System;
using System.Collections.Generic;
using Debri.Common;
using UnityEngine;

namespace Debri.Pools.Internals
{
  [DisallowMultipleComponent]
  internal class GameObjectPoolItem : MonoBehaviour
  {
    public GameObjectPool Owner { get; private set; }

    private readonly Queue<ReleaseHandlerInfo> _scheduledReleaseHandlers = new();

    private void OnDestroy()
    {
      if (!GlobalPools.SuppressWarnings && gameObject.scene.isLoaded)
        Owner.LogWarning($"Pool item {gameObject.name} has been destroyed and will be removed from pool");

      Owner.Remove(this);
      while (_scheduledReleaseHandlers.TryDequeue(out ReleaseHandlerInfo handlerInfo))
        if (handlerInfo.CallByDestroy)
          handlerInfo.Handler();
    }

    public static GameObjectPoolItem Instantiate(GameObjectPool owner, GameObject prototype, Transform parent)
    {
      var item = ObjectUtils
        .InstantiateInactive(prototype, parent)
        .AddComponent<GameObjectPoolItem>();
      item.hideFlags = HideFlags.HideAndDontSave;
      item.Owner = owner;
      return item;
    }

    public void ProcessGet()
    {
      gameObject.SetActive(true);

      foreach (IPoolItemGetHandler getHandler in GetComponents<IPoolItemGetHandler>())
        getHandler.OnGet();
    }

    public void ProcessRelease()
    {
      foreach (IPoolItemReleaseHandler releaseHandler in GetComponents<IPoolItemReleaseHandler>())
        releaseHandler.OnRelease();
      while (_scheduledReleaseHandlers.TryDequeue(out ReleaseHandlerInfo handlerInfo))
        handlerInfo.Handler();

      gameObject.SetActive(false);
    }

    public void ScheduleReleaseHandler(Action handler, bool callByDestroy) =>
      _scheduledReleaseHandlers.Enqueue(new ReleaseHandlerInfo(handler, callByDestroy));

    private readonly struct ReleaseHandlerInfo
    {
      public readonly Action Handler;
      public readonly bool CallByDestroy;

      public ReleaseHandlerInfo(Action handler, bool callByDestroy)
      {
        Handler = handler;
        CallByDestroy = callByDestroy;
      }
    }
  }
}