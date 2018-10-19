using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class ColorChanger : MonoBehaviour
{
    private GameController gameController;

    [SerializeField]
    private Color[] tintColors;
    [SerializeField]
    private int tintIndex = 0;
    [SerializeField]
    private float tintChangeInterval = 30f;
    [SerializeField]
    private float tintChangeDuration = 10f;

    [SerializeField]
    Material wallMaterial;
    [SerializeField]
    Material skyMaterial;
    [SerializeField]
    Material columnMaterial;


    [SerializeField]
    private Color wallBase = Color.grey;
    [SerializeField]
    private float wallTintAmount = 0.5f;
    [SerializeField]
    private Color skyBase = Color.white;
    [SerializeField]
    private float skyTintAmount = 0.5f;

    private Color tintColor;
    private float timeUntilNextTintChange;


    private void Awake()
    {
        gameController = GetComponent<GameController>();

        tintColor = tintColors[tintIndex];
        UpdateTintColor();

        timeUntilNextTintChange = tintChangeInterval;
    }


    void Update()
    {
        if (!gameController.GameIsInProgress) { return; }

        timeUntilNextTintChange -= Time.deltaTime;

        if (timeUntilNextTintChange <= 0)
        {
            timeUntilNextTintChange += tintChangeInterval;
            StartCoroutine(FadeToNextTint());
        }
    }


    private IEnumerator FadeToNextTint()
    {
        Color oldTint = tintColors[tintIndex];
        tintIndex += 1;
        if (tintIndex >= tintColors.Length) { tintIndex = 0; }
        Color newTint = tintColors[tintIndex];

        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / tintChangeDuration;
            f = Mathf.Clamp01(f);
            float smoothedf = (1 - Mathf.Cos(f * Mathf.PI)) / 2;

            tintColor = Color.Lerp(oldTint, newTint, smoothedf);
            UpdateTintColor();

            yield return null;
        }
    }


    private void UpdateTintColor()
    {
        Color wallColor = Color.Lerp(wallBase, tintColor, wallTintAmount);
        Color skyColor = Color.Lerp(skyBase, tintColor, skyTintAmount);

        wallMaterial.SetVector("_MainColor", wallColor);
        skyMaterial.SetVector("_Color", skyColor);
        columnMaterial.SetVector("_Color2", skyColor);
    }
}
