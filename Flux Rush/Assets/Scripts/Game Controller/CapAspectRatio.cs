using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I use this script to record 9:16 gameplay on my 16:9 PC.

public class CapAspectRatio : MonoBehaviour
{
    public float maxAspectRatio = 0.5f;

    private void Awake()
    {
        float aspectRatio = Screen.width / Screen.height;
        if (aspectRatio > maxAspectRatio)
        {
            Screen.SetResolution(
                Mathf.RoundToInt(Screen.height * maxAspectRatio),
                Screen.height,
                Screen.fullScreen);
        }
    }
}
