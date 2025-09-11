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

    private void Update()
    {
        UpdateWeight();
    }

    private void UpdateWeight()
    {
        //Reset views to 0
        foreach (AViewVolume viewVolume in _activeViewVolumes)
            viewVolume.view.weight = 0;

        //Sort active volumes
        _activeViewVolumes.Sort(0, _activeViewVolumes.Count, ComparePriority());

        //Calculate weight
        foreach (AViewVolume v in _activeViewVolumes)
        {
            float weight = Mathf.Clamp01(v.ComputeSelfWeight());
            float remainingWeight = 1.0f - weight;

            //Multiply all active views by remaining weight
            foreach (AViewVolume vToAdd in _activeViewVolumes)
                vToAdd.view.weight *= remainingWeight;

            //Add weight to the current view
            v.view.weight += weight;
        }
    }
    
    private Comparer<AViewVolume> ComparePriority()
    {
        return Comparer<AViewVolume>.Create((a, b) =>
        {
            if (a.priority == b.priority)
                return a.Uid.CompareTo(b.Uid);
            else
                return a.priority.CompareTo(b.priority);
        });
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

    private void OnGUI()
    {
        //List active volumes
        foreach(AViewVolume aViewVolume in _activeViewVolumes)
        {
            GUILayout.Label(aViewVolume.gameObject.name);

        }
    }
}
