using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaUnit : MonoBehaviour, ISelectable
{
    [Header("Outline")]
    [SerializeField] private Outline outline;
    //[SerializeField] private Color defaultColor;
    //[SerializeField] private Color highlightColor;

    private bool isSelected;

    private void Start()
    {
    }

    public void Select(bool value)
    {
        isSelected = value;

        UpdateSelectionView();
    }

    private void UpdateSelectionView()
    {
        outline.enabled = isSelected;
        //MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        //if (isSelected)
        //{
        //    meshRenderer.material.SetColor("_EmissionColor", highlightColor);
        //}
        //else
        //{
        //    meshRenderer.material.SetColor("_EmissionColor", defaultColor);
        //}
    }
}
