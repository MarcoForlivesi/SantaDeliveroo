using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gift : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        SantaUnit santaUnit = other.GetComponent<SantaUnit>();
        if (santaUnit != null)
        {
            santaUnit.CollectGift(this);
        }
    }
}
