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
    private float horizontalSpeed = 15;
    [SerializeField]
    private float verticalSpeed = 6;

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
        float minimumHeight = Mathf.Abs(draggedLane.XPosition - laneStartPosition) * 3;
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
                    float laneX = (((float)draggedLaneIndex + i) / 2);
                    if (distanceToDraggedLane > 0)
                    { laneX += Mathf.Cos(distanceToDraggedLane * Mathf.PI) * 0.5f; }
                    else
                    { laneX -= Mathf.Cos(distanceToDraggedLane * Mathf.PI) * 0.5f; }
                    lanes[i].XPosition = (laneX);
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
        isDragging = false;
    }


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
