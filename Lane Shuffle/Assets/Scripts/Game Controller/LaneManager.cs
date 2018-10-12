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
    [SerializeField]
    private float springSpeed = 10;
    [SerializeField]
    private float verticalDistance = 0.6f;

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
        laneStartPosition = draggedLane.transform.localPosition.x;
    }


    public void ContinueInteraction(Vector3 position)
    {
        if (!isDragging) return;

        // Move the dragged lane
        float draggedAmount = position.x - dragStartPosition.x;
        float lanePosition = laneStartPosition + draggedAmount;
        lanePosition = Mathf.Clamp(lanePosition, 0, laneCount - 1);

        Vector3 targetPosition = Vector3.right * lanePosition + Vector3.up * verticalDistance;
        draggedLane.transform.localPosition = new Vector3(
            targetPosition.x,
            Mathf.Lerp(draggedLane.transform.localPosition.y, targetPosition.y, Time.deltaTime * springSpeed),
            targetPosition.z);

        // Re-order the list if necessary
        int newLaneIndex = Mathf.RoundToInt(lanePosition);
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

                float distanceToDraggedLane = lanePosition - i;

                if (Mathf.Abs(distanceToDraggedLane) < 1)
                {
                    Vector3 droppedLanePosition = Vector3.right * (((float)draggedLaneIndex + i) / 2);
                    if (distanceToDraggedLane > 0)
                    { droppedLanePosition += Vector3.right * Mathf.Cos(distanceToDraggedLane * Mathf.PI) * 0.5f; }
                    else
                    { droppedLanePosition += Vector3.left * Mathf.Cos(distanceToDraggedLane * Mathf.PI) * 0.5f; }
                    lanes[i].transform.localPosition = droppedLanePosition;
                }
                else
                {
                    lanes[i].transform.localPosition = Vector3.right * i;
                }
            }
        }
    }


    public void EndInteraction()
    {
        isDragging = false;
    }


    private void MoveLanesTowardsCorrectPositions()
    {
        for (int i = 0; i < lanes.Count; i++)
        {
            Vector3 targetPosition = Vector3.right * i;
            lanes[i].transform.localPosition = Vector3.Lerp(lanes[i].transform.localPosition, targetPosition, Time.deltaTime * springSpeed);
        }
    }


    private int GetLaneIndexFromPosition(Vector3 position)
    {
        int laneIndex = Mathf.RoundToInt(laneParent.InverseTransformPoint(position).x);
        laneIndex = Mathf.Clamp(laneIndex, 0, laneCount-1);
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
