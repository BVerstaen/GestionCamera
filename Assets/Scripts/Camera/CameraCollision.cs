using UnityEngine;
public class CameraCollision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _targetObject;

    [Header("Parameters")]
    [SerializeField] private float _sphereCastRadius;
    [SerializeField] private LayerMask _obstacleLayers;

    private Vector3 _gizmosCameraPoint;

    private void OnValidate()
    {
        if (_targetObject == null)
            _targetObject = GameObject.FindGameObjectWithTag("Player");
    }

    public bool HasClearPathToTarget(Vector3 startPosition, out Vector3 hitPosition)
    {
        hitPosition = startPosition;
        _gizmosCameraPoint = hitPosition;

        Vector3 targetPos = _targetObject.transform.position;
        Vector3 rayDirection = -(targetPos - startPosition).normalized;
        float rayDistance = Vector3.Distance(targetPos, startPosition);

        //Check if something is in the way
        if (Physics.SphereCast(targetPos, _sphereCastRadius, rayDirection, out RaycastHit hitInfo, rayDistance, _obstacleLayers))
        {
            hitPosition = hitInfo.point;
            _gizmosCameraPoint = hitPosition;
            return hitInfo.collider.gameObject == _targetObject;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(_gizmosCameraPoint, _sphereCastRadius);
    }
}
