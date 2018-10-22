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
        //Debug.Log(Input.GetButtonDown("J1 X Button"));
        transform.position += new Vector3(Input.GetAxis("J1 Left Horizontal"), -Input.GetAxis("J1 Left Vertical"), 0) * Time.deltaTime * 90;

	}
}

