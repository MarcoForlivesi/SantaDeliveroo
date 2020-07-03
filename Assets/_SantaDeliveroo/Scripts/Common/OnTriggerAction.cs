
using UnityEngine;

public class OnTriggerAction : MonoBehaviour
{
    public System.Action<Collider> onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }
}
