using System;
using UnityEngine;

public class AViewVolume : MonoBehaviour
{
    private enum ChangeCameraCollision
    {
        DontChange,
        DeactivateCollision,
        ActivateCollision
    }

    public int priority = 0;
    public AView view;

    private int _Uid = 0;
    public static int NextUid = 0;

    public bool isCutOnSwitch;

    [SerializeField] private ChangeCameraCollision _changeCameraCollision;

    protected bool IsActive {  get; private set; }

    public int Uid { get => _Uid; }
    
    private void Awake()
    {
        _Uid = NextUid;
        NextUid++;
    }
    public virtual float ComputeSelfWeight() { return 1.0f; }

    protected void SetActive(bool isActive)
    {
        IsActive = isActive;

        if (isCutOnSwitch)
            CameraController.Instance.Cut();

        if (isActive)
        {
            ViewVolumeBlender.Instance.AddVolume(this);

            if (_changeCameraCollision == ChangeCameraCollision.DeactivateCollision)
                CameraController.Instance.AddDeactivateCameraCollision();
            else if (_changeCameraCollision == ChangeCameraCollision.ActivateCollision)
                CameraController.Instance.AddActivateCameraCollision();
        }
        else
        {
            ViewVolumeBlender.Instance.RemoveVolume(this);

            if (_changeCameraCollision == ChangeCameraCollision.DeactivateCollision)
                CameraController.Instance.RemoveDeactivateCameraCollision();
            else if (_changeCameraCollision == ChangeCameraCollision.ActivateCollision)
                CameraController.Instance.RemoveActivateCameraCollision();
        } 
    }
}
