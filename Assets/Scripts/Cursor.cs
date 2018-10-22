using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    public GameObject point;

	// Use this for initialization
	void Start () {
        point = GameObject.Find("Point");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        pos.x += Input.GetAxis("J1_X");
        pos.y += Input.GetAxis("J1_Y");
        transform.position = pos;
	}
}
