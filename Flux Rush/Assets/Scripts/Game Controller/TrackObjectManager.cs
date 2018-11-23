using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class generates, moves and destroys the objects on the lanes.
[RequireComponent(typeof(TrackSpeed))]
public class TrackObjectManager : MonoBehaviour
{
    private TrackSpeed trackSpeed;

    [SerializeField]
    private float destroyZ = 10f;

    public float MoveDelta { get; private set; }

    private List<GameObject> trackObjects = new List<GameObject>();
    public void AddObjectToTrack(GameObject newTrackObject) { trackObjects.Add(newTrackObject); }


    private void Awake()
    {
        trackSpeed = GetComponent<TrackSpeed>();
    }


    private void Update()
    {
        MoveDelta = trackSpeed.Speed * Time.deltaTime;

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
}
