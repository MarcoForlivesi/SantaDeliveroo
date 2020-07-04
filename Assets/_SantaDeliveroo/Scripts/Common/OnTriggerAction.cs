
using UnityEngine;

public class OnTriggerAction : MonoBehaviour
{
    public System.Action<Collider> onTriggerEnter;
    public System.Action<Collider> onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }
}
