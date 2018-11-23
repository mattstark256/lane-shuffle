using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I use this script to record 9:16 gameplay on my 16:9 PC.

public class ForceResolution : MonoBehaviour
{
    public KeyCode keyCode;
    public Vector2Int resolution = new Vector2Int(607, 1080);

	void Update ()
    {
		if (Input.GetKeyDown(keyCode))
        {
            Screen.SetResolution(resolution.x, resolution.y, true);
        }
	}
}
