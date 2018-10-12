using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class generates, moves and destroys the objects on the lanes.
[RequireComponent(typeof(GameController))]
public class TrackObjectManager : MonoBehaviour
{
    [SerializeField]
    private float accelerationRate = 5f;
    private float moveSpeed = 0f;
    private float targetMoveSpeed = 0f;
    public float TargetMoveSpeed { set { targetMoveSpeed = value; } }

    [SerializeField]
    private float destroyZ = 10f;

    private List<GameObject> trackObjects = new List<GameObject>();
    public void AddObjectToTrack(GameObject newTrackObject) { trackObjects.Add(newTrackObject); }

    public float MoveDelta { get; private set; }


    private void Update()
    {
        AccelerateTowardsTargetSpeed();
        MoveDelta = moveSpeed * Time.deltaTime;

        // Move all objects along the track, destroy any that are behind the player
        List<GameObject> obstaclesToBeDestroyed = new List<GameObject>();
        foreach (GameObject obstacle in trackObjects)
        {
            if (obstacle == null)
            {
                obstaclesToBeDestroyed.Add(obstacle);
            }
            else
            {
                obstacle.transform.localPosition += Vector3.back * MoveDelta;
                if (obstacle.transform.localPosition.z < destroyZ)
                {
                    obstaclesToBeDestroyed.Add(obstacle);
                }
            }
        }
        while (obstaclesToBeDestroyed.Count > 0)
        {
            GameObject obstacle = obstaclesToBeDestroyed[0];
            obstaclesToBeDestroyed.RemoveAt(0);
            trackObjects.Remove(obstacle);
            Destroy(obstacle);
        }
    }


    private void AccelerateTowardsTargetSpeed()
    {
        if (moveSpeed < targetMoveSpeed)
        {
            moveSpeed += accelerationRate * Time.deltaTime;
            if (moveSpeed > targetMoveSpeed) { moveSpeed = targetMoveSpeed; }
        }

        if (moveSpeed > targetMoveSpeed)
        {
            moveSpeed -= accelerationRate * Time.deltaTime;
            if (moveSpeed < targetMoveSpeed) { moveSpeed = targetMoveSpeed; }
        }
    }
}
