using UnityEngine;
using UnityEngine.SceneManagement;

namespace Debri.Common
{
  public static class ObjectUtils
  {
    private static Transform _tempParent;

    /// <summary>
    /// Clones the object original and returns inactive clone.
    /// </summary>
    public static GameObject InstantiateInactive(GameObject original, Transform parent = null)
    {
      GameObject instance;
      if (parent && !parent.gameObject.activeInHierarchy)
      {
        instance = Object.Instantiate(original, parent);
        instance.gameObject.SetActive(false);
      }
      else
      {
        instance = Object.Instantiate(original, GetTempParent());
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(parent, false);

        if (!parent)
          SceneManager.MoveGameObjectToScene(instance.gameObject, SceneManager.GetActiveScene());
      }

      return instance;
    }

    /// <summary>
    /// Clones the object original and returns inactive clone.
    /// </summary>
    public static TComponent InstantiateInactive<TComponent>(TComponent original, Transform parent = null)
      where TComponent : Component =>
      InstantiateInactive(original.gameObject, parent).GetComponent<TComponent>();

    private static Transform GetTempParent()
    {
      if (_tempParent)
        return _tempParent;

      var tempParentObject = new GameObject("ObjectUtils/Temp Parent");
      tempParentObject.SetActive(false);
      tempParentObject.hideFlags = HideFlags.HideAndDontSave;
      Object.DontDestroyOnLoad(tempParentObject);
      _tempParent = tempParentObject.transform;
      return _tempParent;
    }
  }
}