using UnityEngine;

public class AView : MonoBehaviour
{
    public bool IsActiveOnStart = true;

    [SerializeField] private float _weight = 1;
    public float weight { get => _weight; set => _weight = Mathf.Max(0f, value); }

    protected virtual void OnValidate()
    {
        weight = weight;
    }

    protected virtual void Start()
    {
        if (IsActiveOnStart)
        {
            SetActive(IsActiveOnStart);
        }
    }

    public virtual CameraConfiguration GetConfiguration()
    {
        throw new System.NotImplementedException();
    }

    public void SetActive(bool a_isActive)
    {
        if (a_isActive)
            CameraController.Instance.AddView(this);
        else
            CameraController.Instance.RemoveView(this);
    }
}
