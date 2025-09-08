using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("References")]
    public Camera controlledCamera;
    public CameraConfiguration cameraConfiguration;

    [Header("Debug")]
    [SerializeField] private Color _debugColor;

    private void OnValidate()
    {
        cameraConfiguration.OnClampPitch();
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        ApplyConfiguration();
    }

    private void OnDrawGizmos()
    {
        cameraConfiguration.DrawGizmos(_debugColor);
    }

    private void ApplyConfiguration()
    {
        controlledCamera.transform.position = cameraConfiguration.GetPosition();
        controlledCamera.transform.rotation = cameraConfiguration.GetRotation();
        controlledCamera.fieldOfView = cameraConfiguration.fieldOfView;
    }
}
