using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCommand : MonoBehaviour, IMouseHandler
{

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxDistance = 1.000f;
    [SerializeField] private float minCircleSize = 300.0f;
    [SerializeField] private float circleScaleFactor = 1.0f;
    [SerializeField] private Image moveCircleImage;
    [SerializeField] private LineRenderer pathLine;

    private enum DrawStep
    {
        Idle,
        DrawingCircle
    }

    private DrawStep drawStep;
    private Vector3 startPosition;
    private Vector3 lastPosition;
    private float cameraDistance;

    private void Start()
    {
        moveCircleImage.gameObject.SetActive(false);
        drawStep = DrawStep.Idle;
    }

    private void StartDrawing()
    {
        Transform targetItem = GetTargetItem();

        if (targetItem == null)
        {
            return;
        }

        startPosition = targetItem.position;
        lastPosition = startPosition;

        moveCircleImage.transform.position = startPosition;
        moveCircleImage.transform.localScale = new Vector3(minCircleSize, minCircleSize, 1);

        moveCircleImage.gameObject.SetActive(true);

        drawStep = DrawStep.DrawingCircle;
    }

    private void UpdateDrawing()
    {
        Transform targetItem = GetTargetItem();
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = targetItem.position.z;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, layerMask);
        if (hit)
        {
            pathLine.gameObject.SetActive(true);
            lastPosition = hitInfo.point;
            float moveDistance = Vector3.Distance(startPosition, lastPosition);
            Debug.Log($"collide: { hitInfo.collider.name } moveDistance: { moveDistance }");

            moveDistance = moveDistance * circleScaleFactor;
            moveDistance = Mathf.Max(moveDistance, minCircleSize);

            moveCircleImage.transform.localScale = new Vector3(moveDistance, moveDistance, 1);
            pathLine.positionCount = 2;
            pathLine.SetPosition(0, startPosition);
            pathLine.SetPosition(1, lastPosition);
        }
    }

    private Transform GetTargetItem()
    {
        List<Transform> selectionList = SelectionManager.Instance.CurrentSelection;

        if (selectionList == null || selectionList.Count == 0)
        {
            return null;
        }

        return selectionList[0];
    }

    public void OnMouseLeftClickDown()
    {
    }

    public void OnMouseLeftClick()
    {
        if (drawStep == DrawStep.Idle)
        {
            StartDrawing();
        }
        else
        {
            UpdateDrawing();
        }
    }

    public void OnMouseLeftClickUp()
    {
    }
}
