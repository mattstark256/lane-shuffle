using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Track Section Object", menuName = "Track Section Object", order = 1)]
public class TrackSectionObject : TrackSection
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private Vector3 positionOffset;

    public override void Build(Lane lane, float startPosition, float sectionLength, TrackObjectManager trackObjectManager)
    {
        base.Build(lane, startPosition, sectionLength, trackObjectManager);

        GameObject newObject = Instantiate(prefab);
        newObject.transform.SetParent(lane.transform);
        newObject.transform.localPosition = Vector3.forward * (startPosition + sectionLength * 0.5f) + positionOffset;
        trackObjectManager.AddObjectToTrack(newObject);
    }
}
