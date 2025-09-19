using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using System;

public class Rail : MonoBehaviour
{
    public bool IsLoop;

    [Header("Debug")]
    [SerializeField] private Color _railColor;

    private List<Transform> _railNodes = new List<Transform>();
    private List<Transform> _bezierNodes = new List<Transform>();
    private float _length;

    private int _limit = 1000;

    private void Start()
    {
        //Add found rail to nodes
        GetRailNodes();

        //Get length
        if (_railNodes.Count > 1)
        {
            for (int i = _railNodes.Count - 1; i > 0; i--)
            {
                _length += Vector3.Distance(_railNodes[i].position, _railNodes[i - 1].position);
            }

            //If loop -> add length form last to first
            if (IsLoop)
                _length += Vector3.Distance(_railNodes[_railNodes.Count - 1].position, _railNodes[0].position);
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
                    _railNodes.Add(child.transform);
                else
                    _bezierNodes.Add(child.transform);
            }
        }

        if (IsLoop)
        {
            int limit = _limit;
            while (_bezierNodes.Count != _railNodes.Count && limit > 0)
            {
                limit--;
                if (_bezierNodes.Count > _railNodes.Count)
                {
                    _bezierNodes.RemoveAt(_bezierNodes.Count - 1);
                }
                else if (_bezierNodes.Count < _railNodes.Count)
                {
                    Transform _point;
                    
                    if (_bezierNodes.Count == _railNodes.Count - 1)
                    {
                        print($" - 1 | Rail :{_railNodes.Count} | Bezier :{_bezierNodes.Count}");
                        _point = Instantiate(new GameObject(),
                               _railNodes[_railNodes.Count - 1].position + (_railNodes[0].position - _railNodes[_railNodes.Count - 1].position) / 2,
                               Quaternion.Euler(Vector3.zero), transform).transform;
                    }
                    else
                    {
                        print($" 1 | Rail :{_railNodes.Count} | Bezier :{_bezierNodes.Count}");
                        _point = Instantiate(new GameObject(),
                                                       _railNodes[_bezierNodes.Count].position + (_railNodes[_bezierNodes.Count + 1].position - _railNodes[_bezierNodes.Count].position) / 2,
                                                       Quaternion.Euler(Vector3.zero), transform).transform;
                    }
                    _point.tag = "BezierNode";
                    _point.name = $"Bezier [{_bezierNodes.Count}]";
                    _bezierNodes.Add(_point);
                }
            }
        }
        else
        {
            int limit = _limit;
            while (_bezierNodes.Count != _railNodes.Count - 1 && limit > 0)
            {
                limit--;
                if (_bezierNodes.Count > _railNodes.Count - 1)
                {
                    _bezierNodes.RemoveAt(_bezierNodes.Count - 1);
                }
                else if (_bezierNodes.Count < _railNodes.Count - 1)
                {
                    print($" 2 | Rail :{_railNodes.Count} | Bezier :{_bezierNodes.Count}");

                    Transform _point = Instantiate(new GameObject(),
                                                   _railNodes[_bezierNodes.Count].position + (_railNodes[_bezierNodes.Count + 1].position - _railNodes[_bezierNodes.Count].position) / 2,
                                                   Quaternion.Euler(Vector3.zero), transform).transform;
                    _point.tag = "BezierNode";
                    _point.name = $"Bezier [{_bezierNodes.Count}]";
                    _bezierNodes.Add(_point);
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
            float segmentCurrentDistance = Vector3.Distance(_railNodes[i].position, _railNodes[i + 1].position);
            if (a_distance >= segmentLength && a_distance < (segmentLength + segmentCurrentDistance))
            {
                float progressionOnSegment = (a_distance - segmentLength) / segmentCurrentDistance;
                return Vector3.Lerp(_railNodes[i].position, _railNodes[i + 1].position, progressionOnSegment);
            }
            segmentLength += segmentCurrentDistance;
        }

        //Si IsLoop -> connect last to first
        if (IsLoop)
        {
            float segmentCurrentDistance = Vector3.Distance(_railNodes[_railNodes.Count - 1].position, _railNodes[0].position);
            if (a_distance >= segmentLength && a_distance < (segmentLength + segmentCurrentDistance))
            {
                float progressionOnSegment = (a_distance - segmentLength) / segmentCurrentDistance;
                return Vector3.Lerp(_railNodes[_railNodes.Count - 1].position, _railNodes[0].position, progressionOnSegment);
            }
            segmentLength += segmentCurrentDistance;
        }
        return _railNodes[_railNodes.Count - 1].position;
    }

    public Vector3 GetNearestPositionFromTarget(Vector3 a_targetPosition)
    {
        //Used for gizmos debug
        if (_railNodes == null || 
            _bezierNodes == null || 
            (IsLoop && _railNodes.Count != _bezierNodes.Count) || 
            (!IsLoop && _railNodes.Count - 1 != _bezierNodes.Count))
            GetRailNodes();
        if (_railNodes.Count <= 1 || _bezierNodes.Count <= 0)
            throw new Exception("Missing nodes");

        float currentSmallestDistance = Mathf.Infinity;
        Vector3 currentSmallestPosition = Vector3.zero;
        for (int i = 0; i < _railNodes.Count - 1; i++)
        {
            float lerpPosition = MathUtils.GetNearestPointOnSegmentAsLerp(_railNodes[i].position, _railNodes[i + 1].position, a_targetPosition);
            Vector3 projPosition = MathUtils.QuadraticBezier(_railNodes[i].position, _bezierNodes[i].position, _railNodes[i + 1].position, lerpPosition);
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
            Vector3 proj = MathUtils.GetNearestPointOnSegment(_railNodes[i].position, _railNodes[i + 1].position, pointOnRail);
            if ((proj - pointOnRail).sqrMagnitude <= 1e-6f)
            {
                cumulated += Vector3.Distance(_railNodes[i].position, pointOnRail);
                return Mathf.Clamp01(cumulated / _length);
            }

            cumulated += Vector3.Distance(_railNodes[i].position, _railNodes[i + 1].position);
        }

        //Si IsLoop -> connect last to first
        if (IsLoop)
        {
            int last = _railNodes.Count - 1;
            Vector3 proj = MathUtils.GetNearestPointOnSegment(_railNodes[last].position, _railNodes[0].position, pointOnRail);

            if ((proj - pointOnRail).sqrMagnitude <= 1e-6f)
            {
                cumulated += Vector3.Distance(_railNodes[last].position, pointOnRail);
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

        if (_railNodes.Count <= 1 || _bezierNodes.Count <= 0)
            throw new Exception("Missing nodes");

        //Draw debug rail
        Gizmos.color = _railColor;
        for (int i = 0; i < _railNodes.Count - 1; i++)
        {
            for (int j = 1; j <= 10; j++)
            {
                
                Gizmos.DrawLine(MathUtils.QuadraticBezier(_railNodes[i].position, _bezierNodes[i].position, _railNodes[i + 1].position, (j-1) / 10f),
                                MathUtils.QuadraticBezier(_railNodes[i].position, _bezierNodes[i].position, _railNodes[i + 1].position, j / 10f));
            }
        }
        if (IsLoop)
        {
            for (int j = 1; j <= 10; j++)
            {
                Gizmos.DrawLine(MathUtils.QuadraticBezier(_railNodes[_railNodes.Count - 1].position, _bezierNodes[_bezierNodes.Count - 1].position, _railNodes[0].position, (j - 1) / 10f),
                                        MathUtils.QuadraticBezier(_railNodes[_railNodes.Count - 1].position, _bezierNodes[_bezierNodes.Count - 1].position, _railNodes[0].position, j / 10f));
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_playerPosOnRail, .2f);
    }
}
