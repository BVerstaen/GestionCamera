using UnityEngine;

public class FixedFollowView : AView
{
    public float roll;
    public float fov = 80;

    [Space(7)]
    public Vector3 target;
    public Transform targetTransform;

    private void Reset()
    {
        if (CameraController.Instance != null)
        {
            targetTransform = CameraController.Instance.transform;
        }
    }

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration cameraConfiguration = new CameraConfiguration();

        if (targetTransform != null)
        {
            target = targetTransform.position;
        }

        Vector3 dir = (target - transform.position).normalized;

        cameraConfiguration.yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        cameraConfiguration.pitch = -Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        cameraConfiguration.roll = roll;

        cameraConfiguration.fieldOfView = fov;

        cameraConfiguration.pivot = transform.position;
        cameraConfiguration.distance = 0;

        cameraConfiguration.OnClampPitch();
        if (!CameraController.Instance.fullRollRotation)
            cameraConfiguration.OnClampRoll();

        return cameraConfiguration;
    }
}
