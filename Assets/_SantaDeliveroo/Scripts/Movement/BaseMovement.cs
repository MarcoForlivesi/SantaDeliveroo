using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float acceleration;
    [SerializeField] private float rotationThreshold = 0.1f;
    [SerializeField] private float pointThreshold = 0.5f;
    [SerializeField] private float pointThresholdDeceleration = 1.0f;

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
        bool shouldRotate = Vector3.Angle(direction, transform.forward) > rotationThreshold;

        if (shouldRotate)
        {
            velocity = 0;

            Quaternion newRotation = Quaternion.LookRotation(pointTarget - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, rotationSpeed);
        }
        else
        {
            if (Vector3.Distance(transform.position, pointTarget) > pointThresholdDeceleration)
            {
                velocity += acceleration * Time.deltaTime;
                velocity = Mathf.Min(velocity, maxVelocity);
            }
            else
            {
                velocity -= acceleration * Time.deltaTime;
                velocity = Mathf.Max(velocity, 0);
            }

            //rigidbody.Add(pathLine.GetPosition(1));
            transform.position = Vector3.MoveTowards(transform.position, pointTarget, velocity);
        }
    }
}
