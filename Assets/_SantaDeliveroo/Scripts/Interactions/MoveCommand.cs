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
        Idle,
        DrawingHorizontalCircle,
        DrawingVerticalDistance,
    }

    private DrawStep drawStep;
    private Vector3 startPosition;
    private Vector3 circleCenter;
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
        float angle = Vector3.SignedAngle(circleVector, Camera.main.transform.forward, Vector3.up);
        verticalDistanceImage.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.up);
    }

    private void UpdateData()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = circleCenter.z;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

        if (hitInfos.Length == 0)
        {
            return;
        }

        //Debug.Log($"Hit count: { hitInfos.Length } ");

        Vector3 targetPoint = hitInfos[0].point;
        foreach (RaycastHit hitInfo in hitInfos)
        {
            //Debug.Log($"collide: { hitInfo.collider.name } ");

            if (hitInfo.collider.tag == Tags.Interactable)
            {
                IPointInteractable pointInteractable = hitInfo.collider.GetComponentInParent<IPointInteractable>();

                if (pointInteractable != null)
                {
                    targetPoint = pointInteractable.GetPoint();
                    break;
                }
            }
        }

        pathLine.gameObject.SetActive(true);
        lastPosition = targetPoint;
        moveDistance = Vector3.Distance(circleCenter, lastPosition);
        //Debug.Log($"collide: { hitInfo.collider.name } moveDistance: { moveDistance }");

        moveDistance = moveDistance * circleScaleFactor;
        moveDistance = Mathf.Max(moveDistance, minCircleSize);
    }

    private void UpdateHorizontalCircle()
    {
        horizontalCircleImage.transform.localScale = new Vector3(moveDistance, moveDistance, 1);
        lastHorizontalPosition = lastPosition;
        pathLine.SetPosition(pathLine.positionCount - 1, lastPosition);

        //Debug.Log($"Horizontal Click on: { lastPosition }");
    }

    private void UpdateVerticalDistance()
    {
        Vector3 circleVector = lastHorizontalPosition - circleCenter;
        Vector3 veticalVector = lastPosition - circleCenter;

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

        lastVerticalPosition = circleCenter + veticalVector.normalized * lenght;
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
