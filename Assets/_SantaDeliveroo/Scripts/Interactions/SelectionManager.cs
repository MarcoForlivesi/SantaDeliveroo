using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : MonoBehaviour, IMouseHandler
{
    public event System.Action onSelectionChange;

    static public SelectionManager Instance => instance;
    public  List<ISelectable> CurrentSelection => currentSelection;

    [SerializeField] private LayerMask raycastLayerMask;
    [SerializeField] private float raycastMaxDistance;

    static private SelectionManager instance;
    private List<ISelectable> currentSelection;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        currentSelection = new List<ISelectable>();
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

    public void Deselect(ISelectable selectable)
    {
        List<ISelectable> lastSelection = new List<ISelectable>(currentSelection);

        if (selectable != null)
        {
            currentSelection.Remove(selectable);
            selectable.Select(false);
        }

        if (Utils.Equals(currentSelection, lastSelection) == false)
        {
            SelectionChange();
        }
    }

    public void OnMouseLeftClickDown()
    {
        List<ISelectable> lastSelection = new List<ISelectable>(currentSelection);
        if (Input.GetKey(KeyCode.LeftControl) == false)
        {
            _DeselectAll();
        }

        Ray raycastVector = Camera.main.ScreenPointToRay(Input.mousePosition);

        List<ISelectable> selectableHits = new List<ISelectable>();
        RaycastHit[] raycastHits = Physics.RaycastAll(raycastVector, raycastMaxDistance, raycastLayerMask);
        foreach(RaycastHit raycastHit in raycastHits)
        {
            ISelectable selectable = GetSelectableItem(raycastHit.collider.transform);

            if (selectable != null)
            {
                SelectableType currentType = GetSelectionType();

                if (currentType == SelectableType.None || selectable.GetSelectableType() == currentType)
                {
                    selectable.Select(true);

                    selectableHits.Add(selectable);
                }
            }
        }

        if (selectableHits.Count > 0)
        {
            ISelectable selectable = selectableHits.OrderBy((item) => item.GetPriority()).First();
            currentSelection.Add(selectable);
        }

        if (Input.GetKey(KeyCode.LeftControl) == false)
        {
            if (Utils.Equals(currentSelection, lastSelection) == false)
            {
                SelectionChange();
            }
        }
    }

    private ISelectable GetSelectableItem(Transform transform)
    {
        if (transform == null)
        {
            return null;
        }

        ISelectable selectable = transform.GetComponent<ISelectable>();
        if (selectable != null)
        {
            return selectable;
        }

        return GetSelectableItem(transform.parent);
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
            ISelectable selectable = currentSelection[0];
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

        foreach (ISelectable selectable in currentSelection)
        {
            center += selectable.GetTransform().position;
        }

        center = center / currentSelection.Count;

        Transform centerItem = null;
        float minDistance = -1;

        foreach (ISelectable selectable in currentSelection)
        {
            Transform item = selectable.GetTransform();

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
