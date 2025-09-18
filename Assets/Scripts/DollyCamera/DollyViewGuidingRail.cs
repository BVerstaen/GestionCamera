using UnityEngine;

public class DollyViewGuidingRail : AView
{
    public float roll;
    public float distance;
    public float fov;
    [Space]
    public Transform target;
    [Space]
    public Rail cameraRail;
    public Rail playerRail;
    public AnimationCurve railCurve;

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration cameraConfiguration = new CameraConfiguration();
        
        cameraConfiguration.pivot = GetRailPosition();

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

    private Vector3 GetRailPosition()
    {
        float playerProgressionOnRail = playerRail.GetProgressionOnRail(target.position);
        if(playerProgressionOnRail == float.NaN) //In case of editor
            return cameraRail.GetPosition(0.0f);

        float distanceOnCameraRail = railCurve.Evaluate(playerProgressionOnRail) * cameraRail.GetLength();
        return cameraRail.GetPosition(distanceOnCameraRail);
    }

    private void OnDrawGizmos()
    {
        if (!target)
            return;

        //Gizmo change a bit because you need a child to place the camera
        CameraConfiguration camConfig = GetConfiguration();

        //If rail exist
        if (cameraRail.transform.childCount > 0)
        {
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
