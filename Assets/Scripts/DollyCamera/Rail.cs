using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Rail : MonoBehaviour
{
    public bool IsLoop;

    [Header("Debug")]
    [SerializeField] private Color _railColor;

    private List<Vector3> _railNodes = new List<Vector3>();
    private float _length;

    private void Start()
    {
        //Add found rail to nodes
        foreach (Transform child in transform)
        {
            if (child != null)
            {
                _railNodes.Add(child.transform.position);
            }
        }

        //Get length
        if(_railNodes.Count > 1)
        {
            for (int i = _railNodes.Count - 1; i > 0; i--)
            {
                _length += Vector3.Distance(_railNodes[i], _railNodes[i - 1]);
            }

            //If loop -> add length form last to first
            if(IsLoop)
                _length += Vector3.Distance(_railNodes[_railNodes.Count - 1], _railNodes[0]);
        }
        else
            _length = 0;
    }

    public float GetLength() => _length;

    public Vector3 GetPosition(float distance)
    {
        //Si loop -> faire boucler la distance
        if (IsLoop)
            distance = distance % _length;
        else
            distance = Mathf.Clamp(distance, 0, _length);

        //For each nodes
        float segmentLength = 0;
        for (int i = 0; i < _railNodes.Count - 1; i++)
        {
            //Calcule la longueur du segment
            float segmentCurrentDistance = Vector3.Distance(_railNodes[i], _railNodes[i + 1]);
            if (distance >= segmentLength && distance < (segmentLength + segmentCurrentDistance))
            {
                float progressionOnSegment = (distance - segmentLength) / segmentCurrentDistance;
                return Vector3.Lerp(_railNodes[i], _railNodes[i + 1], progressionOnSegment);
            }
            segmentLength += segmentCurrentDistance;
        }

        //Si IsLoop -> connect last to first
        if (IsLoop)
        {
            float segmentCurrentDistance = Vector3.Distance(_railNodes[_railNodes.Count - 1], _railNodes[0]);
            if (distance >= segmentLength && distance < (segmentLength + segmentCurrentDistance))
            {
                float progressionOnSegment = (distance - segmentLength) / segmentCurrentDistance;
                return Vector3.Lerp(_railNodes[_railNodes.Count - 1], _railNodes[0], progressionOnSegment);
            }
            segmentLength += segmentCurrentDistance;
        }
        return _railNodes[_railNodes.Count - 1];
    }

    public Vector3 GetNearestPositionFromTarget(Vector3 targetPosition)
    {
        float currentSmallestDistance = Mathf.Infinity;
        Vector3 currentSmallestPosition = Vector3.zero;
        for (int i = 0; i < _railNodes.Count - 1; i++)
        {
            Vector3 projPosition = MathUtils.GetNearestPointOnSegment(_railNodes[i], _railNodes[i + 1], targetPosition);
            float distanceToTarget = Vector3.Distance(projPosition, targetPosition);

            if(distanceToTarget < currentSmallestDistance)
            {
                currentSmallestDistance = distanceToTarget;
                currentSmallestPosition = projPosition;
            }
        }

        return currentSmallestPosition;
    }

    private void OnDrawGizmos()
    {
        //Draw debug rail
        Gizmos.color = _railColor;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }
}
