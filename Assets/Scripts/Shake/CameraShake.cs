using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    
    [SerializeField] public const float c_defaultFade = 1f;

    private float _shakeConstant = 0f;
    [SerializeField] public float shakeCurrent { get; private set; } = 0f;
    
    private Dictionary<int, float> _currentShakes = new();
    
    [SerializeField] private AnimationCurve _impulseCurve;
    [SerializeField] private AnimationCurve _blendCurve;

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
        float maxShake = _shakeConstant;
        
        foreach (var keyValuePair in _currentShakes)
        {
            if(keyValuePair.Value > maxShake)
                maxShake = keyValuePair.Value;
        }
        
        shakeCurrent = maxShake;
    }
    
    private IEnumerator ShakeRoutine(int a_id, float a_intensity, AnimationCurve a_curve, bool a_additiveOfConstant = false, float a_duration = -1)
    {
        float max = a_intensity;
        
        float duration = 0;
        float timeFactor = 1;

        if (a_duration <= 0)
        {
            if (a_curve.length > 1)
                duration = a_curve.keys[a_curve.length - 1].time;
        }
        else
        {
            if (a_curve.length > 1)
            {
                duration = a_curve.keys[a_curve.length - 1].time;

                timeFactor = duration / a_duration;
                duration = a_duration;
            }
        }
        
        float time = duration;

        while (time > 0)
        {
            time -= Time.deltaTime;
            
            _currentShakes[a_id] = Mathf.Lerp(0, max, a_curve.Evaluate(1 - (time / duration)) * (timeFactor)) + (a_additiveOfConstant ? _shakeConstant : 0);
            yield return new WaitForFixedUpdate();
        }
        
        _currentShakes.Remove(a_id);
    }

    public int ShakeImpulse(float a_intensity, bool a_additiveOfConstant = false, float a_duration = c_defaultFade)
    {
        int id;
        do
        {
            id = Random.Range(0, 100000);
        } while (_currentShakes.ContainsKey(id));
        
        _currentShakes.Add(id, _impulseCurve.Evaluate(0) * a_intensity);
        StartCoroutine(ShakeRoutine(id, a_intensity, _impulseCurve, a_additiveOfConstant, a_duration));
        
        return id;
    }
    public int ShakeOneShot(float a_intensity, AnimationCurve a_curve ,bool a_additiveOfConstant = false, float a_duration = 0)
    {
        int id;
        do
        {
            id = Random.Range(0, 100000);
        } while (_currentShakes.ContainsKey(id));
        
        _currentShakes.Add(id, a_curve.Evaluate(0) * a_intensity);
        StartCoroutine(ShakeRoutine(id, a_intensity, a_curve, a_additiveOfConstant, a_duration));

        return id;
    }

    public int CreateShakeInstance(float a_nativeIntensity = 0f, bool a_additiveOfConstant = false)
    {
        int id;
        do
        {
            id = Random.Range(0, 100000);
        } while (_currentShakes.ContainsKey(id));
        
        _currentShakes.Add(id, a_nativeIntensity + (a_additiveOfConstant ? _shakeConstant : 0));
        return id;
    }
    public bool ChangeShakeInstanceValue(int a_id, float a_intensity, bool a_additiveOfConstant = false)
    {
        if (_currentShakes.ContainsKey(a_id))
        {
            _currentShakes[a_id] = a_intensity + (a_additiveOfConstant ? _shakeConstant : 0);
            return true;
        }
        else
            return false;
    }
    
    public bool StopShakeInstance(int a_id)
    {
        return _currentShakes.Remove(a_id);
    }
    public void StopAllShakeInstance()
    {
        _currentShakes.Clear();
    }

    public void ChangeShakeConstant(float a_intensity, float a_fadeDuration = 0, bool a_blend = false)
    {
        if (a_fadeDuration <= 0)
            _shakeConstant = a_intensity;
        else 
            StartCoroutine(ShakeConstantRoutine(a_intensity, a_fadeDuration, a_blend));
    }
    private IEnumerator ShakeConstantRoutine(float a_intensity, float a_fadeDuration, bool a_blend)
    {
        float time = a_fadeDuration;
        
        float start = _shakeConstant;
        float max = a_intensity;
        
        while (time > 0)
        {
            time -= Time.deltaTime;
            
            _shakeConstant = Mathf.Lerp(start, max, a_blend ? _blendCurve.Evaluate(1 - (time / a_fadeDuration)) : 1 - (time / a_fadeDuration));
            yield return new WaitForFixedUpdate();
        }
        
        _shakeConstant = max;
    }
}
