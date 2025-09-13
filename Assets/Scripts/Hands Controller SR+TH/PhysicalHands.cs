using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalHands : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;
    private Collider[] handColliders;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        handColliders = GetComponentsInChildren<Collider>();
    }

    private void EnableHandColliders()
    {
        foreach (var item in handColliders)
        {
            item.enabled = true;
        }
    }

    public void EnableHandCollidersDelay(float delay)
    {
        Invoke("EnableHandColliders", delay);
    }

    public void DisableHandColliders()
    {
        foreach (var item in handColliders)
        {
            item.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        //Position
        rb.linearVelocity = (target.position - transform.position) / Time.fixedDeltaTime;

        //Rotation        
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
        Vector3 rotationDifferenceInDegree = angleInDegrees * rotationAxis;
        rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
    }
}
