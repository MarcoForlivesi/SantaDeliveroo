using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaUnit : MonoBehaviour, ISelectable
{
    public event System.Action<SantaUnit, Gift> onGiftsCollected;
    public event System.Action<List<Gift>> onGiftsDelivered;
    public event System.Action<SantaUnit> onKidnapped;

    public PathFollower PathFollower => pathFollower;

    [Header("Outline")]
    [SerializeField] private Outline outline;
    [SerializeField] private Transform[] slots;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float dropTime = 0.5f;
    //[SerializeField] private Color defaultColor;
    //[SerializeField] private Color highlightColor;

    private bool isSelected;
    private bool isKidnapped;
    private bool isSelectable;
    private PathFollower pathFollower;
    private List<Gift> collectedGifts;

    private void Start()
    {
        isSelectable = true;
        pathFollower = GetComponent<PathFollower>();
        collectedGifts = new List<Gift>();
    }

    public void Select(bool value)
    {
        if (isSelectable == false)
        {
            isSelected = false;
            return;
        }

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
                collectedGifts.Add(gift);

                if (onGiftsCollected != null)
                {
                    onGiftsCollected.Invoke(this, gift);
                }

                return;
            }
        }
    }

    public List<Gift> DropGift(List<GiftType> giftRequest)
    {
        List<Gift> giftsFulfilled = new List<Gift>();

        foreach (Transform slot in slots)
        {
            if (slot.childCount > 0)
            {
                Gift gift = slot.transform.GetChild(0).GetComponent<Gift>();

                if (giftRequest.Contains(gift.Type))
                {
                    collectedGifts.Remove(gift);
                    giftsFulfilled.Add(gift);
                }
            }
        }

        if (onGiftsDelivered != null)
        {
            onGiftsDelivered.Invoke(giftsFulfilled);
        }

        StartCoroutine(GiftDropCoroutine(giftsFulfilled));

        return giftsFulfilled;
    }

    private IEnumerator GiftDropCoroutine(List<Gift> giftsFulfilled)
    {
        foreach (Gift gift in giftsFulfilled)
        {
            gift.Drop();
            gift.transform.position = dropPoint.position;

            yield return new WaitForSeconds(dropTime);
        }

        isSelectable = true;
    }

    public void Kidnapped(Befana befana)
    {
        isKidnapped = true;
        Select(false);
        List<Vector3> path = new List<Vector3>();

        if (onKidnapped != null)
        {
            onKidnapped.Invoke(this);
        }

        Chase chase = gameObject.AddComponent<Chase>();
        chase.movementData = pathFollower.movementData;
        pathFollower.enabled = false;
        pathFollower.SetPath(new List<Vector3>());
        chase.SetTarget(befana.transform);
    }

    public bool IsKidnapped()
    {
        return isKidnapped;
    }

    public SelectableType GetSelectableType()
    {
        return SelectableType.Santa;
    }

    List<GiftType> ISelectable.GetGiftTypes()
    {
        List<GiftType> giftTypes = new List<GiftType>();

        foreach(Gift gift in collectedGifts)
        {
            giftTypes.Add(gift.Type);
        }

        return giftTypes;
    }
}
