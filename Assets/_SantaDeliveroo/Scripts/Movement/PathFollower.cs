using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private LineRenderer pathLine;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float acceleration;
    [SerializeField] private float pointThreshold = 0.5f;
    [SerializeField] private float pointThresholdDeceleration = 1.0f;

    private Rigidbody rigidbody;
    private float velocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (pathLine.positionCount < 2)
        {
            return;
        }

        Vector3 pointTarget = pathLine.GetPosition(1);

        bool shouldRotate = false;

        if (shouldRotate)
        {
            velocity = 0;
            Vector3 direction = pointTarget - transform.position;
            Quaternion newRotation = Quaternion.LookRotation(pointTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
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
            //rigidbody.MovePosition(pathLine.GetPosition(1));
            transform.position = Vector3.MoveTowards(transform.position, pointTarget, velocity);
        }

        if (Vector3.Distance(transform.position, pointTarget) < pointThreshold)
        {
            PointReached();
        }

        
        if (pathLine.positionCount > 0)
        {
            pathLine.SetPosition(0, transform.position);
        }
        
    }

    private void PointReached()
    {
        if (pathLine.positionCount < 3)
        {
            pathLine.positionCount = 0;
        }
        else
        {
            for (int i = 1; i < pathLine.positionCount - 1; i++)
            {
                pathLine.SetPosition(i, pathLine.GetPosition(i + 1));
            }
            pathLine.positionCount = pathLine.positionCount - 1;
        }
    }

    private void AddPathPoint(Vector3 point)
    {

        if (pathLine.positionCount < 1)
        {
            pathLine.positionCount = 2;
            pathLine.SetPosition(0, transform.position);
        }
        else
        {
            pathLine.positionCount = pathLine.positionCount + 1;
        }

        pathLine.SetPosition(pathLine.positionCount - 1, point);
    }

    public void SetPath(List<Vector3> pointList)
    {
        pathLine.positionCount = pointList.Count + 1;
        Vector3 start = transform.position;
        pathLine.SetPosition(0, start);

        for (int i = 0; i < pointList.Count; i++)
        {
            pathLine.SetPosition(i + 1, pointList[i] + start);
        }
    }

}
