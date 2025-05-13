using UnityEngine;

namespace CameraSystem
{
    public class CameraResizer: MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private float referenceWidth = 1080f;
       
        [SerializeField]
        private float referenceHeight = 1920f;
       
        [SerializeField]
        private float referenceOrthographicSize = 12f;

        private void Awake()
        {
            ResizeCamera();
        }

        private void ResizeCamera()
        {
            if (mainCamera == null) return;

            float referenceAspect = referenceWidth / referenceHeight;
            float currentAspect = (float)Screen.width / Screen.height;
           
            float orthographicSizeAdjustment = referenceAspect / currentAspect;
           
            mainCamera.orthographicSize = referenceOrthographicSize * orthographicSizeAdjustment;
        }
    }
}