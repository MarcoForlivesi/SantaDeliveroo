using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gift : MonoBehaviour, IPointInteractable, ISelectable
{
    public GiftType Type => giftType;

    [SerializeField] private GiftType giftType;
    [SerializeField] private Transform hint;

    private Rigidbody rigidbody;
    private bool select;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void SetCollected()
    {
        hint.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        SantaUnit santaUnit = other.GetComponent<SantaUnit>();
        if (santaUnit != null)
        {
            santaUnit.CollectGift(this);
        }
    }

    public void Drop()
    {
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }

    public void Delivered()
    {
        //rigidbody.isKinematic = true;
        //rigidbody.useGravity = false;
        Destroy(gameObject);
    }


    public Vector3 GetPoint()
    {
        return transform.position;
    }

    public void Select(bool value)
    {
        select = value;
    }

    public SelectableType GetSelectableType()
    {
        return SelectableType.Gift;
    }

    public List<GiftType> GetGiftTypes()
    {
        List<GiftType> list = new List<GiftType>();
        list.Add(giftType);
        return list;
    }

    public byte GetPriority()
    {
        return 2;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
