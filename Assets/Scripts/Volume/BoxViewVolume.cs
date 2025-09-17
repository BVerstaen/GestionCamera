using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BoxViewVolume : AViewVolume
{
    public string _targetTagName;
    public BoxCollider BoxCollider;
    public float outerBoxSize;

    private float _distance;
    private Transform _targetTransform;

    private float InnerBoxSize
    {
        get => BoxCollider.size.x;
        set => BoxCollider.size = new Vector3(value, BoxCollider.size.y, BoxCollider.size.z);
    }

    private void OnValidate()
    {
        if(BoxCollider == null)
        {
            Debug.LogWarning("A box collider is needed");
            BoxCollider = gameObject.AddComponent<BoxCollider>();
            BoxCollider.isTrigger = true;

            outerBoxSize = InnerBoxSize + 1.0f;
        }

        if (outerBoxSize < InnerBoxSize)
        {
            outerBoxSize = BoxCollider.size.x;
        }
    }

    private void Start()
    {
        _targetTransform = GameObject.FindGameObjectWithTag(_targetTagName).transform;
    }

    private void Update()
    {
        if (!_targetTransform)
            return;

        _distance = Vector3.Distance(transform.position, _targetTransform.position);

        if (!IsInTunnel(_targetTransform.position))
        {
            if(IsActive) SetActive(false);
            return;
        }

        if (_distance <= outerBoxSize && !IsActive)
            SetActive(true);
        else if (_distance > outerBoxSize && IsActive)
            SetActive(false);
    }

    public override float ComputeSelfWeight()
    {
        float weight = Mathf.InverseLerp(outerBoxSize, InnerBoxSize, _distance);
        return Mathf.Clamp01(weight);
    }

    public bool IsInTunnel(Vector3 position)
    {
        Vector3 localPosition = transform.InverseTransformPoint(position);

        return (localPosition.y >= (BoxCollider.center.y - BoxCollider.size.y / 2) &&
                localPosition.y <= (BoxCollider.center.y + BoxCollider.size.y / 2) &&
                localPosition.z >= (BoxCollider.center.z - BoxCollider.size.z / 2) &&
                localPosition.z <= (BoxCollider.center.z + BoxCollider.size.z / 2));
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        //Draw inner box
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(BoxCollider.center, BoxCollider.size);

        //Draw outer box
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(BoxCollider.center, new Vector3(outerBoxSize * 2, BoxCollider.size.y, BoxCollider.size.z));
    }

}
