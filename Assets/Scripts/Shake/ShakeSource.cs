using System;
using UnityEngine;

public class ShakeSource : MonoBehaviour
{
    [SerializeField] private float _intensityStart;
    [SerializeField] private float _intensityCenter;
    [SerializeField] private bool _additiveOfConstant;
    [SerializeField] private float _duration;
    
    [Space(7)]
    [SerializeField] private bool _constantShake;

    private float _maxDistance;
    private Transform _target;
    private int _id;
    private bool _active;
    
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
            float intensity = Mathf.Lerp( _intensityCenter, _intensityStart,distance / _maxDistance);
            
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
