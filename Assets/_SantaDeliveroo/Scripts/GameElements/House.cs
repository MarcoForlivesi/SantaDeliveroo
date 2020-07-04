using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour, IPointInteractable, ISelectable
{
    public event System.Action<House> onGiftDelivered;

    [SerializeField] private Transform hint;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private OnTriggerAction onDropAreaAction;
    [SerializeField] private OnTriggerAction onGroundAreaAction;
    [Header("Gifts")]
    [SerializeField] private List<GiftType> giftRequests;

    private bool isSelected;

    private void Start()
    {
        onDropAreaAction.onTriggerEnter += OnDropAreaEnter;
        onGroundAreaAction.onTriggerEnter += OnGroundAreaEnter;
    }

    public bool IsDeliverComplete()
    {
        return giftRequests.Count == 0;
    }

    public Vector3 GetPoint()
    {
        return dropPosition.position;
    }

    private void OnDeliverComplete()
    {

    }

    private void GiftDelivered()
    {
        

        if (onGiftDelivered != null)
        {
            onGiftDelivered.Invoke(this);
        }
    }

    private void OnDropAreaEnter(Collider collider)
    {
        SantaUnit santaUnit = collider.GetComponent<SantaUnit>();
        if (santaUnit != null)
        {
            List<Gift> gifts = santaUnit.DropGift(giftRequests);

            foreach (Gift gift in gifts)
            {
                giftRequests.Remove(gift.Type);
            }
        }

        GiftDelivered();
    }

    private void OnGroundAreaEnter(Collider collider)
    {
        Gift gift = collider.GetComponent<Gift>();
        if (gift != null)
        {
            hint.gameObject.SetActive(false);
        }

        OnDeliverComplete();
    }

    public void Select(bool value)
    {
        isSelected = true;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public SelectableType GetSelectableType()
    {
        return SelectableType.House;
    }

    public List<GiftType> GetGiftTypes()
    {
        return giftRequests;
    }
}
