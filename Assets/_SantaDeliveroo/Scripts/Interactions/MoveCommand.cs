using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCommand : MonoBehaviour, IMouseHandler
{
    public event System.Action<List<Vector3>> onMoveSelected;
    static public MoveCommand Instance => instance;

    private List<Vector3> pathList;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxDistance = 1.000f;
    [SerializeField] private float minCircleSize = 300.0f;
    [SerializeField] private float circleScaleFactor = 1.0f;
    [SerializeField] private Image horizontalCircleImage;
    [SerializeField] private Image verticalDistanceImage;
    [SerializeField] private LineRenderer pathLine;

    static private MoveCommand instance;

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
    private Vector3 lastVerticalPosition;
    private float moveDistance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        pathList = new List<Vector3>();
        ClearSelection();

        horizontalCircleCollider = horizontalCircleImage.GetComponent<Collider>();
        verticalDistanceCollider = verticalDistanceImage.GetComponent<Collider>();

        SelectionManager.Instance.onSelectionChange += () =>
        {
            Transform targetItem = GetTargetItem();

            if (targetItem == null)
            {
                return;
            }

            SetStartPosition(targetItem.position);
        };
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
            //    StartDrawingVerticalDistance();
            //    break;
            //case DrawStep.DrawingVerticalDistance:
                MoveSelected();
                break;
            default:
                break;
        }
    }

    private void SetStartPosition(Vector3 position)
    {
        startPosition = position;
    }

    private void StartDrawingCircle()
    {
        lastPosition = startPosition;

        horizontalCircleImage.transform.position = startPosition;
        horizontalCircleImage.transform.localScale = new Vector3(minCircleSize, minCircleSize, 1);
        horizontalCircleCollider.enabled = true;
        horizontalCircleImage.gameObject.SetActive(true);

        drawStep = DrawStep.DrawingHorizontalCircle;

        if (pathLine.positionCount == 0)
        {
            Utils.AddPosition(pathLine, startPosition);
        }
        Utils.AddPosition(pathLine, lastPosition);
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
    }

    private void UpdateData()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = startPosition.z;
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
        pathLine.SetPosition(pathLine.positionCount - 1, lastPosition);
    }

    private void UpdateVerticalDistance()
    {
        Vector3 circleVector = lastHorizontalPosition - startPosition;
        Vector3 veticalVector = lastPosition - startPosition;

        float angle = Vector3.SignedAngle(circleVector, veticalVector, Vector3.right);
        float angleAbs = Mathf.Abs(angle);
        float angle90Abs = (angleAbs < 90.0f ? angleAbs : 180.0f - angleAbs);

        bool clockwise = angle < 0;
        clockwise = angleAbs < 90 ? clockwise: !clockwise; 

        verticalDistanceImage.fillClockwise = clockwise;
        verticalDistanceImage.fillOrigin = angleAbs < 90 ? 1 : 3;
        verticalDistanceImage.fillAmount = angle90Abs / 360.0f;
        Debug.Log($"angle: { angle } angleAbs: { angleAbs } angle90Abs: {angle90Abs} fillAmount: { verticalDistanceImage.fillAmount }");

        float lenght = Mathf.Tan(angle90Abs * Mathf.Deg2Rad) * circleVector.magnitude;

        lastVerticalPosition = startPosition + veticalVector.normalized * lenght;
        Debug.Log($"veticalVector: { veticalVector } lenght: { lenght } veticalVector.normalized: { veticalVector.normalized } lastVerticalPosition: { lastVerticalPosition } lastPosition: { lastPosition }");

        pathLine.SetPosition(pathLine.positionCount - 1, lastVerticalPosition);
    }

    private void ClearSelection()
    {
        horizontalCircleImage.gameObject.SetActive(false);
        verticalDistanceImage.gameObject.SetActive(false);
        pathLine.gameObject.SetActive(false);
        pathLine.positionCount = 0;
        pathList.Clear();
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
        pathList.Add(lastPosition - startPosition);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            SetStartPosition(lastPosition);
            StartDrawingCircle();
        }
        else
        {
            if (onMoveSelected != null)
            {
                onMoveSelected.Invoke(pathList);
            }

            ClearSelection();
        }
    }


    
}
