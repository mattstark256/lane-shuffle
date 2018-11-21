using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController), typeof(LaneManager), typeof(TrackObjectManager))]
public class PlayerController : MonoBehaviour
{
    private GameController gameController;
    private LaneManager laneManager;
    private TrackObjectManager trackObjectManager;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject deathEffectPrefab;
    [SerializeField]
    private float laneSwitchDuration = 0.5f;
    [SerializeField]
    private AnimationCurve laneSwitchPositionCurve;
    [SerializeField]
    private AnimationCurve laneSwitchRotationCurve;
    [SerializeField]
    private float cancelSwitchDuration = 0.2f;
    //[SerializeField]
    //private AnimationCurve cancelSwitchCurve;
    [SerializeField]
    private AnimationCurve cancelSwitchPositionCurve;
    [SerializeField]
    private AnimationCurve cancelSwitchRotationCurve;
    [SerializeField, Tooltip("The point during a lane switch after which the switch can't be cancelled.")]
    private float cancelSwitchCutoff = 0.7f;
    //[SerializeField, Tooltip("Used to scale the angle the player switches to when they bounce off a wall")]
    //private float cancelSwitchBounceAngle = 0.6f;
    [SerializeField]
    private Vector3 playerPositionOffset;
    [SerializeField]
    private SoundEffectManager audioEffects;

    private GameObject playerObject;
    private bool isAlive = true;

    private Lane currentLane;
    private bool isSwitchingLane = false;
    private bool isCancellingSwitch = false;
    private bool canCancelSwitch = false; // This is true for part of the time the player is switching lanes. The size of this part depends on cancelSwitchCutoff.
    private Coroutine laneSwitchCoroutine;
    private Coroutine cancelSwitchCoroutine;

    // This is used for queuing up inputs, so an input doesn't get ignored because it is too soon after another.
    private List<int> inputQueue = new List<int>();
    // The player will only collide with objects on their current lane/lanes. For example, if a dragged lane hasn't moved high enough when it passes over the player, we ignore any collisions.
    private List<Lane> collideableLanes = new List<Lane>();


    private void Awake()
    {
        gameController = GetComponent<GameController>();
        laneManager = GetComponent<LaneManager>();
        trackObjectManager = GetComponent<TrackObjectManager>();

        playerObject = Instantiate(playerPrefab);
        playerObject.GetComponent<PlayerCollisions>().playerController = this;
    }


    private void Start()
    {
        currentLane = laneManager.GetLane(1);
        collideableLanes.Add(currentLane);
        playerObject.transform.SetParent(currentLane.transform, false);
        playerObject.transform.localPosition = playerPositionOffset;
    }


    void Update()
    {
        // Controls for desktop version
        if (Input.GetKeyDown(KeyCode.D)) { AddSwitchLaneInput(1); }
        if (Input.GetKeyDown(KeyCode.A)) { AddSwitchLaneInput(-1); }

        if (inputQueue.Count > 0 &&
            !isSwitchingLane &&
            !isCancellingSwitch &&
            isAlive)
        {
            SwitchLane(inputQueue[0]);
            inputQueue.RemoveAt(0);
        }
    }


    public void AddSwitchLaneInput(int direction)
    {
        inputQueue.Add(direction);
    }


    private void SwitchLane(int direction) { laneSwitchCoroutine = StartCoroutine(SwitchLaneCoroutine(direction)); }
    private IEnumerator SwitchLaneCoroutine(int direction)
    {
        int currentLaneIndex = laneManager.GetLaneIndex(currentLane);
        int newLaneIndex = currentLaneIndex + direction;
        if (!laneManager.LaneExists(newLaneIndex)) yield break;

        isSwitchingLane = true;
        canCancelSwitch = true;

        Lane oldLane = currentLane;
        Lane newLane = laneManager.GetLane(newLaneIndex);

        collideableLanes.Add(newLane);

        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / laneSwitchDuration;
            f = Mathf.Clamp01(f);

            playerObject.transform.position = Vector3.Lerp(oldLane.transform.position, newLane.transform.position, laneSwitchPositionCurve.Evaluate(f)) + playerPositionOffset;

            Quaternion directionToNewLane = Quaternion.LookRotation(newLane.transform.position - oldLane.transform.position);
            playerObject.transform.localRotation = Quaternion.Slerp(Quaternion.identity, directionToNewLane, laneSwitchRotationCurve.Evaluate(f));

            // There is a cutoff for when a switch can be cancelled. This is because it looks weird when a 99% complete switch is cancelled.
            if (f > cancelSwitchCutoff) { canCancelSwitch = false; }

            yield return null;
        }

