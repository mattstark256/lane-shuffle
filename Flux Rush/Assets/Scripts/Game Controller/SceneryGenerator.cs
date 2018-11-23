using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackObjectManager))]
public class SceneryGenerator : MonoBehaviour
{
    private TrackObjectManager trackObjectManager;

    [SerializeField]
    private GameObject sceneryPrefab;
    [SerializeField]
    private float generateZ = 20f;
    [SerializeField]
    private float scenerySpacing = 6f;

    private float nextSceneryPosition = 0f;

    private float angle = 20;

    private void Awake()
    {
        trackObjectManager = GetComponent<TrackObjectManager>();
    }

    // Update is called once per frame
    void Update ()
    {
        nextSceneryPosition -= trackObjectManager.MoveDelta;
        while (nextSceneryPosition < generateZ)
        {
            angle += 10;

            GameObject newColumn = Instantiate(sceneryPrefab);
            newColumn.transform.position = Vector3.forward * nextSceneryPosition;
            //newColumn.transform.rotation = Quaternion.Euler(0, 0, angle);
            trackObjectManager.AddObjectToTrack(newColumn);

            nextSceneryPosition += scenerySpacing;
        }
    }
}
