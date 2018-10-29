using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    public GameObject point;
    public HexagonCoord coords;
    public Grid _Grid;
    private float time = 0.0f;
    public float time_increment = 0.5f;

	// Use this for initialization
	void Start () {
        point = GameObject.Find("Point");
        coords.x = -3;
        coords.z = 3;
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(Input.GetButtonDown("J1 X Button"));
        //transform.position += new Vector3(Input.GetAxis("J1 Left Horizontal"), -Input.GetAxis("J1 Left Vertical"), 0) * Time.deltaTime * 90;
        float H_Axis = Input.GetAxis("J1 Left Horizontal");
        float V_Axis = Input.GetAxis("J1 Left Vertical");

        if (H_Axis >= 0.8f && V_Axis <= 0.2 && V_Axis >= -0.2 && Time.time >= time)
        {
            _Move("x", 1);
            time = Time.time + time_increment;
        }

        if (H_Axis <= -0.8f && V_Axis <= 0.2 && V_Axis >= -0.2 && Time.time >= time)
        {
            _Move("x", -1);
            time = Time.time + time_increment;
        }

        if (H_Axis >= 0.5f && V_Axis >= 0.5 && Time.time >= time)
        {
            _Move("z", 1);
            time = Time.time + time_increment;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            _Move("z", 1);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            _Move("x", 1);
        }

        if (Input.GetButtonDown("Jump"))
        {
            _Move("y", 1);
        }

    }

    private void _Move(string dir, int sign) {
        if (dir.Equals("z"))
        {
            coords.z += sign;
        }

        if (dir.Equals("x"))
        {
            coords.x += sign;
        }

        if (dir.Equals("y"))
        {
            coords.x -= sign;
            coords.z += sign;
        }

        gameObject.transform.position = _Grid.Get_Cell_Index(coords).gameObject.transform.position;

        Debug.Log("Current X " + coords.X_coord);
        Debug.Log("Current Y " + coords.Y_coord);
        Debug.Log("Current Z " + coords.Z_coord);
        Debug.Log(_Grid.Get_Cell_Index(coords).gameObject.transform.position);
    }
}

