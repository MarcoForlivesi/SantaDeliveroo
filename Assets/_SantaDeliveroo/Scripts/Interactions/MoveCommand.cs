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
        Idle = 0,
        DrawingHorizontalCircle = 1,
        DrawingVerticalDistance = 2,
        DrawToHotspot = 4,
    }

    private DrawStep drawStep;
    private Vector3 startPosition;
    private Vector3 circleCenter;
    private Vector3 lastMousePos;
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
    }

    private void Update()
    {
        if (Vector3.Distance(lastMousePos, Input.mousePosition) < Mathf.Epsilon)
        {
            return;
        }
        lastMousePos = Input.mousePosition;

        switch (drawStep)
        {
            case DrawStep.DrawingHorizontalCircle:
                UpdateData();
                UpdateHorizontalCircle();
                break;
            case DrawStep.DrawingVerticalDistance:
                UpdateData();
                UpdateVerticalDistance();
                break;
            case DrawStep.DrawToHotspot:
                UpdateData();
                DrawToHotspot();
                break;
            default:
                break;
        }

    }

    public void OnMouseLeftClickDown()
    {
        switch (drawStep)
        {
            case DrawStep.DrawingHorizontalCircle:
                StartDrawingVerticalDistance();
                break;
            case DrawStep.DrawingVerticalDistance:
            case DrawStep.DrawToHotspot:
                MoveSelected();
                break;
            default:
                break;
        }
    }

    public void SetStartPosition(Vector3 position)
    {
        startPosition = position;
        StartDrawingCircle();
    }

    private void StartDrawingCircle()
    {
        if (pathLine.positionCount == 0)
        {
            Utils.AddPosition(pathLine, startPosition);
            circleCenter = startPosition;
        }

        lastPosition = circleCenter;

        horizontalCircleImage.transform.position = circleCenter;
        horizontalCircleImage.transform.localScale = new Vector3(minCircleSize, minCircleSize, 1);
        horizontalCircleCollider.enabled = true;
        horizontalCircleImage.gameObject.SetActive(true);

        verticalDistanceImage.gameObject.SetActive(false);

        drawStep = DrawStep.DrawingHorizontalCircle;

        Utils.AddPosition(pathLine, lastPosition);
    }

    private void StartDrawingVerticalDistance()
    {
        drawStep = DrawStep.DrawingVerticalDistance;

        lastHorizontalPosition = lastPosition;

        horizontalCircleCollider.enabled = false;
        verticalDistanceImage.transform.position = circleCenter;
        verticalDistanceImage.transform.localScale = new Vector3(moveDistance, moveDistance, 1);
        verticalDistanceImage.gameObject.SetActive(true);

        Vector3 circleVector = lastHorizontalPosition - circleCenter;
        float angle = Vector3.Angle(circleVector, Vector3.right) * Mathf.Sign(-circleVector.z);

        verticalDistanceImage.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }

    private void UpdateData()
    {
        bool found = FindTargetPoint(out Vector3 targetPoint);
        if (found == false)
        {
            return;
        }
        //Debug.Log($"targetPoint: { targetPoint }");

        pathLine.gameObject.SetActive(true);
        lastPosition = targetPoint;
        moveDistance = Vector3.Distance(circleCenter, lastPosition);
        //Debug.Log($"collide: { hitInfo.collider.name } moveDistance: { moveDistance }");

        moveDistance = moveDistance * circleScaleFactor;
        moveDistance = Mathf.Max(moveDistance, minCircleSize);
    }

    private bool FindTargetPoint(out Vector3 targetPoint)
    {
        Vector3 mousePosition = Input.mousePosition;
        //mousePosition.z = circleCenter.z;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

        if (hitInfos.Length == 0)
        {
            targetPoint = Vector3.zero;
            return false;
        }
        //Debug.Log($"Hit count: { hitInfos.Length } ");
        //Debug.Log($"circleCenter: { circleCenter }");

        bool chooseInteractable = false;
        if (drawStep != DrawStep.DrawingVerticalDistance)
        {
            chooseInteractable = true;
            drawStep = DrawStep.DrawingHorizontalCircle;
        }

        targetPoint = hitInfos[0].point;
        foreach (RaycastHit hitInfo in hitInfos)
        {
            //Debug.Log($"collide: { hitInfo.collider.name } point: { hitInfo.point }");
            if (chooseInteractable && hitInfo.collider.tag == Tags.Interactable)
            {
                IPointInteractable pointInteractable = hitInfo.collider.GetComponentInParent<IPointInteractable>();

                if (pointInteractable != null)
                {
                    targetPoint = pointInteractable.GetPoint();
                    drawStep = DrawStep.DrawToHotspot;

                    break;
                }
            }
            else
            {
                if (hitInfo.collider.tag != Tags.Interactable)
                {
                    targetPoint = hitInfo.point;
                }
            }
        }

        return true;
    }

    private void UpdateHorizontalCircle()
    {
        horizontalCircleImage.enabled = true;
        horizontalCircleImage.transform.localScale = new Vector3(moveDistance, moveDistance, 1);
        lastHorizontalPosition = lastPosition;
        pathLine.SetPosition(pathLine.positionCount - 1, lastPosition);

        //Debug.Log($"Horizontal Click on: { lastPosition }");
    }

    private void DrawToHotspot()
    {
        horizontalCircleImage.enabled = false;
        pathLine.SetPosition(pathLine.positionCount - 1, lastPosition);
    }

    private void UpdateVerticalDistance()
    {
        Vector3 circleVector = lastHorizontalPosition - circleCenter;
        Vector3 verticalVector = lastPosition - circleCenter;
        float angle = Vector3.Angle(circleVector, verticalVector) * Mathf.Sign(verticalVector.y);
        float angleAbs = Mathf.Abs(angle);
        float angle90Abs = (angleAbs < 90.0f ? angleAbs : 180.0f - angleAbs);

        //bool clockwise = angle < 0;
        bool clockwise = angle > 90.0f;
        //clockwise = angleAbs < 90 ? clockwise: !clockwise; 

        //verticalDistanceImage.fillClockwise = clockwise;
        verticalDistanceImage.fillOrigin = angleAbs < 90 ? 1 : 3;
        verticalDistanceImage.fillAmount = angle90Abs / 360.0f;
        //Debug.Log($"angle: { angle } angleAbs: { angleAbs } angle90Abs: {angle90Abs} fillAmount: { verticalDistanceImage.fillAmount }");

        float height = Mathf.Tan(angle * Mathf.Deg2Rad) * circleVector.magnitude;
        //Debug.Log($" veticalVector: { verticalVector } veticalVector.normalized: { verticalVector.normalized }");
        //Debug.Log($"height: { height } circleVector.magnitude: {circleVector.magnitude}");

        Vector3 lastVerticalPosition = lastHorizontalPosition + height * Vector3.up;
        if (angleAbs < 90)
        {
            lastVerticalPosition = lastHorizontalPosition + height * Vector3.up;
        }
        else
        {
            Vector3 opposite = circleCenter - lastHorizontalPosition;
            lastVerticalPosition =  circleCenter + opposite - height * Vector3.up;
        }

        Vector3 localScale = verticalDistanceImage.transform.localScale;
        localScale.y = height * circleScaleFactor;
        verticalDistanceImage.transform.localScale = localScale;

        //Debug.Log($"angle90Abs: {angle90Abs} tan: { Mathf.Tan(angle90Abs * Mathf.Deg2Rad) }");
        //Debug.Log($"circleCenter: { circleCenter }");
        //Debug.Log($"lastHorizontalPosition: { lastHorizontalPosition }");
        //Debug.Log($"lastVerticalPosition: { lastVerticalPosition } lastPosition: { lastPosition }");

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

    

    public void OnMouseLeftClick()
    {
        
    }

    public void OnMouseLeftClickUp()
    {
    }

    private void MoveSelected()
    {
        //Debug.Log($"Add point: { lastPosition } startPosition: { startPosition }");

        pathList.Add(lastPosition - startPosition);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            circleCenter = lastPosition;
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
