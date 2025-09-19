using System;
using UnityEngine;

public class GetLastCameraPosition : MonoBehaviour
{
    [SerializeField] private TriggerViewVolume _triggerVolume;
    [SerializeField] private Transform _viewToMove;

    private void OnEnable()
    {
        _triggerVolume.OnExitedTrigger += SetViewPosition;
    }

    private void OnDisable()
    {
        _triggerVolume.OnExitedTrigger -= SetViewPosition;
    }

    private void SetViewPosition(Vector3 lastCameraPosition)
    {
        _viewToMove.position = lastCameraPosition;
    }
}
