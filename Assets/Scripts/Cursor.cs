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
        coords.x = 0;
        coords.z = 0;
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(Input.GetButtonDown("J1 X Button"));
        //transform.position += new Vector3(Input.GetAxis("J1 Left Horizontal"), -Input.GetAxis("J1 Left Vertical"), 0) * Time.deltaTime * 90;
        float H_Axis = Input.GetAxis("J1 Left Horizontal");
        float V_Axis = Input.GetAxis("J1 Left Vertical");

        if (H_Axis >= 0.8f && V_Axis <= 0.2f && V_Axis >= -0.2f && Time.time >= time)
        {
            _Move("x", 1);
            time = Time.time + time_increment;
            Debug.Log("Up_X");
        }
        else if (H_Axis <= -0.8f && V_Axis <= 0.2f && V_Axis >= -0.2f && Time.time >= time)
        {
            _Move("x", -1);
            time = Time.time + time_increment;
            Debug.Log("Down_X");
        }
        else if (H_Axis >= 0.4f && V_Axis <= -0.4f && Time.time >= time)
        {
            _Move("z", 1);
            time = Time.time + time_increment;
            Debug.Log("Up_Z");
        }
        else if (H_Axis <= -0.4f && V_Axis >= 0.4f && Time.time >= time)
        {
            _Move("z", -1);
            time = Time.time + time_increment;
            Debug.Log("Up_Z");
        }
        else if (H_Axis <= -0.4f && V_Axis <= -0.4f && Time.time >= time)
        {
            _Move("y", 1);
            time = Time.time + time_increment;
            Debug.Log("Up_Z");
        }
        else if (H_Axis >= 0.4f && V_Axis >= 0.4f && Time.time >= time)
        {
            _Move("y", -1);
            time = Time.time + time_increment;
            Debug.Log("Up_Z");
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
        //Deal with indexing errors by checking len of cell array.
        //When going up or down on the stick just go in the y then z.

        int prev_coord_x = coords.X_coord;
        int prev_coord_z = coords.Z_coord;

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

        HexagonCoord next_cell;
        try
        {
            next_cell = _Grid.Get_Cell_Index(coords).coords;

            if (coords.X_coord == next_cell.X_coord && coords.Z_coord == next_cell.Z_coord)
            {
                //It found a real tile.
                gameObject.transform.position = _Grid.Get_Cell_Index(coords).gameObject.transform.position;
            }
            else
            {
                //It found a not-real tile. Revert Change.
                coords.x = prev_coord_x;
                coords.z = prev_coord_z;
            }
        }
        catch(System.IndexOutOfRangeException e)
        {
            coords.x = prev_coord_x;
            coords.z = prev_coord_z;
            Debug.Log(e.Message);
        }
           

        Debug.Log("Current X " + coords.X_coord);
        Debug.Log("Current Y " + coords.Y_coord);
        Debug.Log("Current Z " + coords.Z_coord);
        Debug.Log(_Grid.Get_Cell_Index(coords).gameObject.transform.position);
    }
}