        collideableLanes.Remove(oldLane);

        currentLane = newLane;

        playerObject.transform.SetParent(currentLane.transform, true);
        playerObject.transform.localPosition = playerPositionOffset; // This line is necessary because worldPositionStays very occasionally doesn't work correctly, and the player jumps to a position that doesn't match their world position. I haven't yet figured out why this happens.

        isSwitchingLane = false;
    }


    private void CancelSwitch() { cancelSwitchCoroutine = StartCoroutine(CancelSwitchCoroutine()); }
    private IEnumerator CancelSwitchCoroutine()
    {
        audioEffects.PlayEffect("Thud");

        StopCoroutine(laneSwitchCoroutine);

        isSwitchingLane = false;
        isCancellingSwitch = true;
        canCancelSwitch = false;

        // Disable collisions with the lane they were trying to switch to.
        collideableLanes.Clear();
        collideableLanes.Add(currentLane);

        Vector3 cancelPosition = playerObject.transform.localPosition;
        Quaternion cancelRotation = playerObject.transform.localRotation;

        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / cancelSwitchDuration;
            f = Mathf.Clamp01(f);

            playerObject.transform.localPosition = Vector3.Lerp(cancelPosition, playerPositionOffset, cancelSwitchPositionCurve.Evaluate(f));
            playerObject.transform.localRotation = Quaternion.SlerpUnclamped(Quaternion.identity, cancelRotation, cancelSwitchRotationCurve.Evaluate(f));

            yield return null;
        }

        isCancellingSwitch = false;
    }


    public void PlayerEnteredCollider(Collider other)
    {
        // Ignore collisions with objects that are not on a lane in collideableLanes.
        bool colliderIsOnCollidableLane = false;
        foreach (Lane lane in collideableLanes)
        {
            if (other.transform.IsChildOf(lane.transform)) { colliderIsOnCollidableLane = true; }
        }
        if (!colliderIsOnCollidableLane) return;

        if (other.tag == "Obstacle")
        {
            // The player should bounce back if they moved sideways by accident.
            if (canCancelSwitch && !ObstacleBlocksCurrentLane(other))
            {
                CancelSwitch();
                return;
            }

            // The player shouldn't be able to hit things while cancelling, unless they block the current lane.
            if (isCancellingSwitch && !ObstacleBlocksCurrentLane(other))
            {
                return;
            }

            Die();
        }

        if (other.tag == "Coin")
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin == null) { Debug.Log("Can't find Coin script on coin!"); return; }
            CollectCoin(coin);
        }
    }


    // This is needed because you shouldn't be able to cancel from a head on collision with an obstacle in your current lane
    private bool ObstacleBlocksCurrentLane(Collider obstacle)
    {
        return
            obstacle.transform.localPosition.x == 0 && // If it's in the middle of the lane (ie ignore side walls)
            obstacle.transform.IsChildOf(currentLane.transform); // and it's a child of the current lane
    }


    private void Die()
    {
        if (!isAlive) return;
        isAlive = false;

        //Debug.Log("Died!");

        if (isSwitchingLane)
        {
            StopCoroutine(laneSwitchCoroutine);
            isSwitchingLane = false;
        }

        if (isCancellingSwitch)
        {
            StopCoroutine(cancelSwitchCoroutine);
            isCancellingSwitch = false;
        }

        GameObject deathEffect = Instantiate(deathEffectPrefab);
        deathEffect.transform.SetParent(currentLane.transform);
        deathEffect.transform.localPosition = playerObject.transform.localPosition;
        deathEffect.transform.localRotation = playerObject.transform.localRotation;
        trackObjectManager.AddObjectToTrack(deathEffect);

        Destroy(playerObject);
        gameController.GameOver();
    }


    private void CollectCoin(Coin coin)
    {
        coin.Collect();
        gameController.AddToScore(coin.Value);
        audioEffects.PlayEffect("Coin");
    }
}
