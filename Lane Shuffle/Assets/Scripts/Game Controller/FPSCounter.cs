using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private int sampleSize = 10;
    [SerializeField]
    private Text fPSText;
    [SerializeField]
    private Toggle fPSToggleCheckbox;

    private bool fPSCounterEnabled;
    private List<float> deltaTimes = new List<float>();


    private void Awake()
    {
        fPSCounterEnabled = (PlayerPrefs.GetInt("FPS Counter Enabled") == 1);
        fPSText.enabled = fPSCounterEnabled;
        fPSToggleCheckbox.isOn = fPSCounterEnabled;
    }


    void Update()
    {
        if (!fPSCounterEnabled) { return; }
        if (Time.deltaTime == 0) { return; }

        deltaTimes.Add(Time.deltaTime);
        if (deltaTimes.Count > sampleSize)
        {
            deltaTimes.RemoveAt(0);
        }

        float total = 0;
        foreach (float f in deltaTimes) { total += f; }
        float average = total / deltaTimes.Count;
        fPSText.text = "FPS: " + (1 / average).ToString("F2");
    }


    public bool FPSCounterEnabled
    {
        set
        {
            fPSCounterEnabled = value;
            int i = value ? 1 : 0;
            PlayerPrefs.SetInt("FPS Counter Enabled", i);
            fPSText.enabled = value;
        }
    }
}
