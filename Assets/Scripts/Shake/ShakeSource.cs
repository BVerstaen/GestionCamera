using System;
using UnityEngine;

public class ShakeSource : MonoBehaviour
{
    [SerializeField] private bool _constantShake;
    [SerializeField] private bool _additiveOfConstant;
    [Space(7)]
    [SerializeField] private float _intensityStart = 0;
    [SerializeField] private float _intensityCenter = 1;
    [SerializeField] private float _fullIntensityZone;
    private float DeadZone { get => _maxDistance * _fullIntensityZone; set => _fullIntensityZone = Mathf.Clamp01(value); }
    

    private float _maxDistance;
    private Transform _target;
    private int _id;
    private bool _active;

    private bool _wasOnConstant = false;

    private void OnValidate()
    {
        DeadZone = _fullIntensityZone;

        if (_constantShake && !_wasOnConstant)
        {
            _additiveOfConstant = false;
            _wasOnConstant = true;
        }
        else if (_additiveOfConstant && _wasOnConstant)
        {
            _constantShake = false;
            _wasOnConstant = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _target = other.transform;
            _maxDistance = (_target.position - transform.position).magnitude;

            if (_maxDistance <= 0)
                return;
            
            _active = true;
            
            if (!_constantShake)
                _id = CameraShake.Instance.CreateShakeInstance(_intensityStart, _additiveOfConstant);
            else
                CameraShake.Instance.ChangeShakeConstant(_intensityStart);
        }
    }

    private void Update()
    {
        if (_active)
        {
            float distance = (_target.position - transform.position).magnitude;
            float intensity;

            if (_maxDistance - DeadZone == 0)
                intensity = Mathf.Lerp(_intensityCenter, _intensityStart, Mathf.Clamp01((distance - DeadZone) / (_maxDistance - DeadZone)));
            else
                intensity = _intensityCenter;


            if (!_constantShake)
                CameraShake.Instance.ChangeShakeInstanceValue(_id, intensity, _additiveOfConstant);
            else
                CameraShake.Instance.ChangeShakeConstant(intensity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _active = false;

            if (!_constantShake)
                CameraShake.Instance.StopShakeInstance(_id);
            else
                CameraShake.Instance.ChangeShakeConstant(0f, 1f, true);
        }
    }
    
    
}
