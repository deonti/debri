using System.Collections;
using UnityEngine;

namespace Debri.Common.Internal
{
  internal class CoroutineRunner : MonoBehaviour
  {
    public static Coroutine StartCoroutine(GameObject gameObject, IEnumerator routine) =>
      gameObject.GetOrAddComponent<CoroutineRunner>(Initialize).StartCoroutine(routine);

    private static void Initialize(CoroutineRunner runner) =>
      runner.hideFlags = HideFlags.HideAndDontSave;
  }
}