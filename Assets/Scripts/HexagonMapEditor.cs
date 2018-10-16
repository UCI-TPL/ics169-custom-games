using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexagonMapEditor : MonoBehaviour {

    public Color[] colors;
    public Grid hexGrid;
    private Color activeColor;

    public StartUnit unitPrefab;

    public StartUnit SelectedUnit;
    public bool isUnitSelected = false;
    public HexagonCell unitCell; // cell the unit is on

    HexagonCell previousCell;

	// Use this for initialization
	void Awake () {
		
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
            if(Input.GetMouseButton(1))
            {
                DeselectUnit();
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
        if(currentCell.occupied) // if clicked and there is a unit there
        {
            SelectUnit(currentCell, index); // make the selected unit that unit
        }
        else if(!currentCell.occupied && isUnitSelected) // a unit is already selected
        {
            MoveUnit(index);//move that selected unit
        }

    }

    void SelectUnit(HexagonCell current, int index)
    {
        SelectedUnit = current.unitOnTile;
        unitCell = hexGrid.cells[index];
        isUnitSelected = true;
        hexGrid.ShowPath(unitCell, SelectedUnit.mobility);
    }

    void DeselectUnit()
    {
        SelectedUnit = null;
        unitCell = null;
        isUnitSelected = false;
        hexGrid.ClearPath();
    }

    //public void ShowPath(HexagonCell current, int mobility)
    //{
    //    for(int i =  0; i < (hexGrid.width * hexGrid.height); i++)
    //    {
    //        if(current.coords.FindDistanceTo(hexGrid.cells[i].coords) <= mobility)
    //        {
    //            hexGrid.cells[i].color = hexGrid.touchedColor;
    //        }
    //    }
    //    hexGrid.
    //}

    //public void SelectColor(int index)
    //{
    //    activeColor = colors[index];
    //}

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
        if (!isUnitSelected) // only create units when not selecting one
        {
            SelectedUnit = Instantiate(unitPrefab);
            isUnitSelected = true;
            unitCell = hexGrid.cells[0];
            SelectedUnit.transform.position = hexGrid.cells[0].transform.position;
            unitCell.occupied = true;
            unitCell.unitOnTile = SelectedUnit;
        }
    }

    void MoveUnit(int index)
    {
        int distance = unitCell.coords.FindDistanceTo(hexGrid.cells[index].coords);
        //Debug.Log("Distance From: " + unitCell.coords.ToString() + " To: " +
        //hexGrid.cells[index].coords.ToString() +
        //" = " + distance.ToString()); //for debugging distance
        if (SelectedUnit.mobility >= distance)
        {
            unitCell.occupied = false;
            unitCell.unitOnTile = null;
            SelectedUnit.transform.position = hexGrid.cells[index].transform.position;
            unitCell = hexGrid.cells[index];
            hexGrid.cells[index].occupied = true;
            hexGrid.cells[index].unitOnTile = SelectedUnit;
        }
        else
        {
            Debug.LogError("CAN'T MOVE THATS TOO FAR FOR THE UNIT");
        }
    }
}
