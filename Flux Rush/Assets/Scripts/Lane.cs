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


    // This shouldn't be set more than once per update.
    public float XPosition
    {
        get
        {
            return transform.localPosition.x;
        }
        set
        {
            RecordVelocity((value - XPosition) / Time.deltaTime);

            transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z);
        }
    }


    // The recorded velocity is a moving average of the last 5 velocities.
    public float RecordedVelocity { get; private set; }
    private List<float> recordedVelocities = new List<float>();
    private const int maxRecordedVelocitiesCount = 5;
    private void RecordVelocity(float velocity)
    {
        if (recordedVelocities.Count >= maxRecordedVelocitiesCount) { recordedVelocities.RemoveAt(0); }
        recordedVelocities.Add(velocity);
        RecordedVelocity = recordedVelocities.Average();
    }
}
