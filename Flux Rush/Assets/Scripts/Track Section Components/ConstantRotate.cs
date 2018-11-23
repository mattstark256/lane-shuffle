using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour {

    [SerializeField]
    Vector3 rotateSpeed;

    void Update ()
    {
        Vector3 rotateAmount = rotateSpeed * Time.deltaTime;
        transform.localRotation *= Quaternion.Euler(rotateAmount.x, rotateAmount.y, rotateAmount.z);
    }
}
