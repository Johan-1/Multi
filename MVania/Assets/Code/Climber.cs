using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : MonoBehaviour
{

    [SerializeField] Transform[] _positions;
    [SerializeField] float _speed;
    [SerializeField] float _cornerSpeed;
    [SerializeField] float _dstBeforeEdgeStartRotate = 0.5f;
    [SerializeField] int _startTargetIndex;

    int _targetIndex;

    // initialize to a non existing index(dont want this set to an index that exists)
    int _targetIndexOnStartRotate = -1;

    

    void Awake()
    {
        _targetIndex = _startTargetIndex;        
    }

    void Update()
    {
        // move towards the transform of target
        transform.position = Vector2.MoveTowards(transform.position, _positions[_targetIndex].position, _speed * Time.deltaTime);
        CheckDistance();

        // if at target position, increment targetindex to next in array
        if (transform.position == _positions[_targetIndex].position)
        {            
            _targetIndex++;
            // reset if out of bounds
            if (_targetIndex == _positions.Length)
                _targetIndex = 0;

        }

    }

    void CheckDistance()
    {
        // get dst squared from current position to target position
        float distance = (transform.position - _positions[_targetIndex].position).sqrMagnitude;
        
        // start rotation if distance is less then _dst before edge, also check so we only start rotation towards the same point once
        if (distance < (_dstBeforeEdgeStartRotate * _dstBeforeEdgeStartRotate) && _targetIndexOnStartRotate != _targetIndex)
        {
            StartCoroutine(RotateOverTime());

            // set witch index was the current one when we started to rotate
            _targetIndexOnStartRotate = _targetIndex;

        }

    }

    IEnumerator RotateOverTime()
    {
        // rotate from current rotation to the rotation of the target point
        Quaternion start = transform.rotation;
        Quaternion end = _positions[_targetIndex].rotation;        

        // lerp rotation
        float fraction = 0.0f;
        while (fraction < 1.0f)
        {
            fraction += Time.deltaTime * _cornerSpeed;
            transform.rotation = Quaternion.Lerp(start, end , fraction);
            yield return null;
        }

    }

}
