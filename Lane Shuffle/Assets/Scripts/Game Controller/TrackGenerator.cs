using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackObjectManager), typeof(LaneManager))]
public class TrackGenerator : MonoBehaviour
{
    private TrackObjectManager trackObjectManager;
    private LaneManager laneManager;

    [SerializeField]
    private float generateZ = 20f;
    [SerializeField]
    private float startSectionLength = 5f;
    [SerializeField]
    private float sectionLength = 3f;

    [SerializeField]
    TrackSection blankSection;
    [SerializeField]
    TrackSection solidSection;
    [SerializeField]
    TrackSection sideWallSection;
    [SerializeField]
    TrackSection coinSection;
    [SerializeField]
    TrackSection slideyWallSection;
    [SerializeField]
    TrackSection hiddenWallForwardSection;
    [SerializeField]
    TrackSection hiddenWallBackSection;

    private float nextTrackSectionPosition = 0f;
    private int generatedRowCount = 0; // The number of horizontal rows of track that have been generated

    // This class keeps its own copy of "lanes", because laneManager keeps re-arranging its copy.
    private List<Lane> lanes;
    // It uses a an array of lists to store upcoming track sections for each lane
    private List<TrackSection>[] upcomingSections;

    private void Awake()
    {
        trackObjectManager = GetComponent<TrackObjectManager>();
        laneManager = GetComponent<LaneManager>();

        nextTrackSectionPosition = startSectionLength;
    }

    void Start ()
    {
        lanes = new List<Lane>();
        for (int i = 0; i < laneManager.LaneCount; i++)
        {
            lanes.Add(laneManager.GetLane(i));
        }

        upcomingSections = new List<TrackSection>[laneManager.LaneCount];
        for (int i = 0; i < upcomingSections.Length; i++)
        {
            upcomingSections[i] = new List<TrackSection>();
        }
    }
	
	void Update ()
    {
        // Generate a new row if necessary
        nextTrackSectionPosition -= trackObjectManager.MoveDelta;
        while (nextTrackSectionPosition < generateZ)
        {
            Add2Gap();

            if (generatedRowCount > 0 && Random.value < 0.5f)
            {
                AddCoinSection();
            }

            if (generatedRowCount > 3 && Random.value < 0.67f)
            {
                float r = Random.value;
                if (generatedRowCount > 30 && r < 0.2f)
                {
                    AddSlideyWall();
                }
                else if(generatedRowCount > 60 && r >= 0.2f && r < 0.4f)
                {
                    AddHiddenWall();
                }
                else
                {
                    AddSideWall();
                }
            }

            FillEmptySections();

            // For each lane
            for (int i = 0; i < upcomingSections.Length; i++)
            {
                if (upcomingSections[i].Count == 0)
                {
                    Debug.Log("One of the upcomingSections lists is empty! This shouldn't have happened!");
                }
                else if (upcomingSections[i][0] == null)
                {
                    Debug.Log("One of the upcomingSections lists holds a null value! This shouldn't have happened!");
                    upcomingSections[i].RemoveAt(0);
                }
                else
                {
                    // Build the queued section
                    upcomingSections[i][0].Build(lanes[i], nextTrackSectionPosition, sectionLength, trackObjectManager);
                    // Then remove it from the queue
                    upcomingSections[i].RemoveAt(0);
                }
            }

            nextTrackSectionPosition += sectionLength;
            generatedRowCount++;
        }
    }


    // Pick a lane with no queue and add 2 blank then 1 solid
    private void Add2Gap()
    {
        List<int> lanesWithNoQueue = GetLanesWithNoQueue();
        if (lanesWithNoQueue.Count == 0) return;
        int lane = lanesWithNoQueue[Random.Range(0, lanesWithNoQueue.Count)];

        upcomingSections[lane].Add(blankSection);
        upcomingSections[lane].Add(blankSection);
        upcomingSections[lane].Add(solidSection);
    }


    // Pick a lane with a blank at the front of the queue and replace it with a side wall
    private void AddSideWall()
    {
        List<int> lanesWithBlankSection = GetLanesWithSectionType(blankSection);
        if (lanesWithBlankSection.Count == 0) return;
        int lane = lanesWithBlankSection[Random.Range(0, lanesWithBlankSection.Count)];

        upcomingSections[lane].RemoveAt(0);
        upcomingSections[lane].Insert(0, sideWallSection);
    }


    private void AddSlideyWall()
    {
        List<int> lanesWithBlankSection = GetLanesWithSectionType(blankSection);
        if (lanesWithBlankSection.Count == 0) return;
        int lane = lanesWithBlankSection[Random.Range(0, lanesWithBlankSection.Count)];

        upcomingSections[lane].RemoveAt(0);
        upcomingSections[lane].Insert(0, slideyWallSection);
    }


    private void AddHiddenWall()
    {
        List<int> lanesWithBlankSection = GetLanesWithSectionType(blankSection);
        if (lanesWithBlankSection.Count == 0) return;
        int lane = lanesWithBlankSection[Random.Range(0, lanesWithBlankSection.Count)];

        TrackSection sideWallSection = (upcomingSections[lane][1]==solidSection) ? hiddenWallForwardSection : hiddenWallBackSection;
        upcomingSections[lane].RemoveAt(0);
        upcomingSections[lane].Insert(0, sideWallSection);
    }


    // Pick a lane with no queue and add 1 coin then 1 solid
    private void AddCoinSection()
    {
        List<int> lanesWithNoQueue = GetLanesWithNoQueue();
        if (lanesWithNoQueue.Count == 0) return;
        int lane = lanesWithNoQueue[Random.Range(0, lanesWithNoQueue.Count)];

        upcomingSections[lane].Add(coinSection);
        upcomingSections[lane].Add(solidSection);
    }


    // Add 1 solid to all lanes with no queue
    private void FillEmptySections()
    {
        List<int> lanesWithNoQueue = GetLanesWithNoQueue();
        foreach (int i in lanesWithNoQueue)
        {
            upcomingSections[i].Add(solidSection);
        }
    }


    private List<int> GetLanesWithNoQueue()
    {
        List<int> lanesWithNoQueue = new List<int>();
        for (int i = 0; i < upcomingSections.Length; i++)
        {
            if (upcomingSections[i].Count == 0)
            {
                lanesWithNoQueue.Add(i);
            }
        }
        return lanesWithNoQueue;
    }


    // Get all lanes which have the specified section type at the front of the queue
    private List<int> GetLanesWithSectionType(TrackSection sectionType)
    {
        List<int> lanesWithCorrectType = new List<int>();
        for (int i = 0; i < upcomingSections.Length; i++)
        {
            if (upcomingSections[i].Count > 0 &&
                upcomingSections[i][0] == sectionType)
            {
                lanesWithCorrectType.Add(i);
            }
        }
        return lanesWithCorrectType;
    }
}
