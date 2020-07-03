using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCommand : MonoBehaviour, IMouseHandler
{
    public event System.Action onMoveSelected;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxDistance = 1.000f;
    [SerializeField] private float minCircleSize = 300.0f;
    [SerializeField] private float circleScaleFactor = 1.0f;
    [SerializeField] private Image horizontalCircleImage;
    [SerializeField] private Image verticalDistanceImage;
    [SerializeField] private LineRenderer pathLine;

    private Collider horizontalCircleCollider;
    private Collider verticalDistanceCollider;

    private enum DrawStep
    {
        Idle,
        DrawingHorizontalCircle,
        DrawingVerticalDistance,
    }

    private DrawStep drawStep;
    private Vector3 startPosition;
    private Vector3 lastPosition;
    private Vector3 lastHorizontalPosition;
    private float moveDistance;

    private void Start()
    {
        ClearSelection();

        horizontalCircleCollider = horizontalCircleImage.GetComponent<Collider>();
        verticalDistanceCollider = verticalDistanceImage.GetComponent<Collider>();

        SelectionManager.Instance.onSelectionChange += () => InputManager.Instance.SetCurrentHandler(this);
    }

    private void Update()
    {
        switch(drawStep)
        {
            case DrawStep.DrawingHorizontalCircle:
                UpdateData();
                UpdateHorizontalCircle();
                break;
            case DrawStep.DrawingVerticalDistance:
                UpdateData();
                UpdateVerticalDistance();
                break;
            default:
                break;
        }

    }

    public void OnMouseLeftClickDown()
    {
        switch (drawStep)
        {
            case DrawStep.Idle:
                StartDrawingCircle();
                break;
            case DrawStep.DrawingHorizontalCircle:
                StartDrawingVerticalDistance();
                break;
            case DrawStep.DrawingVerticalDistance:
                MoveSelected();
                break;
            default:
                break;
        }
    }

    private void StartDrawingCircle()
    {
        Transform targetItem = GetTargetItem();

        if (targetItem == null)
        {
            return;
        }

        startPosition = targetItem.position;
        lastPosition = startPosition;

        horizontalCircleImage.transform.position = startPosition;
        horizontalCircleImage.transform.localScale = new Vector3(minCircleSize, minCircleSize, 1);
        horizontalCircleCollider.enabled = true;
        horizontalCircleImage.gameObject.SetActive(true);

        drawStep = DrawStep.DrawingHorizontalCircle;
    }

    private void StartDrawingVerticalDistance()
    {
        drawStep = DrawStep.DrawingVerticalDistance;

        lastHorizontalPosition = lastPosition;
        horizontalCircleCollider.enabled = false;
        verticalDistanceImage.transform.position = startPosition;
        verticalDistanceImage.transform.localScale = new Vector3(moveDistance, moveDistance, 1);
        verticalDistanceImage.gameObject.SetActive(true);

        Vector3 circleVector = lastHorizontalPosition - startPosition;
        float angle = Vector3.SignedAngle(circleVector, Camera.main.transform.forward, Vector3.up);
        verticalDistanceImage.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.up);

        pathLine.positionCount = 1;
        pathLine.SetPosition(0, startPosition);
    }

    private void UpdateData()
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
            moveDistance = Vector3.Distance(startPosition, lastPosition);
            //Debug.Log($"collide: { hitInfo.collider.name } moveDistance: { moveDistance }");

            moveDistance = moveDistance * circleScaleFactor;
            moveDistance = Mathf.Max(moveDistance, minCircleSize);
        }
    }

    private void UpdateHorizontalCircle()
    {
        horizontalCircleImage.transform.localScale = new Vector3(moveDistance, moveDistance, 1);
        pathLine.positionCount = 2;
        pathLine.SetPosition(0, startPosition);
        pathLine.SetPosition(1, lastPosition);
    }

    private void UpdateVerticalDistance()
    {
        Vector3 circleVector = lastHorizontalPosition - startPosition;
        Vector3 veticalVector = lastPosition - startPosition;

        float angle = Vector3.SignedAngle(circleVector, veticalVector, Vector3.right);
        Debug.Log($"angle: { angle }");

        verticalDistanceImage.fillClockwise = Mathf.Sign(angle) < 0;
        verticalDistanceImage.fillAmount = Mathf.Abs(angle) / 360.0f;
        verticalDistanceImage.fillOrigin = angle > 0 ? 1 : 3;

        pathLine.positionCount = 2;
        pathLine.SetPosition(0, startPosition);
        pathLine.SetPosition(1, lastPosition);
    }

    private void ClearSelection()
    {
        horizontalCircleImage.gameObject.SetActive(false);
        verticalDistanceImage.gameObject.SetActive(false);
        drawStep = DrawStep.Idle;
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

    public void OnMouseLeftClick()
    {
        
    }

    public void OnMouseLeftClickUp()
    {
    }

    private void MoveSelected()
    {
        if (onMoveSelected != null)
        {
            onMoveSelected.Invoke();
        }
    }
}
