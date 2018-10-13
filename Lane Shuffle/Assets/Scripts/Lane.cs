using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [SerializeField]
    private float verticalDistance = 0.6f;

    // Actual Y position is a sin function of height variable. This makes the lane accelerate downwards as if by gravity.
    private float height = 0;
    public float Height
    {
        get
        {
            return height;
        }
        set
        {
            height = value;
            height = Mathf.Clamp01(height);

            float modifiedHeight = Mathf.Sin(height * Mathf.PI * 0.5f) * verticalDistance;

            transform.localPosition = new Vector3(transform.localPosition.x, modifiedHeight, transform.localPosition.z);
        }
    }

    public float XPosition
    {
        get
        {
            return transform.localPosition.x;
        }
        set
        {
            transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
