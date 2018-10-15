using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexagonMapEditor : MonoBehaviour {

    public Color[] colors;
    public Grid hexGrid;
    private Color activeColor;

    public StartUnit unitPrefab;

    public StartUnit unit;
    public bool unitAlive = false;
    public HexagonCell unitCell; // cell the unit is on

    HexagonCell previousCell;

	// Use this for initialization
	void Awake () {
        SelectColor(0);
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!EventSystem.current.IsPointerOverGameObject())
        {
            if(Input.GetMouseButton(0))
            {
                HandleInput();
                return;
            }
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            CreateUnit();
            return;
        }
        previousCell = null;
	}

    void HandleInput()
    {
        HexagonCell currentCell = GetCellUnderCursor();
        int index = currentCell.coords.X_coord + currentCell.coords.Z_coord * hexGrid.width + currentCell.coords.Z_coord / 2;
        if(unitAlive)
        {
            MoveUnit(index);
        }
        //if (currentCell)
        //{

        //}
        //else
        //{
        //    previousCell = null;
        //}
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

    HexagonCell GetCellUnderCursor()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            return hexGrid.GetCell(hit.point);
        }
        return null;
    }

    void CreateUnit()
    {
        //HexagonCell cell = GetCellUnderCursor();
        //if (cell)
        //{
        //    StartUnit unit = Instantiate(unitPrefab);
        //    unit.transform.SetParent(hexGrid.transform, false);
        //}
        if (!unitAlive)
        {
            Debug.Log("create boi");
            unit = Instantiate(unitPrefab);
            unitAlive = true;
            unitCell = hexGrid.cells[0];
            unit.transform.position = hexGrid.cells[0].transform.position;
        }
    }

    void MoveUnit(int index)
    {
        int distance = unitCell.coords.FindDistanceTo(hexGrid.cells[index].coords);
        //Debug.Log("Distance From: " + unitCell.coords.ToString() + " To: " +
        //hexGrid.cells[index].coords.ToString() +
        //" = " + distance.ToString()); //for debugging distance
        if (unit.mobility >= distance)
        {
            unit.transform.position = hexGrid.cells[index].transform.position;
            unitCell = hexGrid.cells[index];
        }
        else
        {
            Debug.LogError("CAN'T MOVE THATS TOO FAR FOR THE UNIT");
        }
    }
}
