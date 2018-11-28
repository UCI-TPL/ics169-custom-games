using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounder : MonoBehaviour {
    public Transform cursor;
    public float bound_x;
    public float bound_y;
    public float lerp_speed;
    private Vector3 desiredPos;

    // Use this for initialization
    private void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        float dx = cursor.position.x - transform.position.x;
        if(dx > bound_x || dx < -bound_x)
        {
            if(transform.position.x < cursor.position.x)
            {
                delta.x = dx - bound_x;
            }
            else
            {
                delta.x = dx + bound_x;
            }
        }

        float dy = cursor.position.y - transform.position.y;
        if (dy > bound_y || dy < -bound_y)
        {
            if (transform.position.y < cursor.position.y)
            {
                delta.y = dy - bound_y;
            }
            else
            {
                delta.y = dy + bound_y;
            }
        }

        desiredPos = transform.position + delta;
        transform.position = Vector3.Lerp(transform.position,desiredPos,lerp_speed);
    }
}
