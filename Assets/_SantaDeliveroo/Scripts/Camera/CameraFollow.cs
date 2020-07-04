using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    static public CameraFollow Instance => instance;

    private static CameraFollow instance;

    [SerializeField] private float speed;

    private Transform targetItem;
    private Vector3 prevPosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        SelectionManager.Instance.onSelectionChange += OnSelectionChange;
        prevPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (targetItem == null)
        {
            return;
        }

        Vector3 moveVector = targetItem.position - prevPosition;
        transform.position = Vector3.Slerp(transform.position, moveVector, Time.deltaTime * speed);
        prevPosition = transform.position;
        Vector3 distanceVector = targetItem.position - transform.position;
    }

    public void OnSelectionChange()
    {
        Transform targetItem = SelectionManager.Instance.GetTargetItem();
        this.targetItem = targetItem;
    }
}
