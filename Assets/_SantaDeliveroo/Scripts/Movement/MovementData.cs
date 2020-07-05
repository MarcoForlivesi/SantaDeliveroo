using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementData : MonoBehaviour
{
    public float rotationSpeed;
    public float maxVelocity;
    public float acceleration;
    public float rotationThreshold = 0.1f;
    public float pointThreshold = 0.5f;
    public float pointThresholdDeceleration = 1.0f;
}
