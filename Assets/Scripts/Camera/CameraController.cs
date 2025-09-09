using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("References")]
    public Camera controlledCamera;
    [SerializeField] private CameraConfiguration _currentConfiguration;
    [SerializeField] private CameraConfiguration _targetConfiguration;

    [Header("Smoothing")]
    [SerializeField] private float _smoothSpeed;
    [SerializeField] private float _smoothTreshold = 1;

    [Header("Debug")]
    [SerializeField] private Color _debugColor;

    private List<AView> activeViews;

    private void OnValidate()
    {
        _currentConfiguration.OnClampPitch();
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

    private void Start()
    {
        _currentConfiguration = ComputeAverage();
        _targetConfiguration = ComputeAverage();
    }

    private void Update()
    {
        _targetConfiguration = ComputeAverage();
        SmoothCurrentConfigurationToTarget();
        ApplyConfiguration();
    }

    private void OnDrawGizmos()
    {
        _currentConfiguration.DrawGizmos(_debugColor);
    }

    private void ApplyConfiguration()
    {
        controlledCamera.transform.position = _currentConfiguration.GetPosition();
        controlledCamera.transform.rotation = _currentConfiguration.GetRotation();
        controlledCamera.fieldOfView = _currentConfiguration.fieldOfView;
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
        CameraConfiguration newConfig = new CameraConfiguration();

        Vector2 sumYaw = Vector2.zero;
        float sumPitch = 0;
        Vector2 sumRoll = Vector2.zero;
        float sumFov = 0;
        Vector3 sumPivot = Vector3.zero;

        float sumWeight = 0;

        foreach (AView view in activeViews)
        {
            CameraConfiguration config = view.GetConfiguration();

            sumWeight += view.weight;

            sumYaw += new Vector2(Mathf.Cos(config.yaw * Mathf.Deg2Rad), Mathf.Sin(config.yaw * Mathf.Deg2Rad)) * view.weight;
            sumPitch += config.pitch * view.weight;
            sumRoll += new Vector2(Mathf.Cos(config.roll * Mathf.Deg2Rad), Mathf.Sin(config.roll * Mathf.Deg2Rad)) * view.weight;

            sumFov += config.fieldOfView * view.weight;

            sumPivot += config.pivot * view.weight;
        }

        newConfig.yaw = Vector2.SignedAngle(Vector2.right, sumYaw);
        newConfig.pitch = sumPitch / sumWeight;
        newConfig.roll = Vector2.SignedAngle(Vector2.right, sumRoll);

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

    private void SmoothCurrentConfigurationToTarget()
    {
        float currentSpeed = _smoothSpeed * Time.deltaTime;

        if (currentSpeed < _smoothTreshold)
        {
            _currentConfiguration.yaw = _currentConfiguration.yaw + (_targetConfiguration.yaw -_currentConfiguration.yaw) * currentSpeed;
            _currentConfiguration.pitch = _currentConfiguration.pitch + (_targetConfiguration.pitch - _currentConfiguration.pitch) * currentSpeed;
            _currentConfiguration.roll = _currentConfiguration.roll + (_targetConfiguration.roll - _currentConfiguration.roll) * currentSpeed;

            _currentConfiguration.pivot = _currentConfiguration.pivot + (_targetConfiguration.pivot - _currentConfiguration.pivot) * currentSpeed;
            _currentConfiguration.distance = _currentConfiguration.distance + (_targetConfiguration.distance - _currentConfiguration.distance) * currentSpeed;
            _currentConfiguration.fieldOfView = _currentConfiguration.fieldOfView + (_targetConfiguration.fieldOfView - _currentConfiguration.fieldOfView) * currentSpeed;
        }
        else
            _currentConfiguration = _targetConfiguration;
    }
}