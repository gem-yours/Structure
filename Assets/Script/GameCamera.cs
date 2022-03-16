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

    public Vector3 offset = Vector3.zero;
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
        var correctedOffset = offset / 20;
        if(correctedOffset.magnitude > offsetSize) {
            correctedOffset = correctedOffset.normalized * offsetSize;
        }

        var newPosition = target.transform.position + initialDistance + correctedOffset;
        transform.position = Vector3.MoveTowards(transform.position, newPosition, speed);        
    }
}
