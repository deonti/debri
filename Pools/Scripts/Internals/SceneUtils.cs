using System.Collections.Generic;
using Debri.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Debri.Pools.Internals
{
  internal static class SceneUtils
  {
    private static readonly Dictionary<Scene, GlobalPoolsSettings> _settings = new();
    private static readonly Dictionary<Scene, Transform> _defaultContainers = new();

    public static Transform GetPoolItemsContainer(this Scene scene)
    {
      if (scene.TryGetSettings(out GlobalPoolsSettings settings)
          && settings.TryGetContainer(out Transform container))
        return container;

      if (!_defaultContainers.TryGetValue(scene, out Transform defaultContainer))
      {
        var gameObject = new GameObject("Scene Pool Items");
        defaultContainer = gameObject.transform;

        if (gameObject.scene != scene)
        {
          Debug.LogWarning("Unexpected scene for new GameObject", gameObject);
          SceneManager.MoveGameObjectToScene(gameObject, scene);
        }

        _defaultContainers.Add(scene, defaultContainer);
        _ = gameObject.SubscribeToOnDestroy(() => _defaultContainers.Remove(scene));
      }

      return defaultContainer;
    }

    private static bool TryGetSettings(this Scene scene, out GlobalPoolsSettings settings) =>
      _settings.TryGetValue(scene, out settings);

    public static void SetSettings(this Scene scene, GlobalPoolsSettings settings)
    {
      if (_settings.ContainsKey(scene))
      {
        Debug.LogError($"Global Pools Settings already exist for scene {scene.name}", settings);
        return;
      }

      _settings[scene] = settings;
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void ResetOnEnterPlayMode()
    {
      _settings.Clear();
      _defaultContainers.Clear();
    }
#endif
  }
}