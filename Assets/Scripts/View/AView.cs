using UnityEngine;

public class AView : MonoBehaviour
{
    //Variable à retirer selon l'exo 1.3 du TP_Volumes
    //public bool IsActiveOnStart = true;

    [SerializeField] private float _weight = 1;
    public float weight { get => _weight; set => _weight = Mathf.Max(0f, value); }

    [Header("Debug")]
    public Color gizmosColor = Color.blue;

    protected virtual void OnValidate()
    {
        weight = weight;
    }

    protected virtual void Start()
    {
        //if (IsActiveOnStart)
        //{
        //    SetActive(IsActiveOnStart);
        //}
    }

    public virtual CameraConfiguration GetConfiguration()
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetActive(bool a_isActive)
    {
        if (a_isActive)
            CameraController.Instance.AddView(this);
        else
            CameraController.Instance.RemoveView(this);
    }

    private void OnDrawGizmos()
    {
        CameraConfiguration camConfig = GetConfiguration();

        Gizmos.color = gizmosColor;
        Gizmos.DrawSphere(camConfig.pivot, 0.1f);
        Vector3 position = camConfig.GetPosition();
        Gizmos.DrawLine(camConfig.pivot, position);
        Gizmos.matrix = Matrix4x4.TRS(position, camConfig.GetRotation(), Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, camConfig.fieldOfView, 0.5f, 0f, Camera.main.aspect);
        Gizmos.matrix = Matrix4x4.identity;
    }
}