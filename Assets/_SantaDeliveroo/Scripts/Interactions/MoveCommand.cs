using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCommand : MonoBehaviour, IMouseHandler
{
    [SerializeField] private float circleScaleFactor = 1.0f;
    [SerializeField] private Image moveCircleImage;

    private enum DrawStep
    {
        Idle,
        DrawingCircle
    }

    private DrawStep drawStep;
    private Vector3 startPosition;
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

        drawStep = DrawStep.DrawingCircle;
        startPosition = Input.mousePosition;
    }

    private void UpdateDrawing()
    {
        Transform targetItem = GetTargetItem();
        float cameraDistance = Vector3.Distance(targetItem.transform.position, Camera.main.transform.position);

        float moveDistance = Vector3.Distance(startPosition, Input.mousePosition) * cameraDistance;
        Debug.Log($"moveDistance: { moveDistance }");

        moveCircleImage.transform.localScale = new Vector3(moveDistance, moveDistance, moveDistance) * circleScaleFactor;
        moveCircleImage.gameObject.SetActive(true);
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
