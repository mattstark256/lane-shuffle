using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class generates the lanes, keeps track of their relative positions and gives them instructions. It deals with the re-ordering when the player drags a lane.
public class LaneManager : MonoBehaviour
{
    [SerializeField]
    private Lane lanePrefab;
    [SerializeField]
    private int laneCount = 4;
    public int LaneCount { get { return laneCount; } }
    [SerializeField]
    private Transform laneParent;
    [SerializeField, Tooltip("How quickly lanes lerp horizontally towards the correct position")]
    private float horizontalSpeed = 17;
    [SerializeField, Tooltip("How quickly lanes rise and fall when picked up")]
    private float verticalSpeed = 7;
    [SerializeField]
    private float dragDistanceForLaneToBeFullyRaised = 0.4f;
    [SerializeField, Tooltip("How sharply/suddenly a lane slides under the dragged one")]
    private float slideUnderSharpness = 0.5f;
    [SerializeField, Tooltip("The maximum extra distance a lane can travel when dropped. When 0, lanes just travel to the nearest slot.")]
    private float momentumMaxDistance = 0.3f;
    [SerializeField, Tooltip("Steepness of momentum function (based on atan) at 0.")]
    private float momentumSteepness = 0.4f;

    private List<Lane> lanes = new List<Lane>();

    private bool isDragging;
    private int draggedLaneIndex;
    private Lane draggedLane;
    private float laneStartPosition;
    private Vector3 dragStartPosition;


    private void Awake()
    {
        GenerateLanes();
    }


    private void Update()
    {
        if (!isDragging)
        {
            MoveLanesTowardsCorrectPositions();
        }
    }


    private void GenerateLanes()
    {
        laneParent.position = Vector3.left * (laneCount - 1) / 2;

        for (int i = 0; i < laneCount; i++)
        {
            Lane newLane = Instantiate(lanePrefab);
            newLane.transform.SetParent(laneParent);
            newLane.transform.localPosition = Vector3.right * i;
            lanes.Add(newLane);
        }
    }


    public void StartInteraction(Vector3 position)
    {
        draggedLaneIndex = GetLaneIndexFromPosition(position);
        draggedLane = lanes[draggedLaneIndex];
        isDragging = true;
        dragStartPosition = position;
        laneStartPosition = draggedLane.XPosition;
    }


    public void ContinueInteraction(Vector3 position)
    {
        if (!isDragging) return;

        // Move the dragged lane and record its velocity
        float draggedAmount = position.x - dragStartPosition.x;
        float newXPosition = laneStartPosition + draggedAmount;
        newXPosition = Mathf.Clamp(newXPosition, 0, laneCount - 1);
        draggedLane.XPosition = newXPosition;

        // Raise the dragged lane
        draggedLane.Height += Time.deltaTime * verticalSpeed;
        // Prevent intersections with other lanes if it's been dragged very fast
        float minimumHeight = Mathf.Abs(draggedLane.XPosition - laneStartPosition) / dragDistanceForLaneToBeFullyRaised;
        if (draggedLane.Height < minimumHeight) { draggedLane.Height = minimumHeight; }

        // Re-order the list if necessary
        int newLaneIndex = Mathf.RoundToInt(draggedLane.XPosition);
        if (newLaneIndex != draggedLaneIndex)
        {
            lanes.RemoveAt(draggedLaneIndex);
            lanes.Insert(newLaneIndex, draggedLane);
            draggedLaneIndex = newLaneIndex;
        }

        // Update the positions of the other lanes
        for (int i = 0; i < lanes.Count; i++)
        {
            if (i != draggedLaneIndex)
            {
                lanes[i].Height += Time.deltaTime * -verticalSpeed;

                float distanceToDraggedLane = draggedLane.XPosition - i;

                if (Mathf.Abs(distanceToDraggedLane) < 1)
                {
                    float midpointX = ((float)draggedLaneIndex + i) / 2;

                    float offsetA = Mathf.Cos(distanceToDraggedLane * Mathf.PI); // Trig wave
                    float offsetB = Mathf.Sin(offsetA * Mathf.PI / 2); // Trig wave from trig wave (sharper transition)
                    float offset = Mathf.Lerp(offsetA, offsetB, slideUnderSharpness);
                    offset /= 2;

                    if (distanceToDraggedLane > 0)
                    { lanes[i].XPosition = midpointX + offset; }
                    else
                    { lanes[i].XPosition = midpointX - offset; }
                }
                else
                {
                    lanes[i].XPosition = (i);
                }
            }
        }
    }


    public void EndInteraction()
    {
        if (!isDragging) return;

        isDragging = false;

        // When the player stops dragging a lane, its new index is determined by both its position and its velocity
        int newLaneIndex = Mathf.RoundToInt(draggedLane.XPosition + GetMomentumDistance(draggedLane));
        if (newLaneIndex != draggedLaneIndex)
        {
            lanes.RemoveAt(draggedLaneIndex);
            lanes.Insert(newLaneIndex, draggedLane);
            draggedLaneIndex = newLaneIndex;
        }
    }


    private float GetMomentumDistance(Lane lane)
    {
        return Mathf.Atan(lane.RecordedVelocity * momentumSteepness) * 2 / Mathf.PI * momentumMaxDistance;
    }
    //// Used for testing the momentum values
    //private void OnDrawGizmos()
    //{
    //    if (isDragging)
    //    {
    //        Gizmos.color = Color.red;
    //        Vector3 startPosition = draggedLane.transform.position;
    //        Vector3 endPosition = Vector3.right * (startPosition.x + GetMomentumDistance(draggedLane));
    //        Gizmos.DrawLine(startPosition, endPosition);
    //    }
    //}


    private void MoveLanesTowardsCorrectPositions()
    {
        for (int i = 0; i < lanes.Count; i++)
        {
            lanes[i].Height -= Time.deltaTime * verticalSpeed;

            lanes[i].XPosition = Mathf.Lerp(lanes[i].XPosition, i, Time.deltaTime * horizontalSpeed);
        }
    }


    private int GetLaneIndexFromPosition(Vector3 position)
    {
        int laneIndex = Mathf.RoundToInt(laneParent.InverseTransformPoint(position).x);
        laneIndex = Mathf.Clamp(laneIndex, 0, laneCount - 1);
        return laneIndex;
    }


    public int GetLaneIndex(Lane lane)
    {
        for (int i = 0; i < lanes.Count; i++)
        {
            if (lanes[i] == lane)
            {
                return i;
            }
        }
        return 0;
    }


    public Lane GetLane(int laneIndex)
    {
        return lanes[laneIndex];
    }


    public bool LaneExists(int laneIndex)
    {
        return laneIndex >= 0 && laneIndex < laneCount;
    }
}
