using UnityEngine;

public class FixedView : AView
{
    public float yaw;
    public float pitch;
    public float roll;
    public float fov = 80;

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration cameraConfiguration = new CameraConfiguration();

        cameraConfiguration.yaw = yaw;
        cameraConfiguration.pitch = pitch;
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
