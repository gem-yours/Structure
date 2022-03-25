using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public GameObject target
    {
        get
        {
            return _target;
        }
        set 
        {
            _target = value;
            initialDistance = transform.position - target.transform.position;
        }
    }
    private GameObject _target;

    public Vector3 offset
    {
        get
        {
            return _offset;
        }
        set
        {
            // 1秒かけてカメラ位置が最遠になるように
            _offset += value.normalized / offsetSize;
             if (_offset.magnitude > offsetSize) {
                 _offset = _offset.normalized * offsetSize;
            }
        }
    }
    private Vector3 _offset = Vector3.zero;
    private Vector3 initialDistance;
    private float offsetSize = 3;
    private float speed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) {
            return;
        }

        // offsetによってカメラ位置をずらすことで移動を表現したい
        var newPosition = target.transform.position + initialDistance + _offset;
        transform.position = Vector3.MoveTowards(transform.position, newPosition, speed);        
    }
}
