using UnityEngine;

public class DolllyView : AView
{
    public float roll;
    public float distance;
    public float fov;

    public Transform target;

    public Rail rail;
    public float distanceOnRail;
    public float speed;

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration cameraConfiguration = new CameraConfiguration();

        //cameraConfiguration.yaw = yaw;
        //cameraConfiguration.pitch = pitch;
        cameraConfiguration.roll = roll;

        cameraConfiguration.fieldOfView = fov;
        cameraConfiguration.pivot = rail.GetPosition(distanceOnRail);
        cameraConfiguration.distance = 0;

        cameraConfiguration.OnClampPitch();
        if (CameraController.Instance.fullRollRotation)
            cameraConfiguration.OnClampRoll();

        return cameraConfiguration;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
            MoveDistanceOnRail(-1);
        else if (Input.GetKey(KeyCode.D))
            MoveDistanceOnRail(1);
    }

    public void MoveDistanceOnRail(int direction)
    {
        distanceOnRail += direction * speed * Time.deltaTime;
    }
}
