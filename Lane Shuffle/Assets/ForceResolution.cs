using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceResolution : MonoBehaviour {

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
