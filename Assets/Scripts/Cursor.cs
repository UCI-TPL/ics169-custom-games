﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    public GameObject point;
    public HexagonCoord coords;
    public Grid _Grid;
    private float time = 0.0f;
    public float time_increment = 0.5f;
    private bool cascade_dir;
    public HexagonMapEditor editor;

	// Use this for initialization
	void Start () {
        point = GameObject.Find("Point");
        coords.x = 0;
        coords.z = 0;
        cascade_dir = false;
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(Input.GetButtonDown("J1 X Button"));
        //transform.position += new Vector3(Input.GetAxis("J1 Left Horizontal"), -Input.GetAxis("J1 Left Vertical"), 0) * Time.deltaTime * 90;
        float H_Axis = Input.GetAxis("J1 Left Horizontal");
        float V_Axis = Input.GetAxis("J1 Left Vertical");

        if (Time.time >= time)
        {
            
            if ((Mathf.Pow(H_Axis, 2) + Mathf.Pow(V_Axis, 2)) <= 0.08f)
            {
                //Dead Zone
            }
            else
            {
                float Angle = Mathf.Atan2(H_Axis, V_Axis) * Mathf.Rad2Deg;
                //Debug.Log(Angle);
                //0 -> 180 (right)   0 -> -180 (left)

                if (Angle > 67.5 && Angle < 112.5)
                {
                    _Move("x", 1);
                    time = Time.time + time_increment;
                }
                else if (Angle < -67.5 && Angle > -112.5)
                {
                    _Move("x", -1);
                    time = Time.time + time_increment;
                }
                else if (Angle < 157.5 && Angle > 112.5)
                {
                    _Move("z", 1);
                    time = Time.time + time_increment;
                }
                else if (Angle > -157.5 && Angle < -112.5)
                {
                    _Move("y", 1);
                    time = Time.time + time_increment;
                }
                else if (Angle > -67.5 && Angle < -22.5)
                {
                    _Move("z", -1);
                    time = Time.time + time_increment;
                }
                else if (Angle < 67.5 && Angle > 22.5)
                {
                    _Move("y", -1);
                    time = Time.time + time_increment;
                }
                else if (Angle < -157.5 || Angle > 157.5)
                {
                    //cascade up
                    if (cascade_dir)
                    {
                        _Move("z", 1);
                        time = Time.time + time_increment;
                        cascade_dir = false;
                    }
                    else
                    {
                        _Move("y", 1);
                        time = Time.time + time_increment;
                        cascade_dir = true;
                    }
                }
                else if (Angle > -22.5 && Angle < 22.5)
                {
                    Debug.Log("Down");
                    //cascade down
                    if (cascade_dir)
                    {
                        _Move("y", -1);
                        time = Time.time + time_increment;
                        cascade_dir = false;
                    }
                    else
                    {
                        _Move("z", -1);
                        time = Time.time + time_increment;
                        cascade_dir = true;
                    }
                }
            }
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
        HexagonCell next_hex_cell;
        try
        {
            next_cell = _Grid.Get_Cell_Index(coords).coords;
            next_hex_cell = _Grid.Get_Cell_Index(coords);

            if (coords.X_coord == next_cell.X_coord && coords.Z_coord == next_cell.Z_coord)
            {
                //It found a real tile.
                if (editor.isUnitSelected)
                {
                    if (editor.Is_Tile_In_Move_Range())
                    {
                        gameObject.transform.position = _Grid.Get_Cell_Index(coords).gameObject.transform.position;
                    }
                    else
                    {
                        coords.x = prev_coord_x;
                        coords.z = prev_coord_z;
                    }
                }
                else
                {
                    gameObject.transform.position = _Grid.Get_Cell_Index(coords).gameObject.transform.position;
                }
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


        if (editor.isUnitSelected)
        {
            editor.Show_Units_In_Range();
            if (_Grid.Get_Cell_Index(coords).unitOnTile != null && editor.SelectedUnit != _Grid.Get_Cell_Index(coords).unitOnTile)
            {
                StartUnit _tileUnit = _Grid.Get_Cell_Index(coords).unitOnTile;
                if (_tileUnit.CompareTag("Player 1"))
                {
                    editor.Assign_BUI_Var(editor.BattleUI_P1_Hover, _tileUnit);
                    editor.BattleUI_P1_Hover.Show();
                    editor.BattleUI_P2_Hover.Hide();
                }
                else
                {
                    editor.Assign_BUI_Var(editor.BattleUI_P2_Hover, _tileUnit);
                    editor.BattleUI_P2_Hover.Show();
                    editor.BattleUI_P1_Hover.Hide();
                }

            }
            else
            {
                editor.BattleUI_P1_Hover.Hide();
                editor.BattleUI_P1_Hover.Hide();
            }
        }
        else
        {
            if (_Grid.Get_Cell_Index(coords).unitOnTile != null)
            {
                StartUnit _tileUnit = _Grid.Get_Cell_Index(coords).unitOnTile;
                if (_tileUnit.CompareTag("Player 1"))
                {
                    editor.Assign_BUI_Var(editor.BattleUI_P1, _tileUnit);
                    editor.BattleUI_P1.Show();
                    editor.BattleUI_P2.Hide();
                }
                else
                {
                    editor.Assign_BUI_Var(editor.BattleUI_P2, _tileUnit);
                    editor.BattleUI_P2.Show();
                    editor.BattleUI_P1.Hide();
                }

            }
            else
            {
                editor.BattleUI_P1.Hide();
                editor.BattleUI_P1.Hide();
            }
        }

        

        //Debug.Log("Current X " + coords.X_coord);
        //Debug.Log("Current Y " + coords.Y_coord);
        //Debug.Log("Current Z " + coords.Z_coord);
        //Debug.Log(_Grid.Get_Cell_Index(coords).gameObject.transform.position);
    }
}

