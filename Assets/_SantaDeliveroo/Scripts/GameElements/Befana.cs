using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Befana : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private bool pingPong;
    [Header("Movement")]
    [SerializeField] private Chase chase;
    [SerializeField] private PathFollower pathFollower;
    [Header("Trigger")]
    [SerializeField] private OnTriggerAction influeceTrigger;
    [SerializeField] private OnTriggerAction triggerAction;

    private List<Vector3> path;
    private bool reverse;

    private void Start()
    {
        reverse = true;

        path = new List<Vector3>();
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            path.Add(lineRenderer.GetPosition(i));
        }

        pathFollower.onPathComplete += OnPathComplete;
        lineRenderer.gameObject.SetActive(false);
        influeceTrigger.onTriggerEnter += OnInfluenceEnter;
        influeceTrigger.onTriggerExit += OnInfluenceExit;
        triggerAction.onTriggerEnter += OnTriggerEnterAction;
    }

    public void StartPatrolling()
    {
        StartPatrollingPath(false);
    }

    private void StartPatrollingPath(bool reverse)
    {
        List<Vector3> points = new List<Vector3>();
        if (reverse)
        {
            for (int i = path.Count - 1; i >= 0; i--)
            {
                points.Add(path[i] - transform.position);
            }
        }
        else
        {
            for (int i = 0; i < path.Count; i++)
            {
                points.Add(path[i] - transform.position);
            }
        }
        
        pathFollower.SetPath(points);
    }

    private void OnPathComplete()
    {
        reverse = pingPong && reverse;
        StartPatrollingPath(reverse);
        reverse = !reverse;
    }

    private void OnInfluenceEnter(Collider other)
    {
        SantaUnit santaUnit = other.GetComponent<SantaUnit>();
        if (santaUnit == null)
        {
            return;
        }

        pathFollower.enabled = false;
        chase.enabled = true;
        chase.SetTarget(santaUnit.transform);
    }

    private void OnInfluenceExit(Collider other)
    {
        SantaUnit santaUnit = other.GetComponent<SantaUnit>();
        if (santaUnit ==  null)
        {
            return;
        }

        pathFollower.enabled = true;
        chase.enabled = false;
        chase.SetTarget(null);
    }

    private void OnTriggerEnterAction(Collider other)
    {
        SantaUnit santaUnit = other.GetComponent<SantaUnit>();
        if (santaUnit == null)
        {
            return;
        }

        List<Vector3> path = new List<Vector3>();
        Vector3 honolulu = GameController.Instance.Honolulu.position;
        path.Add(honolulu);
        pathFollower.SetPath(path);

        santaUnit.Kidnapped();

        //Destroy(gameObject);
        //Destroy(santaUnit.gameObject);
    }
}
