using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gift : MonoBehaviour, IPointInteractable
{
    [SerializeField] private Transform hint;

    private Rigidbody rigidbody;

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
}
