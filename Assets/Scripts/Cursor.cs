using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public GameObject point;
    public HexagonCoord coords;
    public HexagonCoord prev_coords;
    public Grid _Grid;
    private float time = 0.0f;
    public float time_increment = 0.5f;
    private bool cascade_dir;
    public HexagonMapEditor editor;
    public int original_sorting_value;

    // Use this for initialization
    void Start()
    {
        point = GameObject.Find("Point");
        coords.x = 0;
        coords.z = 0;
        cascade_dir = false;
        //original_sorting_value = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        //Debug.Log("--------------- " + original_sorting_value);
        Order_Cursor(_Grid.GetCell(transform.position).coords, _Grid.sprites_per_tile);
    }

    // Update is called once per frame
    void Update()
    {
        if (editor.allow_cursor_control)
        {
            float H_Axis;
            float V_Axis;

            if (editor.currentState == HexagonMapEditor.TurnStates.P1_MOVE)
            {
                H_Axis = Input.GetAxis(editor.PlayerInfo.player1 + "Left Horizontal");
                V_Axis = Input.GetAxis(editor.PlayerInfo.player1 + "Left Vertical");
            }
            else if (editor.PlayerInfo.one_player)
            {
                H_Axis = Input.GetAxis(editor.PlayerInfo.player1 + "Left Horizontal");
                V_Axis = Input.GetAxis(editor.PlayerInfo.player1 + "Left Vertical");
            }
            else
            {
                H_Axis = Input.GetAxis(editor.PlayerInfo.player2 + "Left Horizontal");
                V_Axis = Input.GetAxis(editor.PlayerInfo.player2 + "Left Vertical");
            }

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
                        //Debug.Log("Down");
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
        //Debug.Log(Input.GetButtonDown("J1 X Button"));
        //transform.position += new Vector3(Input.GetAxis("J1 Left Horizontal"), -Input.GetAxis("J1 Left Vertical"), 0) * Time.deltaTime * 90;
    }

    private void _Move(string dir, int sign)
    {
        //Deal with indexing errors by checking len of cell array.
        //When going up or down on the stick just go in the y then z.

        prev_coords = coords;

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
        catch (System.IndexOutOfRangeException e)
        {
            coords.x = prev_coord_x;
            coords.z = prev_coord_z;
            Debug.Log(e.Message);
        }


        if (editor.isUnitSelected)
        {
            editor.Show_Units_In_Range();
            //add to if statement if you dont want the same unit to pop up in both
            //&& editor.SelectedUnit != _Grid.Get_Cell_Index(coords).unitOnTile
            if (_Grid.Get_Cell_Index(coords).unitOnTile != null)
            {
                //if(editor.allow_cursor_control)
                StartUnit _tileUnit = _Grid.Get_Cell_Index(coords).unitOnTile;

                BattleUI _tileUnit_UI = _tileUnit.Unit_Stats_Panel.GetComponent<BattleUI>();
                editor.Assign_Stats_Var(_tileUnit_UI, _tileUnit);
                _tileUnit_UI.Show();

            }
        }
        else
        {
            if (_Grid.Get_Cell_Index(coords).unitOnTile != null)
            {
                StartUnit _tileUnit = _Grid.Get_Cell_Index(coords).unitOnTile;

                BattleUI _tileUnit_UI = _tileUnit.Unit_Stats_Panel.GetComponent<BattleUI>();
                editor.Assign_Stats_Var(_tileUnit_UI, _tileUnit);
                _tileUnit_UI.Show();

            }
        }

        Hide_Prev_UI();

        Order_Cursor(_Grid.GetCell(transform.position).coords, _Grid.sprites_per_tile);


        //Debug.Log("Current X " + coords.X_coord);
        //Debug.Log("Current Y " + coords.Y_coord);
        //Debug.Log("Current Z " + coords.Z_coord);
        //Debug.Log(_Grid.Get_Cell_Index(coords).gameObject.transform.position);
    }

    public void Assign_Position(Vector3 _new_position, HexagonCoord _new_coord)
    {
        gameObject.transform.position = _new_position;
        prev_coords = coords;
        coords = _new_coord;
        Hide_Prev_UI();
        if(editor.allow_cursor_control == true)
        {
            StartUnit _tileUnit = _Grid.Get_Cell_Index(coords).unitOnTile;
            BattleUI _tileUnit_UI = _tileUnit.Unit_Stats_Panel.GetComponent<BattleUI>();
            editor.Assign_Stats_Var(_tileUnit_UI, _tileUnit);
            _tileUnit_UI.Show();
        }
        else
        {
            Hide_Prev_UI();
        }
        Order_Cursor(_Grid.GetCell(transform.position).coords, _Grid.sprites_per_tile);
    }

    public void Order_Cursor(HexagonCoord _coord, int num_sprites_per_cell)
    {
        int _current_sorting_order = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = original_sorting_value +
            ((_coord.X_coord + _coord.Y_coord) * num_sprites_per_cell);
        Debug.Log(original_sorting_value +
            ((_coord.X_coord + _coord.Y_coord) * num_sprites_per_cell));
    }

        private void Hide_Prev_UI()
    {
        if (_Grid.Get_Cell_Index(prev_coords).unitOnTile != null)
        {
            StartUnit _tileUnit = _Grid.Get_Cell_Index(prev_coords).unitOnTile;
            BattleUI _tileUnit_UI = _tileUnit.Unit_Stats_Panel.GetComponent<BattleUI>();
            _tileUnit_UI.Hide();
        }
    }
}

