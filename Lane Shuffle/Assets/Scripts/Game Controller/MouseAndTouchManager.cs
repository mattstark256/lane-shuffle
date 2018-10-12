using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// This class deals with positional inputs, ie the mouse or touch.
[RequireComponent(typeof(LaneManager))]
public class MouseAndTouchManager : MonoBehaviour
{
    private LaneManager laneManager;

    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private LayerMask interactLayerMask;

    private bool inputEnabled = true;
    private int interactingFingerID = -1;
    private bool mouseDown = false;
    private bool InteractionInProgress { get { return interactingFingerID != -1 || mouseDown; } }


    private void Awake()
    {
        laneManager = GetComponent<LaneManager>();
    }


    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        if (enabled == false)
        {
            interactingFingerID = -1;
            mouseDown = false;
            EndInput();
        }
    }


    private void Update()
    {
        // This checks for touches before it checks for mouse input. This means if the device is a touchscreen, it doesn't treat a touch as a click.

        #region TouchControls

        if (inputEnabled &&
            !InteractionInProgress)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began &&
                    !PointIsOverUI(touch.position))
                {
                    interactingFingerID = touch.fingerId;
                    StartInput(touch.position);
                }
            }
        }

        if (interactingFingerID != -1)
        {
            if (GetTouchByFingerID(interactingFingerID).phase != TouchPhase.Ended)
            {
                ContinueInput(GetTouchByFingerID(interactingFingerID).position);

            }
            else
            {
                EndInput();
                interactingFingerID = -1;
            }
        }

        #endregion

        #region MouseControls

        if (inputEnabled &&
            !InteractionInProgress &&
            Input.GetButtonDown("Fire1") &&
            !PointIsOverUI(Input.mousePosition))
        {
            mouseDown = true;
            StartInput(Input.mousePosition);
        }

        if (mouseDown)
        {
            if (Input.GetButton("Fire1"))
            {
                ContinueInput(Input.mousePosition);
            }
            else
            {
                EndInput();
                mouseDown = false;
            }
        }

        #endregion
    }


    private void StartInput(Vector3 position)
    {
        Vector3 worldPosition = GetWorldPositionFromScreenPosition(position);
        laneManager.StartInteraction(worldPosition);
    }


    private void ContinueInput(Vector3 position)
    {
        Vector3 worldPosition = GetWorldPositionFromScreenPosition(position);
        laneManager.ContinueInteraction(worldPosition);
    }


    private void EndInput()
    {
        laneManager.EndInteraction();
    }


    // This function is required because Input.GetTouch() uses the array index of current touches, not the fingerID
    private Touch GetTouchByFingerID(int id)
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == id)
            {
                return touch;
            }
        }
        Debug.Log("can't find touch by fingerId! returning touch 0 instead");
        return Input.GetTouch(0);
    }


    // https://www.reddit.com/r/Unity3D/comments/2zc2xa/how_to_exlude_ui_from_touch_input/cphkhyo/
    private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>();
    public bool PointIsOverUI(Vector2 point)
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = point;
        tempRaycastResults.Clear();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, tempRaycastResults);
        return tempRaycastResults.Count > 0;
    }


    private Vector3 GetWorldPositionFromScreenPosition(Vector3 position)
    {
        Ray inputRay = playerCamera.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit, 1000, interactLayerMask))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
