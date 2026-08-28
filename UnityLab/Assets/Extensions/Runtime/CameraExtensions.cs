using UnityEngine;

namespace Kyrnel
{
    public static class CameraExtensions
    {
        public static bool IsOutsideCameraViewport(this Camera camera, Vector3 worldPos)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(worldPos);
            if (viewportPoint.z < 0f || viewportPoint.z > camera.farClipPlane)
                return true;
            return viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1;
        }
    }
}