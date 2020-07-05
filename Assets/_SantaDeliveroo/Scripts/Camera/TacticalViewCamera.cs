using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalViewCamera : MonoBehaviour
{
    public Camera Camera => cameraReference;
    [SerializeField] private Camera cameraReference;
    

    private void Update()
    {
        
    }
}
