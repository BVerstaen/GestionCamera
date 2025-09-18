using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("References")]
    public GameObject targetObject;

    [Header("Parameters")]
    public float sphereCastRadius;

    private void OnValidate()
    {
        if (targetObject == null)
            targetObject = GameObject.FindGameObjectWithTag("Player");
    }

    public bool HasClearPathToTarget(Vector3 startPosition, out Vector3 hitPosition)
    {
        hitPosition = startPosition;
        Vector3 targetPos = targetObject.transform.position;
        Vector3 rayDirection = (targetPos - startPosition).normalized;
        float rayDistance = Vector3.Distance(targetPos, startPosition);
        if (Physics.SphereCast(startPosition, sphereCastRadius, rayDirection, out RaycastHit hitInfo, rayDistance))
        {
            return hitInfo.collider.gameObject == targetObject;
        }
        return true;
    }
}
