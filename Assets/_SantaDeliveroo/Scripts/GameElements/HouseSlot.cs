using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSlot : MonoBehaviour, IPointInteractable
{
    public event System.Action<HouseSlot> onGiftDelivered;

    [SerializeField] private Transform hint;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private OnTriggerAction onDropAreaAction;
    [SerializeField] private OnTriggerAction onGroundAreaAction;

    private bool isDelivered;

    private void Start()
    {
        isDelivered = false;
        onDropAreaAction.onTriggerEnter += OnDropAreaEnter;
        onGroundAreaAction.onTriggerEnter += OnGroundAreaEnter;
    }

    public bool IsDelivered()
    {
        return isDelivered;
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
        isDelivered = true;

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
            santaUnit.DropGift();
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
}
