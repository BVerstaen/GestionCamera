using System;
using UnityEngine;

public class ShakeOneShot : MonoBehaviour
{
    [SerializeField] private float _intensity;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private bool _additiveOfConstant;
    [SerializeField] private float _duration;
    
    [Space(7)]
    [SerializeField] private bool _stopOnExit;

    private int _id;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _id = CameraShake.Instance.ShakeOneShot(_intensity, _curve, _additiveOfConstant, _duration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_stopOnExit && other.CompareTag("Player"))
        {
            CameraShake.Instance.StopShakeInstance(_id);
        }
    }
}
