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
        Debug.Log(index);
        if(unitAlive)
        {
            unit.transform.position = hexGrid.cells[index].transform.position;
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
            unit.transform.position = hexGrid.cells[0].transform.position;
        }
    }
}
