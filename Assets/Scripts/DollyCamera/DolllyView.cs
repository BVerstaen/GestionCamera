using UnityEngine;

public class DolllyView : AView
{
    public float roll;
    public float distance;
    public float fov;
    [Space]
    public bool IsAuto;
    public Transform target;
    [Space]
    public Rail rail;
    public float distanceOnRail;
    public float speed;

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration cameraConfiguration = new CameraConfiguration();

        Vector3 dir = (target.position - transform.position).normalized;
        cameraConfiguration.yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        cameraConfiguration.pitch = -Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        cameraConfiguration.roll = roll;

        cameraConfiguration.fieldOfView = fov;

        if (IsAuto)
            cameraConfiguration.pivot = rail.GetNearestPositionFromTarget(target.position);
        else
            cameraConfiguration.pivot = rail.GetPosition(distanceOnRail);

        cameraConfiguration.distance = 0;

        cameraConfiguration.OnClampPitch();
        if (CameraController.Instance.fullRollRotation)
            cameraConfiguration.OnClampRoll();

        return cameraConfiguration;
    }

    //TEST FOR RAIL
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
