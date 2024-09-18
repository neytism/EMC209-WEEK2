using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace GNW.CamController
{
    public class PlayerCamera : NetworkBehaviour
    {
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0, 10, -5);
        private Camera _mainCamera;

        [SerializeField] private float mouseSensitivity = 100f;
        private float _horizontalRotation = 0f;
        private float _verticalRotation = 0f; 

        private void Start()
        {
            _mainCamera = Camera.main;

            //Cursor.lockState = CursorLockMode.Locked; 
            //Cursor.visible = false; 
        }

        private void LateUpdate()
        {
            if (HasInputAuthority)
            {
                //HandleMouseLook(); 
                UpdateCameraPosition(); 
            }
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            
            _horizontalRotation += mouseX;
            _verticalRotation -= mouseY;
            
            _verticalRotation = Mathf.Clamp(_verticalRotation, -30f, 60f);
        }

        private void UpdateCameraPosition()
        {
            // if (cameraTarget != null)
            // {
            //     Quaternion rotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);
            //     Vector3 position = cameraTarget.position + rotation * cameraOffset;
            //
            //     _mainCamera.transform.position = position; 
            //     _mainCamera.transform.LookAt(cameraTarget.position + Vector3.up * 1.5f);
            //     
            //     cameraTarget.transform.rotation = Quaternion.Euler(0,_mainCamera.transform.rotation.y,0);
            // }
            
            if (cameraTarget != null)
            {
                _mainCamera.transform.position = cameraTarget.position + cameraOffset;
                _mainCamera.transform.LookAt(cameraTarget);
            }
        }
    }
}