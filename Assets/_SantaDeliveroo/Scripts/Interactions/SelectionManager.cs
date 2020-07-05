using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour, IMouseHandler
{
    public event System.Action onSelectionChange;

    static public SelectionManager Instance => instance;
    public  List<Transform> CurrentSelection => currentSelection;

    [SerializeField] private LayerMask raycastLayerMask;
    [SerializeField] private float raycastMaxDistance;

    static private SelectionManager instance;
    private List<Transform> currentSelection;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        currentSelection = new List<Transform>();
    }

    private void _DeselectAll()
    {
        currentSelection.Clear();
        List<SantaUnit> santaUnits = GameController.Instance.SantaUnits;

        foreach (SantaUnit santaUnit in santaUnits)
        {
            santaUnit.Select(false);
        }
    }

    public void DeselectAll()
    {
        _DeselectAll();
        SelectionChange();
    }

    public void Deselect(Transform selectableTransform)
    {
        List<Transform> lastSelection = new List<Transform>(currentSelection);

        ISelectable selectable = selectableTransform.GetComponent<ISelectable>();
        if (selectable != null)
        {
            currentSelection.Remove(selectableTransform);
            selectable.Select(false);
        }

        if (Utils.Equals(currentSelection, lastSelection) == false)
        {
            SelectionChange();
        }
    }

    public void OnMouseLeftClickDown()
    {
        List<Transform> lastSelection = new List<Transform>(currentSelection);
        if (Input.GetKey(KeyCode.LeftControl) == false)
        {
            _DeselectAll();
        }

        Ray raycastVector = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(raycastVector, out RaycastHit raycastHit, raycastMaxDistance, raycastLayerMask);
        if (hit)
        {
            Transform selectableTransform = GetSelectableTransform(raycastHit.collider.transform);

            if (selectableTransform != null)
            {
                ISelectable selectable = selectableTransform.GetComponentInParent<ISelectable>();

                if (selectable != null)
                {
                    SelectableType currentType = GetSelectionType();

                    if (currentType == SelectableType.None || selectable.GetSelectableType() == currentType)
                    {
                        selectable.Select(true);

                        currentSelection.Add(selectableTransform);
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) == false)
        {
            if (Utils.Equals(currentSelection, lastSelection) == false)
            {
                SelectionChange();
            }
        }
    }

    private Transform GetSelectableTransform(Transform transform)
    {
        if (transform == null)
        {
            return null;
        }

        ISelectable selectable = transform.GetComponent<ISelectable>();
        if (selectable != null)
        {
            return transform;
        }

        return GetSelectableTransform(transform.parent);
    }

    public void OnMouseLeftClick()
    {
    }

    public void OnMouseLeftClickUp()
    {
    }

    public SelectableType GetSelectionType()
    {
        if (currentSelection.Count > 0)
        {
            ISelectable selectable = currentSelection[0].GetComponent<ISelectable>();
            return selectable.GetSelectableType();
        }

        return SelectableType.None;
    }

    private void SelectionChange()
    {
        if (onSelectionChange != null)
        {
            onSelectionChange.Invoke();
        }
    }

    public Transform GetTargetItem()
    {
        Vector3 center = Vector3.zero;

        if (currentSelection == null || currentSelection.Count == 0)
        {
            return null;
        }

        foreach (Transform item in currentSelection)
        {
            center += item.position;
        }

        center = center / currentSelection.Count;

        Transform centerItem = null;
        float minDistance = -1;

        foreach (Transform item in currentSelection)
        {
            if (centerItem == null)
            {
                minDistance = Vector3.Distance(center, item.position);
                centerItem = item;
            }
            else
            {
                float distance = Vector3.Distance(center, item.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    centerItem = item;
                }
            }
        }

        return centerItem;
    }
}
