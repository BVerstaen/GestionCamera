using System.Collections.Generic;
using UnityEngine;

public class ViewVolumeBlender : MonoBehaviour
{
    public static ViewVolumeBlender Instance;

    private List<AViewVolume> _activeViewVolumes = new List<AViewVolume>();
    private Dictionary<AView, List<AViewVolume>> _volumesPerViews = new Dictionary<AView, List<AViewVolume>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddVolume(AViewVolume a_volume)
    {
        _activeViewVolumes.Add(a_volume);

        if (!_volumesPerViews.ContainsKey(a_volume.view))
        {
            _volumesPerViews.Add(a_volume.view, new List<AViewVolume>());
            a_volume.view.SetActive(true);
        }

        _volumesPerViews[a_volume.view].Add(a_volume);
    }

    public void RemoveVolume(AViewVolume a_volume)
    {
        _activeViewVolumes.Remove(a_volume);

        _volumesPerViews[a_volume.view].Remove(a_volume);
        if(_volumesPerViews[a_volume.view].Count <= 0)
        {
            _volumesPerViews.Remove(a_volume.view);
            a_volume.view.SetActive(false);
        }
    }
}
