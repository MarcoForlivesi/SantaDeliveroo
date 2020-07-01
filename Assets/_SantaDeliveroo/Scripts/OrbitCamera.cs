using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour, IMouseHandler
{
    [SerializeField] private Transform target;
    [Range(1f, 20f)]
    [SerializeField] float distance = 5f;
    [Range(1f, 20f)]
    [SerializeField] private float smoothSpeed = 1f;

    [SerializeField] private Vector2 rotationSpeed;

    private Vector2 lookPosition;
    private Vector2 orbitAngles;

    void LateUpdate()
    {
        Vector3 targetPosition = GetTargetPosition();
        UpdateZoom();

        Vector3 lerpPosition = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * smoothSpeed);
        Quaternion lookRotation = Quaternion.Euler(orbitAngles);

        transform.SetPositionAndRotation(lerpPosition, lookRotation);
        //Debug.Log($"target { targetPosition } current { transform.position }");
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 targetPosition = transform.position;

        Vector3 deltaInputMovement = GetDeltaInputMovemement();

        if (Utils.NearlyZero(deltaInputMovement) == false)
        {
            target = null;

            targetPosition += deltaInputMovement;
        }


        if (target != null)
        {
            targetPosition = target.position;
            Vector3 lookDirection = transform.forward;
            targetPosition = targetPosition - lookDirection * distance;
        }

        return targetPosition;
    }

    private Vector3 GetDeltaInputMovemement()
    {
        Vector3 deltaVector = Vector3.zero;
        float inputRight = Input.GetAxis("Right");
        float inputUp = Input.GetAxis("Up");
        float inputForward = Input.GetAxis("Forward");

        if (Utils.NearlyZero(inputRight) == false)
        {
            deltaVector += transform.right * inputRight * smoothSpeed;
        }
        if (Utils.NearlyZero(inputUp) == false)
        {
            deltaVector += transform.up * inputUp * smoothSpeed;
        }
        if (Utils.NearlyZero(inputForward) == false)
        {
            deltaVector += transform.forward * inputForward * smoothSpeed;
        }

        return deltaVector;
    }

    private void UpdateZoom()
    {
        float input = Input.GetAxis("Mouse ScrollWheel");
        distance = distance - input;
    }

    public void OnMouseLeftClickDown()
    {
    }

    public void OnMouseLeftClick()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X"));

        if (Utils.NearlyZero(input) == false)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
        }
    }

    public void OnMouseLeftClickUp()
    {
    }
}
