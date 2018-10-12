using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Track Section", menuName = "Track Section", order = 1)]
public class TrackSection : ScriptableObject
{
    public virtual void Build(Lane lane, float startPosition, float sectionLength, TrackObjectManager trackObjectManager)
    {

    }
}
