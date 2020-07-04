using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaUnit : MonoBehaviour, ISelectable
{
    public PathFollower PathFollower => pathFollower;

    [Header("Outline")]
    [SerializeField] private Outline outline;
    [SerializeField] private Transform[] slots;
    [SerializeField] private Transform dropPoint;
    //[SerializeField] private Color defaultColor;
    //[SerializeField] private Color highlightColor;

    private bool isSelected;
    private PathFollower pathFollower;
    private List<Gift> collectedGifts;

    private void Start()
    {
        pathFollower = GetComponent<PathFollower>();
        collectedGifts = new List<Gift>();
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

    public void CollectGift(Gift gift)
    {
        foreach (Transform slot in slots)
        {
            if (slot.childCount == 0)
            {
                gift.SetCollected();
                gift.transform.SetParent(slot);
                gift.transform.position = slot.position;
                return;
            }
        }
    }

    public Gift DropGift()
    {
        foreach (Transform slot in slots)
        {
            if (slot.childCount > 0)
            {
                Gift gift = slot.transform.GetChild(0).GetComponent<Gift>();
                gift.Drop();
                gift.transform.position = dropPoint.position;
                return gift;
            }
        }

        return null;
    }
}
