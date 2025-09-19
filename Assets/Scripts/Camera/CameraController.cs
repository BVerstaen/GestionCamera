using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("References")]
    public Camera controlledCamera;
    [SerializeField] private CameraCollision _cameraCollision;
    [Space]
    [SerializeField] private CameraConfiguration _currentConfiguration;
    [SerializeField] private CameraConfiguration _targetConfiguration;

    [Header("Smoothing")]
    [SerializeField] private float _smoothSpeed = 1;
    [SerializeField] private float _smoothTreshold = 1;

    [Header("Debug")]
    [SerializeField] private Color _debugColor;

    [SerializeField] private bool _fullRollRotation;
    public bool fullRollRotation { get => _fullRollRotation; }
    [SerializeField] private bool _deactivateSmooth;

    private List<AView> activeViews;

    private bool _isCutRequested = false;

    private CameraShake _shake;
    public CameraShake cameraShake { get => _shake; }    

    [SerializeField] private float _cameraShakeFactor = 1f;
    [SerializeField] public bool rollShake = false;
    
    private void OnValidate()
    {
        _currentConfiguration.OnClampPitch();
        if (!_fullRollRotation)
            _currentConfiguration.OnClampRoll();

        if (_smoothSpeed <= 0)
            _smoothSpeed = 1;
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
        _shake = CameraShake.Instance;
        
        _currentConfiguration = ComputeAverage();
        _targetConfiguration = ComputeAverage();
    }

    private void Update()
    {
        _targetConfiguration = ComputeAverage();

        //Collision 
        if (_cameraCollision.CanCollide() && !_cameraCollision.HasClearPathToTarget(_targetConfiguration.pivot, out Vector3 hitPosition))
            _targetConfiguration.pivot = hitPosition;

        if (_isCutRequested)
        {
            _currentConfiguration = _targetConfiguration;
            _isCutRequested = false;
        }
        else
        {
            SmoothCurrentConfigurationToTarget();
        }

        ApplyConfiguration();
    }

    private void OnDrawGizmos()
    {
        _currentConfiguration.DrawGizmos(_debugColor);
    }

    private void ApplyConfiguration()
    {
        CameraConfiguration applyConfiguration = _currentConfiguration;
        
        ApplyShake(ref applyConfiguration);

        controlledCamera.transform.position = applyConfiguration.GetPosition();
        controlledCamera.transform.rotation = applyConfiguration.GetRotation();
        controlledCamera.fieldOfView = applyConfiguration.fieldOfView;
    }

    private void ApplyShake(ref CameraConfiguration r_cameraConfig)
    {
        if (cameraShake.shakeCurrent <= 0)
            return;

        float intensity = cameraShake.shakeCurrent * _cameraShakeFactor;

        if (controlledCamera.orthographic)
        {
            r_cameraConfig.pivot += new Vector3(Random.Range(intensity / 2f, intensity),
                                                Random.Range(intensity / 2f, intensity),
                                                Random.Range(intensity / 2f, intensity));
        }
        else
        {
            r_cameraConfig.yaw += Random.Range(intensity / 2f, intensity);
            r_cameraConfig.pitch += Random.Range(intensity / 2f, intensity);
            if (rollShake)
                r_cameraConfig.roll += Random.Range(intensity / 2f, intensity);
        }
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
        if (activeViews == null || activeViews.Count <= 0)
            return _currentConfiguration;

        CameraConfiguration newConfig = new CameraConfiguration();

        Vector2 sumYaw = Vector2.zero;
        float sumPitch = 0;
        float sumRoll = 0;
        Vector2 sumRollComplete = Vector2.zero;
        float sumFov = 0;
        Vector3 sumPivot = Vector3.zero;

        float totalWeight = 0;

        foreach (AView view in activeViews)
        {
            CameraConfiguration config = view.GetConfiguration();

            totalWeight += view.weight;

            sumYaw += new Vector2(Mathf.Cos(config.yaw * Mathf.Deg2Rad), Mathf.Sin(config.yaw * Mathf.Deg2Rad)) * view.weight;
            sumPitch += config.pitch * view.weight;
            sumRoll += config.roll * view.weight;
            sumRollComplete += new Vector2(Mathf.Cos(config.roll * Mathf.Deg2Rad), Mathf.Sin(config.roll * Mathf.Deg2Rad)) * view.weight;

            sumFov += config.fieldOfView * view.weight;
            sumPivot += config.pivot * view.weight;
        }

        if (totalWeight <= 0)
            return _currentConfiguration;

        newConfig.yaw = Vector2.SignedAngle(Vector2.right, sumYaw);
        newConfig.pitch = sumPitch / totalWeight;
        newConfig.roll = !_fullRollRotation ? Vector2.SignedAngle(Vector2.right, sumRollComplete) : sumRoll / totalWeight;

        newConfig.fieldOfView = sumFov / totalWeight;
        newConfig.pivot = sumPivot / totalWeight;

        return newConfig;
    }

    private void SmoothCurrentConfigurationToTarget()
    {
        float currentSpeed = (_smoothSpeed > 0 ? _smoothSpeed : 1) * Time.deltaTime;

        if (!_deactivateSmooth && currentSpeed < _smoothTreshold)
        {
            _currentConfiguration.yaw = _currentConfiguration.yaw + Mathf.DeltaAngle(_currentConfiguration.yaw, _targetConfiguration.yaw) * currentSpeed;
            _currentConfiguration.pitch = _currentConfiguration.pitch + (_targetConfiguration.pitch - _currentConfiguration.pitch) * currentSpeed;
            if (_fullRollRotation)
                _currentConfiguration.roll = _currentConfiguration.roll + Mathf.DeltaAngle(_currentConfiguration.roll, _targetConfiguration.roll) * currentSpeed;
            else
                _currentConfiguration.roll = _currentConfiguration.roll + (_targetConfiguration.roll - _currentConfiguration.roll) * currentSpeed;

            _currentConfiguration.pivot = _currentConfiguration.pivot + (_targetConfiguration.pivot - _currentConfiguration.pivot) * currentSpeed;
            _currentConfiguration.distance = _currentConfiguration.distance + (_targetConfiguration.distance - _currentConfiguration.distance) * currentSpeed;
            _currentConfiguration.fieldOfView = _currentConfiguration.fieldOfView + (_targetConfiguration.fieldOfView - _currentConfiguration.fieldOfView) * currentSpeed;
        }
        else
            _currentConfiguration = _targetConfiguration;
    }

    public void Cut()
    {
        _isCutRequested = true;
    }

    public void AddDeactivateCameraCollision()
    {
        _cameraCollision.AddNoCollisionVolume();
    }

    public void RemoveDeactivateCameraCollision()
    {
        _cameraCollision.RemoveNoCollisionVolume();
    }
}