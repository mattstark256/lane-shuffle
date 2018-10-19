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
    private Vector3 playerPositionOffset;
    [SerializeField]
    private SoundEffectManager audioEffects;

    private GameObject playerObject;
    private Lane currentLane;

    private int laneSwitchBuffer = 0; // This is used for queuing up inputs, so an input doesn't get ignored because it is too soon after another
    private bool isSwitchingLane = false;
    private Coroutine laneSwitchCoroutine;

    private bool isAlive = true;

    // The player will only collide with objects on their current lane/lanes. For example, if a dragged lane hasn't moved high enough when it passes over the player, we will ignore any collisions.
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            AddSwitchLaneInput(1);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            AddSwitchLaneInput(-1);
        }

        if (!isSwitchingLane && isAlive)
        {
            if (laneSwitchBuffer > 0) { SwitchLane(1); }
            if (laneSwitchBuffer < 0) { SwitchLane(-1); }
        }
    }


    public void AddSwitchLaneInput(int direction)
    {
        laneSwitchBuffer += direction;
    }


    private void SwitchLane(int direction)
    {
        laneSwitchBuffer -= direction;
        laneSwitchCoroutine = StartCoroutine(SwitchLaneCoroutine(direction));
    }


    private IEnumerator SwitchLaneCoroutine(int direction)
    {
        int currentLaneIndex = laneManager.GetLaneIndex(currentLane);
        int newLaneIndex = currentLaneIndex + direction;
        if (!laneManager.LaneExists(newLaneIndex)) yield break;

        isSwitchingLane = true;

        Lane oldLane = currentLane;
        Lane newLane = laneManager.GetLane(newLaneIndex);

        currentLane = newLane;
        collideableLanes.Add(newLane);
        playerObject.transform.SetParent(currentLane.transform, false);

        float f = 0;
        while (f < 1)
        {
            f += Time.deltaTime / laneSwitchDuration;
            f = Mathf.Clamp01(f);

            playerObject.transform.position = Vector3.Lerp(oldLane.transform.position, newLane.transform.position, laneSwitchPositionCurve.Evaluate(f)) + playerPositionOffset;

            Quaternion directionToNewLane = Quaternion.LookRotation(newLane.transform.position - oldLane.transform.position);
            playerObject.transform.localRotation = Quaternion.Slerp(Quaternion.identity, directionToNewLane, laneSwitchRotationCurve.Evaluate(f));

            yield return null;
        }

        collideableLanes.Remove(oldLane);
        isSwitchingLane = false;
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
            Die();
        }

        if (other.tag == "Coin")
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin == null) { Debug.Log("Can't find Coin script on coin!"); return; }
            CollectCoin(coin);
        }
    }


    private void Die()
    {
        if (!isAlive) return;
        isAlive = false;

        //Debug.Log("Died!");

        if (laneSwitchCoroutine != null)
        {
            StopCoroutine(laneSwitchCoroutine);
            isSwitchingLane = false;
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
