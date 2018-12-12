using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounder : MonoBehaviour {
    public Transform cursor;
    public float bound_x;
    public float bound_y;
    public float lerp_speed;
    private Vector3 desiredPos;
    public float main_camera_size;
    public float duration = 0.1f;
    public float speed = 20f;
    public float magnitude = 2f;
    public AnimationCurve damper = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.9f, .33f, -2f, -2f), new Keyframe(1f, 0f, -5.65f, -5.65f));
    public bool testPosition = false;
    private Vector3 originalPos;
    private Vector3 current_cam_pos;

    // Use this for initialization
    private void Start()
    {
        main_camera_size = Camera.main.orthographicSize;
        originalPos = transform.localPosition;
    }

    public void Change_Zoom(float new_camera_size)
    {
        Camera.main.orthographicSize = new_camera_size;
    }

    public void Reset_Zoom()
    {
        Camera.main.orthographicSize = main_camera_size;
    }

    public void Lerp_Reset_Zoom()
    {
        Camera.main.orthographicSize = (float)Mathf.Lerp(Camera.main.orthographicSize, main_camera_size, 1.5f);
    }

    public void Shake_Camera(float magnitude, float speed)
    {
        StopAllCoroutines();
        StartCoroutine(ShakePosition(transform, duration, speed, magnitude, damper));
    }

    public void Lerp_Change_Zoom(float new_camera_size)
    {
        Camera.main.orthographicSize = (float)Mathf.Lerp(Camera.main.orthographicSize, new_camera_size, 1.5f);
    }

    IEnumerator ShakePosition(Transform transform, float duration, float speed, float magnitude, AnimationCurve damper = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float damperedMag = (damper != null) ? (damper.Evaluate(elapsed / duration) * magnitude) : magnitude;
            float x = (Mathf.PerlinNoise(Time.time * speed, 0f) * damperedMag) - (damperedMag / 2f);
            float y = (Mathf.PerlinNoise(0f, Time.time * speed) * damperedMag) - (damperedMag / 2f);
            transform.localPosition = new Vector3(current_cam_pos.x + x, current_cam_pos.y + y, current_cam_pos.z);
            yield return null;
        }
        transform.localPosition = current_cam_pos;
    }

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
        current_cam_pos = desiredPos;
        transform.position = Vector3.Lerp(transform.position,desiredPos,lerp_speed);
    }
}
