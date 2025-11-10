using System;
using System.Collections.Generic;
using UnityEngine;

namespace Debri.Pools.Internals
{
  [DisallowMultipleComponent]
  internal class GameObjectPoolItem : MonoBehaviour
  {
    public GameObjectPool Owner { get; private set; }

    private readonly Queue<Action> _scheduledReleaseHandlers = new();

    private void OnDestroy() =>
      Owner.Remove(this);

    public static GameObjectPoolItem Instantiate(GameObjectPool owner, GameObject prototype, Transform parent)
    {
      GameObject gameObject = Instantiate(prototype, parent);
      var item = gameObject.AddComponent<GameObjectPoolItem>();
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
      gameObject.SetActive(false);

      foreach (IPoolItemReleaseHandler releaseHandler in GetComponents<IPoolItemReleaseHandler>())
        releaseHandler.OnRelease();
      while (_scheduledReleaseHandlers.TryDequeue(out Action onRelease))
        onRelease();
    }

    public void ScheduleReleaseHandler(Action onRelease) =>
      _scheduledReleaseHandlers.Enqueue(onRelease);
  }
}