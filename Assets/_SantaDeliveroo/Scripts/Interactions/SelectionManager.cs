using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour, IMouseHandler
{
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

    private void DeselectAll()
    {
        currentSelection.Clear();
        List<SantaUnit> santaUnits = GameController.Instance.SantaUnits;

        foreach (SantaUnit santaUnit in santaUnits)
        {
            santaUnit.Select(false);
        }
    }

    public void OnMouseLeftClickDown()
    {
        DeselectAll();

        Ray raycastVector = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(raycastVector, out RaycastHit raycastHit, raycastMaxDistance, raycastLayerMask);
        if (hit)
        {
            ISelectable selectable = raycastHit.collider.GetComponent<ISelectable>();

            if (selectable != null)
            {
                selectable.Select(true);

                currentSelection.Add(raycastHit.collider.transform);
            }
        }
    }

    public void OnMouseLeftClick()
    {
    }

    public void OnMouseLeftClickUp()
    {
        if (currentSelection.Count > 0)
        {
            InputManager.Instance.SetMoveCommand();
        }
    }
}
