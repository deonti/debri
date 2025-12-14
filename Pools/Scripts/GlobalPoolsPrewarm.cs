using System;
using UnityEngine;

namespace Debri.Pools
{
  [AddComponentMenu("Debri/Pools/Global Pools Prewarm")]
  [DefaultExecutionOrder(-999)]
  public class GlobalPoolsPrewarm : MonoBehaviour
  {
    [SerializeField] private ItemSettings[] _items;

    private void Awake()
    {
      foreach (ItemSettings settings in _items)
        GlobalPools.Get(settings.ItemPrototype).Prewarm(settings.Count);
    }

    [Serializable]
    private class ItemSettings
    {
      public GameObject ItemPrototype;
      public int Count;
    }
  }
}