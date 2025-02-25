﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : BaseMovement
{
    public event System.Action onPathComplete;

    [SerializeField] private LineRenderer pathLine;

    private Rigidbody rigidbody;
    private float velocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pathLine.positionCount < 2)
        {
            return;
        }

        Vector3 pointTarget = pathLine.GetPosition(1);
        MoveTowards(pointTarget);

        if (Vector3.Distance(transform.position, pointTarget) < movementData.pointThreshold)
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
            OnPathComplete();
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

        //Debug.Log($"SetPath:");

        for (int i = 0; i < pointList.Count; i++)
        {
            pathLine.SetPosition(i + 1, pointList[i] + start);
            //Debug.Log($"{i} : { pointList[i] + start }");
        }
    }

    private void OnPathComplete()
    {
        if (onPathComplete != null)
        {
            onPathComplete.Invoke();
        }
    }
}
