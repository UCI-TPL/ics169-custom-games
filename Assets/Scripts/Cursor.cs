using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    public GameObject point;
    public HexagonCoord coords;



	// Use this for initialization
	void Start () {
        point = GameObject.Find("Point");
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(Input.GetButtonDown("J1 X Button"));
        transform.position += new Vector3(Input.GetAxis("J1 Left Horizontal"), -Input.GetAxis("J1 Left Vertical"), 0) * Time.deltaTime * 90;

        if (Input.GetButtonDown("Fire1"))
        {
            _Move("z", 1);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            _Move("x", 1);
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
        Debug.Log("Current X " + coords.X_coord);
        Debug.Log("Current Y " + coords.Y_coord);
        Debug.Log("Current Z " + coords.Z_coord);
    }
}

