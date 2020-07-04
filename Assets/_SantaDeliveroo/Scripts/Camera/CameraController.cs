using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private OrbitCamera orbitCamera;
    [SerializeField] private TacticalViewCamera tacticalViewCamera;
    [SerializeField] private float switchTime = 1.5f;
    [SerializeField] private float positionThreshold = 0.01f;
    [SerializeField] private float rotationThreshold = 1.0f;

    private bool isTacticalView;
    private bool isSwitching;

    private Vector3 targetPosition;
    private Vector3 targetDirection;

    private Vector3 prevPosition;
    private Vector3 prevDirection;

    private void Start()
    {
        isTacticalView = false;
        isSwitching = false;
        orbitCamera.enabled = true;
        tacticalViewCamera.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTacticalView)
            {
                orbitCamera.enabled = true;
                tacticalViewCamera.enabled = false;

                targetPosition = prevPosition;
                targetDirection = prevDirection;
            }
            else
            {
                orbitCamera.enabled = false;
                tacticalViewCamera.enabled = true;

                targetPosition = tacticalViewCamera.transform.position;
                targetDirection = tacticalViewCamera.transform.forward;

                prevPosition = orbitCamera.transform.position;
                prevDirection = orbitCamera.transform.forward;
            }

            isSwitching = true;
            isTacticalView = !isTacticalView;
        }

        if (isSwitching)
        {
            SwitchCamera();
        }
    }


    private void SwitchCamera()
    {
        Camera cameraMain = Camera.main;
        cameraMain.transform.position = Vector3.Lerp(cameraMain.transform.position,
               targetPosition,
               switchTime * Time.deltaTime);

        cameraMain.transform.rotation = Quaternion.Slerp(cameraMain.transform.rotation,
            Quaternion.LookRotation(targetDirection),
            switchTime * Time.deltaTime);

        isSwitching = Vector3.Distance(cameraMain.transform.position, targetPosition) > positionThreshold ||
            Vector3.Angle(cameraMain.transform.forward, targetDirection) > rotationThreshold;
    }
}
