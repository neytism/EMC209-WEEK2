using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace GNW2.CamController
{
    public class PlayerCamera : NetworkBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -10);
        private Camera _mainCamera;
    
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (HasInputAuthority)
            {
                if (playerTransform != null)
                {
                    _mainCamera.transform.position = playerTransform.position + cameraOffset;
                    _mainCamera.transform.LookAt(playerTransform);
                }
            }
        }
    }

}


