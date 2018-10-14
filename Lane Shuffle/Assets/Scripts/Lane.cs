using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // for list average

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
            HorizontalVelocity = (value - XPosition) / Time.deltaTime;

            transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z);
        }
    }


    private float horizontalVelocity = 0; // This is a moving average of several values
    private List<float> velocities = new List<float>();
    private const int maxVelocitiesCount = 5;
    public float HorizontalVelocity
    {
        get
        {
            return horizontalVelocity;
        }
        private set
        {
            if (velocities.Count >= maxVelocitiesCount) { velocities.RemoveAt(0); }
            velocities.Add(value);
            horizontalVelocity = velocities.Average();
        }
    }
}
