using UnityEngine;

namespace UIManager
{
    public class CanvasFaceCamera : MonoBehaviour
    {
        Transform mainCameraTransform;

        void Start()
        {
            mainCameraTransform = Camera.main.transform;
        }

        void LateUpdate()
        {
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
        }
    }
}