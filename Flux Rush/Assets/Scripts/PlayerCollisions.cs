using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used because "OnTriggerEnter" must be on the same object as the collider component
public class PlayerCollisions : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        playerController.PlayerEnteredCollider(other);
    }
}
