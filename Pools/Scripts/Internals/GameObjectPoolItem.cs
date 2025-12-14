using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Debri.Pools.Internals
{
  [DisallowMultipleComponent]
  internal class GameObjectPoolItem : MonoBehaviour
  {
    public GameObjectPool Owner { get; private set; }

    private readonly Queue<Action> _scheduledReleaseHandlers = new();
    private static Transform _tempParent;

    private void OnDestroy()
    {
      if (!GlobalPools.SuppressWarnings && gameObject.scene.isLoaded)
        Owner.LogWarning($"Pool item {gameObject.name} has been destroyed and will be removed from pool");

      Owner.Remove(this);
    }

    public static GameObjectPoolItem Instantiate(GameObjectPool owner, GameObject prototype, Transform parent)
    {
      GameObject gameObject = Instantiate(prototype, GetTempParent());
      gameObject.SetActive(false);
      gameObject.transform.SetParent(parent, false);
      if (!parent)
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

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
      foreach (IPoolItemReleaseHandler releaseHandler in GetComponents<IPoolItemReleaseHandler>())
        releaseHandler.OnRelease();
      while (_scheduledReleaseHandlers.TryDequeue(out Action onRelease))
        onRelease();

      gameObject.SetActive(false);
    }

    public void ScheduleReleaseHandler(Action onRelease) =>
      _scheduledReleaseHandlers.Enqueue(onRelease);

    private static Transform GetTempParent()
    {
      if (_tempParent)
        return _tempParent;

      var tempParentObject = new GameObject("Temp PoolItems Parent");
      tempParentObject.SetActive(false);
      tempParentObject.hideFlags = HideFlags.HideAndDontSave;
      DontDestroyOnLoad(tempParentObject);
      _tempParent = tempParentObject.transform;
      return _tempParent;
    }
  }
}