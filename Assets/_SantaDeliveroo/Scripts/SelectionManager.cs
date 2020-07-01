using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask raycastLayerMask;
    [SerializeField] private float raycastMaxDistance;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DeselectAll();

            Ray raycastVector = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(raycastVector, out RaycastHit raycastHit, raycastMaxDistance, raycastLayerMask);
            if (hit)
            {
                SantaUnit santaUnit = raycastHit.collider.GetComponent<SantaUnit>();

                if (santaUnit != null)
                {
                    santaUnit.Select(true);
                }
            }
        }
    }

    private void DeselectAll()
    {
        List<SantaUnit> santaUnits = GameController.Instance.SantaUnits;

        foreach (SantaUnit santaUnit in santaUnits)
        {
            santaUnit.Select(false);
        }
    }
}
