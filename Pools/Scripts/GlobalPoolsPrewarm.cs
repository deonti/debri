using System;
using UnityEngine;

namespace Debri.Pools
{
  [AddComponentMenu("Debri/Pools/Global Pools Prewarm")]
  [DefaultExecutionOrder(-999)]
  public class GlobalPoolsPrewarm : MonoBehaviour
  {
    [SerializeField] private Transform _container;
    [SerializeField] private ItemSettings[] _items;

    private void Reset() =>
      _container = transform;

    private void Awake()
    {
      foreach (ItemSettings settings in _items)
        GlobalPools.Get(settings.ItemPrototype, _container).Prewarm(settings.Count);
    }

    [Serializable]
    private class ItemSettings
    {
      public GameObject ItemPrototype;
      public int Count;
    }
  }
}