using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : MonoBehaviour
{

    [SerializeField] Transform[] _positions;
    [SerializeField] float _speed;
    [SerializeField] float _cornerSpeed;
    [SerializeField] float _dstBeforeEdgeStartRotate = 0.5f;
    [SerializeField] int _startTargetPoint;

    int _targetPosition;

    int _targetOnRotate = 1000;

    

    void Awake()
    {
        _targetPosition = _startTargetPoint;
        
    }

    void Update()
    {

        transform.position = Vector2.MoveTowards(transform.position, _positions[_targetPosition].position, _speed * Time.deltaTime);
        CheckDistance();

        if (transform.position == _positions[_targetPosition].position)
        {            
            _targetPosition++;
            if (_targetPosition == _positions.Length)
                _targetPosition = 0;

        }

    }

    void CheckDistance()
    {
        float distance = (transform.position - _positions[_targetPosition].position).magnitude;
        
        if (distance < _dstBeforeEdgeStartRotate && _targetOnRotate != _targetPosition)
        {
            StartCoroutine(RotateOverTime());
            _targetOnRotate = _targetPosition;

        }

    }

    IEnumerator RotateOverTime()
    {
        Quaternion start = transform.rotation;
        Quaternion end = _positions[_targetPosition].rotation;        

        float fraction = 0.0f;
        while (fraction < 1.0f)
        {
            fraction += Time.deltaTime * _cornerSpeed;
            transform.rotation = Quaternion.Lerp(start, end , fraction);
            yield return null;
        }

    }

}
