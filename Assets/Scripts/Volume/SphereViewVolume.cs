using UnityEngine;

public class SphereViewVolume : AViewVolume
{
    public string _targetTagName;
    public float outerRadius;
    public float innerRadius;

    private float _distance;
    private Transform _targetTransform;

    private void OnValidate()
    {
        if(innerRadius > outerRadius)
        {
            innerRadius = outerRadius;
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

        if (_distance <= outerRadius && !IsActive)
            SetActive(true);
        else if (_distance > outerRadius && IsActive)
            SetActive(false);
    }

    public override float ComputeSelfWeight()
    {
        if (outerRadius == innerRadius)
            return 1;

        float weight = Mathf.InverseLerp(outerRadius, innerRadius, _distance);
        return Mathf.Clamp01(weight);
    }

    private void OnDrawGizmos()
    {
        //Draw outer sphere
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, outerRadius);

        //Draw inner sphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }
}
