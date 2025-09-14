using System.Xml;
using UnityEngine;

public enum CameraPosition
{
    bottom = 0,
    middle = 1,
    top = 2
}
public class FreeFollowView:  AView
{
    public float[] pitch =  new float[3];
    public float[] roll =  new float[3];
    public float[] fov =  new float[3];

    public float yaw;
    [SerializeField] private float yawSpeed = 1;

    private float _target;

    [SerializeField] private Curve _curve;
    private float _curvePosition;
    [SerializeField] private float curvePosition { get => _curvePosition; set => Mathf.Clamp01(value); }
    [SerializeField] private float _curveSpeed = 1;


    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration cameraConfiguration = new CameraConfiguration();
        
        cameraConfiguration.yaw = yaw;
        cameraConfiguration.pitch = GetCurveValues(pitch);
        cameraConfiguration.roll = GetCurveValues(roll);
        
        cameraConfiguration.fieldOfView = GetCurveValues(fov);
        
        cameraConfiguration.pivot = _curve.GetPosition(curvePosition, Matrix4x4.TRS(transform.position, Quaternion.Euler(0, yaw, 0), Vector3.one));
        cameraConfiguration.distance = 0;
        
        cameraConfiguration.OnClampPitch();
        if (!CameraController.Instance.fullRollRotation)
            cameraConfiguration.OnClampRoll();
        
        return cameraConfiguration;
    }

    public void OnCameraInputs(Vector2 a_input, float a_deltaTime)
    {
        OnYawInputs(a_input.x, a_deltaTime);
        OnCurveInputs(a_input.y, a_deltaTime);
    }

    public void OnYawInputs(float a_input, float a_deltaTime)
    {
        yaw += a_input * yawSpeed * a_deltaTime;
    }
    public void OnCurveInputs(float a_input, float a_deltaTime)
    {
        curvePosition += a_input * _curveSpeed * a_deltaTime;
    }

    private float GetCurveValues(float[] a_values)
    {
        if (curvePosition <= 0.5f)
        {
            float temp = curvePosition / 0.5f;
            return Mathf.Lerp(a_values[0], a_values[1], temp);
        }
        else
        {
            float temp = (curvePosition - 0.5f) / 0.5f;
            return Mathf.Lerp(a_values[1], a_values[2], temp);
        }
    }
}
