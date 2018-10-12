using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int value = 1;
    public int Value { get { return value; } }

    [SerializeField]
    private GameObject gfxObject;
    [SerializeField]
    private float animationDuration = 0.5f;

	public void Collect()
    {
        StartCoroutine(CollectCoroutine());
    }

    private IEnumerator CollectCoroutine()
    {
        Vector3 initialScale = gfxObject.transform.localScale;

        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / animationDuration;
            f = Mathf.Clamp01(f);

            gfxObject.transform.localScale = initialScale * Mathf.Cos(f * Mathf.PI / 2);

            yield return null;
        }

        Destroy(gameObject);
    }
}
