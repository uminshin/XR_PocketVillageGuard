using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    public bool IsFrozen { get; private set; }

    private Coroutine freezeCoroutine;

    public void Freeze(float duration)
    {
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
        }

        freezeCoroutine = StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        IsFrozen = true;

        Debug.Log($"{gameObject.name} is frozen.");

        // 나중에 여기에서 얼음 이펙트나 얼음 재질 변경을 넣으면 됨

        yield return new WaitForSeconds(duration);

        IsFrozen = false;
        freezeCoroutine = null;

        Debug.Log($"{gameObject.name} freeze ended.");
    }
}
