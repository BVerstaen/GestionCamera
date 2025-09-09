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

        if (IsAuto)
            cameraConfiguration.pivot = rail.GetNearestPositionFromTarget(target.position);
        else
            cameraConfiguration.pivot = rail.GetPosition(distanceOnRail);

        Vector3 dir = (target.position - cameraConfiguration.pivot).normalized;
        cameraConfiguration.yaw = (Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
        cameraConfiguration.pitch = (-Mathf.Asin(dir.y) * Mathf.Rad2Deg);
        cameraConfiguration.roll = roll;

        cameraConfiguration.fieldOfView = fov;
        cameraConfiguration.distance = distance;

        cameraConfiguration.OnClampPitch();
        if (CameraController.Instance)
        {
            if (CameraController.Instance.fullRollRotation)
                cameraConfiguration.OnClampRoll();
        }

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

    public void MoveDistanceOnRail(int a_direction)
    {
        distanceOnRail += a_direction * speed * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        //Gizmo change a bit because you need a child to place the camera
        CameraConfiguration camConfig = GetConfiguration();
        
        //Get first child, if exist
        if (rail.transform.childCount > 0)
        {
            camConfig.pivot = rail.transform.GetChild(0).position;

            Gizmos.color = gizmosColor;
            Gizmos.DrawSphere(camConfig.pivot, 0.1f);
            Vector3 position = camConfig.GetPosition();
            Gizmos.DrawLine(camConfig.pivot, position);
            Gizmos.matrix = Matrix4x4.TRS(position, camConfig.GetRotation(), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, camConfig.fieldOfView, 0.5f, 0f, Camera.main.aspect);
            Gizmos.matrix = Matrix4x4.identity;
        }

    }
}
