using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupText : MonoBehaviour {
    public float DestroyTime;
    Vector3 startPosition;
    public AnimationClip animator;
    // Use this for initialization
    void Start () {
        startPosition = transform.localPosition;
        DestroyTime = animator.length;
    }
	
	// Update is called once per frame
	void Update () {
        Destroy(gameObject, DestroyTime);
    }
    private void LateUpdate()
    {
        transform.localPosition += startPosition;
    }
}
