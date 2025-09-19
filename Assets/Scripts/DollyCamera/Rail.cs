using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class Rail : MonoBehaviour
{
    public bool IsLoop;

    [Header("Debug")]
    [SerializeField] private Color _railColor;

    private List<Vector3> _railNodes = new List<Vector3>();
    private List<Vector3> _bezierNodes = new List<Vector3>();
    private float _length;

    private void Start()
    {
        //Add found rail to nodes
        GetRailNodes();

        //Get length
        if (_railNodes.Count > 1)
        {
            for (int i = _railNodes.Count - 1; i > 0; i--)
            {
                _length += Vector3.Distance(_railNodes[i], _railNodes[i - 1]);
            }

            //If loop -> add length form last to first
            if (IsLoop)
                _length += Vector3.Distance(_railNodes[_railNodes.Count - 1], _railNodes[0]);
        }
        else
            _length = 0;
    }

    public float GetLength() => _length;

    public bool IsRailNodesInitialized() => _railNodes.Count > 0;

    [Button]
    private void GetRailNodes()
    {
        _railNodes.Clear();
        _bezierNodes.Clear();

        foreach (Transform child in transform)
        {
            if (child != null)
            {
                if (!child.CompareTag("BezierNode"))
                    _railNodes.Add(child.transform.position);
                else
                    _bezierNodes.Add(child.transform.position);
            }
        }

        if (IsLoop)
        {
            while (_bezierNodes.Count == _railNodes.Count)
            {
                if (_bezierNodes.Count > _railNodes.Count)
                {
                    _bezierNodes.RemoveAt(_bezierNodes.Count - 1);
                }
                else if (_bezierNodes.Count < _railNodes.Count)
                {
                    Transform _point = Instantiate(new GameObject(), _railNodes[_bezierNodes.Count] - _railNodes[_bezierNodes.Count + 1], Quaternion.Euler(Vector3.zero), transform).transform;
                    _point.tag = "BezierNode";
                    _point.name = $"Bezier [{_bezierNodes}]";
                    _bezierNodes.Add(_point.position);
                }
            }
        }
        else
        {
            while (_bezierNodes.Count == _railNodes.Count - 1)
            {
                if (_bezierNodes.Count > _railNodes.Count - 1)
                {
                    _bezierNodes.RemoveAt(_bezierNodes.Count - 1);
                }
                else if (_bezierNodes.Count < _railNodes.Count - 1)
                {
                    Transform _point = Instantiate(new GameObject(), _railNodes[_bezierNodes.Count] - _railNodes[_bezierNodes.Count + 1], Quaternion.Euler(Vector3.zero), transform).transform;
                    _point.tag = "BezierNode";
                    _point.name = $"Bezier [{_bezierNodes}]";
                    _bezierNodes.Add(_point.position);
                }
            }
        }
    }

    public Vector3 GetPosition(float a_distance)
    {
        //Used for gizmos debug
        if (_railNodes.Count <= 0)
            GetRailNodes();

        //Si loop -> faire boucler la distance
        if (IsLoop)
            a_distance = a_distance % _length;
        else
            a_distance = Mathf.Clamp(a_distance, 0, _length);

        //For each nodes
        float segmentLength = 0;
        for (int i = 0; i < _railNodes.Count - 1; i++)
        {
            //Calcule la longueur du segment
            float segmentCurrentDistance = Vector3.Distance(_railNodes[i], _railNodes[i + 1]);
            if (a_distance >= segmentLength && a_distance < (segmentLength + segmentCurrentDistance))
            {
                float progressionOnSegment = (a_distance - segmentLength) / segmentCurrentDistance;
                return Vector3.Lerp(_railNodes[i], _railNodes[i + 1], progressionOnSegment);
            }
            segmentLength += segmentCurrentDistance;
        }

        //Si IsLoop -> connect last to first
        if (IsLoop)
        {
            float segmentCurrentDistance = Vector3.Distance(_railNodes[_railNodes.Count - 1], _railNodes[0]);
            if (a_distance >= segmentLength && a_distance < (segmentLength + segmentCurrentDistance))
            {
                float progressionOnSegment = (a_distance - segmentLength) / segmentCurrentDistance;
                return Vector3.Lerp(_railNodes[_railNodes.Count - 1], _railNodes[0], progressionOnSegment);
            }
            segmentLength += segmentCurrentDistance;
        }
        return _railNodes[_railNodes.Count - 1];
    }

    public Vector3 GetNearestPositionFromTarget(Vector3 a_targetPosition)
    {
        //Used for gizmos debug
        if (_railNodes.Count <= 0)
            GetRailNodes();

        float currentSmallestDistance = Mathf.Infinity;
        Vector3 currentSmallestPosition = Vector3.zero;
        for (int i = 0; i < _railNodes.Count - 1; i++)
        {
            float lerpPosition = MathUtils.GetNearestPointOnSegmentAsLerp(_railNodes[i], _railNodes[i + 1], a_targetPosition);
            Vector3 projPosition = MathUtils.QuadraticBezier(_railNodes[i], _bezierNodes[i], _railNodes[i + 1], lerpPosition);
            float distanceToTarget = Vector3.Distance(projPosition, a_targetPosition);

            if(distanceToTarget < currentSmallestDistance)
            {
                currentSmallestDistance = distanceToTarget;
                currentSmallestPosition = projPosition;
            }
        }

        return currentSmallestPosition;
    }

    public float GetProgressionOnRail(Vector3 a_targetPosition)
    {
        Vector3 pointOnRail = GetNearestPositionFromTarget(a_targetPosition);
        _playerPosOnRail = pointOnRail;

        float cumulated = 0f;

        for (int i = 0; i < _railNodes.Count - 1; i++)
        {
            Vector3 proj = MathUtils.GetNearestPointOnSegment(_railNodes[i], _railNodes[i + 1], pointOnRail);
            if ((proj - pointOnRail).sqrMagnitude <= 1e-6f)
            {
                cumulated += Vector3.Distance(_railNodes[i], pointOnRail);
                return Mathf.Clamp01(cumulated / _length);
            }

            cumulated += Vector3.Distance(_railNodes[i], _railNodes[i + 1]);
        }

        //Si IsLoop -> connect last to first
        if (IsLoop)
        {
            int last = _railNodes.Count - 1;
            Vector3 proj = MathUtils.GetNearestPointOnSegment(_railNodes[last], _railNodes[0], pointOnRail);

            if ((proj - pointOnRail).sqrMagnitude <= 1e-6f)
            {
                cumulated += Vector3.Distance(_railNodes[last], pointOnRail);
                return Mathf.Clamp01(cumulated / _length);
            }
        }

        return 1f;
    }

    private Vector3 _playerPosOnRail = Vector3.zero;

    private void OnDrawGizmos()
    {
        if (_railNodes == null || _bezierNodes == null || (IsLoop && _railNodes.Count != _bezierNodes.Count) || (!IsLoop && _railNodes.Count - 1 != _bezierNodes.Count))
            GetRailNodes();

        //Draw debug rail
        Gizmos.color = _railColor;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            for (int j = 1; j <= 10; j++)
            {
                
                Gizmos.DrawLine(MathUtils.QuadraticBezier(_railNodes[i], _bezierNodes[i], _railNodes[i + 1], (j-1) / 10f),
                                MathUtils.QuadraticBezier(_railNodes[i], _bezierNodes[i], _railNodes[i + 1], j / 10f));
            }
        }
        if (IsLoop)
        {
            for (int j = 1; j <= 10; j++)
            {
                Gizmos.DrawLine(MathUtils.QuadraticBezier(_railNodes[_railNodes.Count - 1], _bezierNodes[_bezierNodes.Count - 1], _railNodes[0], (j - 1) / 10f),
                                        MathUtils.QuadraticBezier(_railNodes[_railNodes.Count - 1], _bezierNodes[_bezierNodes.Count - 1], _railNodes[0], j / 10f));
            }
        }


        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_playerPosOnRail, .2f);
    }
}
