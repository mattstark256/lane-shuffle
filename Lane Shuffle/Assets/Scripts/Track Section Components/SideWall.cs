using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWall : MonoBehaviour {

    [SerializeField]
    private float distanceFromCentre = 0.3f;

    private int direction;

    private void Awake()
    {
        direction = (Random.value < 0.5f) ? -1 : 1;
    }

    void Start()
    {
        transform.localPosition += Vector3.right * direction * distanceFromCentre;
    }
}
