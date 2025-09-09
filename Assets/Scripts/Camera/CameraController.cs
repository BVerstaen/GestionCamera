using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("References")]
    public Camera controlledCamera;
    [SerializeField] private CameraConfiguration _cameraConfiguration;

    [Header("Debug")]
    [SerializeField] private Color _debugColor;

    [SerializeField] private bool _fullRollRotation;
    public bool fullRollRotation { get => _fullRollRotation;}

    private List<AView> activeViews;

    private void OnValidate()
    {
        _cameraConfiguration.OnClampPitch();
        if (_fullRollRotation)
            _cameraConfiguration.OnClampRoll();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
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
        _cameraConfiguration.DrawGizmos(_debugColor);
    }

    private void ApplyConfiguration()
    {
        _cameraConfiguration = ComputeAverage();

        controlledCamera.transform.position = _cameraConfiguration.GetPosition();
        controlledCamera.transform.rotation = _cameraConfiguration.GetRotation();
        controlledCamera.fieldOfView = _cameraConfiguration.fieldOfView;
    }

    public void AddView(AView view)
    {
        if (activeViews == null)
            activeViews = new List<AView>();

        activeViews.Add(view);
    }
    public void RemoveView(AView view)
    {
        if (activeViews == null)
            return;

        activeViews.Remove(view);
    }

    private CameraConfiguration ComputeAverage()
    {
        if (activeViews == null)
            return _cameraConfiguration;

        CameraConfiguration newConfig = new CameraConfiguration();

        Vector2 sumYaw = Vector2.zero;
        float sumPitch = 0;
        float sumRoll = 0;
        Vector2 sumRollComplete = Vector2.zero;
        float sumFov = 0;
        Vector3 sumPivot = Vector3.zero;

        float sumWeight = 0;

        foreach (AView view in activeViews)
        {
            CameraConfiguration config = view.GetConfiguration();

            sumWeight += view.weight;

            sumYaw += new Vector2(Mathf.Cos(config.yaw * Mathf.Deg2Rad), Mathf.Sin(config.yaw * Mathf.Deg2Rad)) * view.weight;
            sumPitch += config.pitch * view.weight;
            sumRoll += config.roll * view.weight;
            sumRollComplete += new Vector2(Mathf.Cos(config.roll * Mathf.Deg2Rad), Mathf.Sin(config.roll * Mathf.Deg2Rad)) * view.weight;

            sumFov += config.fieldOfView * view.weight;

            sumPivot += config.pivot * view.weight;
        }

        if (sumWeight <= 0)
            return _cameraConfiguration;

        newConfig.yaw = Vector2.SignedAngle(Vector2.right, sumYaw);
        newConfig.pitch = sumPitch / sumWeight;
        newConfig.roll = _fullRollRotation ? Vector2.SignedAngle(Vector2.right, sumRollComplete) : sumRoll / sumWeight;

        newConfig.fieldOfView = sumFov / sumWeight;
        newConfig.pivot = sumPivot / sumWeight;

        return newConfig;
    }
    public float ComputeAverageYaw()
    {
        Vector2 sum = Vector2.zero;
        foreach (AView view in activeViews)
        {
            CameraConfiguration config = view.GetConfiguration();
            sum += new Vector2(Mathf.Cos(config.yaw * Mathf.Deg2Rad), Mathf.Sin(config.yaw * Mathf.Deg2Rad)) * view.weight;
        }
        return Vector2.SignedAngle(Vector2.right, sum);
    }
}