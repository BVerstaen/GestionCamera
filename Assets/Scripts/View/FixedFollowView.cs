using UnityEngine;

public class FixedFollowView : AView
{
    public float roll;
    public float fov = 80;

    [SerializeField] private float _yawOffsetMax = 180f;
    [SerializeField] private float _pitchOffsetMax = 90f;

    [Space(7)]
    public Vector3 target;
    public Transform targetTransform;

    public bool _getLastCameraPos;

    private void Reset()
    {
        if (targetTransform == null)
            targetTransform = GameObject.FindGameObjectWithTag("Player")?.transform; 
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        _yawOffsetMax = Mathf.Clamp(_yawOffsetMax, 0, 180);
        _pitchOffsetMax = Mathf.Clamp(_pitchOffsetMax, 0, 90);

        if (targetTransform == null)
            targetTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration cameraConfiguration = new CameraConfiguration();

        if (targetTransform != null)
        {
            target = targetTransform.position;
        }

        Vector3 dir = transform.InverseTransformDirection((target - transform.position).normalized);
        float dirAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        cameraConfiguration.yaw = transform.eulerAngles.y + Mathf.Clamp(dirAngle, -_yawOffsetMax, _yawOffsetMax);
        cameraConfiguration.pitch = Mathf.Clamp(-Mathf.Asin(dir.y) * Mathf.Rad2Deg, -_pitchOffsetMax, _pitchOffsetMax);
        cameraConfiguration.roll = roll;

        cameraConfiguration.fieldOfView = fov;

        cameraConfiguration.pivot = transform.position;
        cameraConfiguration.distance = 0;

        cameraConfiguration.OnClampPitch();
        if (CameraController.Instance)
        {
            if (!CameraController.Instance.fullRollRotation)
                cameraConfiguration.OnClampRoll();
        }

        return cameraConfiguration;
    }

    public override void SetActive(bool a_isActive)
    {
        if (_getLastCameraPos && a_isActive)
            transform.position = Camera.main.transform.position;

        base.SetActive(a_isActive);
    }
}
