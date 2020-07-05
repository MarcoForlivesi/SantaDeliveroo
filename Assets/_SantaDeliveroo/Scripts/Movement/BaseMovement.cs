using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    public MovementData movementData;

    private Rigidbody rigidbody;
    private float velocity;

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }


    protected void MoveTowards(Vector3 pointTarget)
    {
        Vector3 direction = pointTarget - transform.position;
        bool shouldRotate = Vector3.Angle(direction, transform.forward) > movementData.rotationThreshold;

        if (shouldRotate)
        {
            velocity = 0;

            Quaternion newRotation = Quaternion.LookRotation(pointTarget - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, movementData.rotationSpeed);
        }
        else
        {
            if (Vector3.Distance(transform.position, pointTarget) > movementData.pointThresholdDeceleration)
            {
                velocity += movementData.acceleration * Time.deltaTime;
                velocity = Mathf.Min(velocity, movementData.maxVelocity);
            }
            else
            {
                velocity -= movementData.acceleration * Time.deltaTime;
                velocity = Mathf.Max(velocity, 0);
            }

            //rigidbody.Add(pathLine.GetPosition(1));
            transform.position = Vector3.MoveTowards(transform.position, pointTarget, velocity);
        }
    }
}
