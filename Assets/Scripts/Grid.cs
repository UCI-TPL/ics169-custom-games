﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Grid class sets up the board of our game.  It allows for initializatiton of the width
// height of the board.  Here is where the tile coordinates and placement is done
public class Grid : MonoBehaviour {

    // variables
    public int width = 20;
    public int height = 20;
    public Sprite sprite, TeamBuffTile;
    public HexagonCell power_up_prefab;
    public Transform GridMap;

    // prefabs cell and cellLabel should be children of grid
    public HexagonCell cellPrefab;

    public Text cellLabelPrefab;

    public Color defaultColor = Color.gray;
    public Color touchedColor = Color.cyan;
    public Color attackColor = Color.green;

    public HexagonCell[] cells;
    Canvas gridCanvas;
    HexagonMesh hexMesh;

	// Use this for initialization
	void Awake () {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexagonMesh>();
        CreateGrid();
    }

    private void Start() // runs after awake()
    {
        //hexMesh.Triangulate(cells);
    }

    void CreateCell(int a, int b, int c) // should only be called once when initializing the map 
    {
        Vector3 position;
        position.x = (a + b * 0.5f - b / 2) * (HexagonInfo.innerRadius * 2f);
        position.y = b * (HexagonInfo.outerRadius * 1.5f);
        position.z = 0f;

        HexagonCell cell = cells[c] = Instantiate<HexagonCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coords = HexagonCoord.FromOffsetCoordinates(a, b);
        cell.spriteRenderer.color = defaultColor;
        //cell.color = defaultColor;

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        label.text = cell.coords.ToStringSeparateLines();

    }


    public void ShowPath(HexagonCell current, int mobility, int range,Color color_m, Color color_a)
    {
        for (int i = 0; i < (width * height); i++)
        {
            if (cells[i].gameObject.tag != "Wall")
            {
                if (current.coords.FindDistanceTo(cells[i].coords) <= mobility)
                {
                    cells[i].spriteRenderer.color = color_m;
                }
                else if (current.coords.FindDistanceTo(cells[i].coords) <= mobility + range)
                {
                    cells[i].spriteRenderer.color = color_a;
                }
                else
                {
                    cells[i].spriteRenderer.color = defaultColor;
                }
            }

        }
    }

    public void ClearPath()
    {
        for (int i = 0; i < (width * height); i++)
        {
            cells[i].spriteRenderer.color = defaultColor;
        }
    }

    void TouchCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexagonCoord coordinates = HexagonCoord.FromPosition(position);
        int index = coordinates.X_coord + coordinates.Z_coord * width + coordinates.Z_coord / 2;
        Debug.Log(index);
        HexagonCell cell = cells[index];
        cell.color = touchedColor;
        hexMesh.Triangulate(cells);
    }

    public HexagonCell GetCell (Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexagonCoord coordinates = HexagonCoord.FromPosition(position);
        int index = coordinates.X_coord + coordinates.Z_coord * width + coordinates.Z_coord / 2;
        return cells[index];
        
    }

    public HexagonCell[] CreateGrid()
    {
        cells = new HexagonCell[height * width]; // create an array of correct length

        for (int b = 0, c = 0; b < height; b++) // fill the array with actual hexagon cells
        {

            for (int a = 0; a < width; a++)
            {
                CreateCell(a, b, c++);
            }
        }
        List<int> hex_list = CreateList();
        List<int> power_ups = PowerUpList();
        HexagonCell[] result = ChangeHexInfo(cells, hex_list, power_ups);
        return cells;
    }

    public HexagonCell[] ChangeHexInfo(HexagonCell[] cells_, List<int> hexlist_, List<int> powercells_)
    {
        if (hexlist_.Count != 0)
        {
            for (int i = 0; i < hexlist_.Count; i++)
            {
                cells_[hexlist_[i]].gameObject.tag = "Wall";
                cells_[hexlist_[i]].gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                cells_[hexlist_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
                //cells_[hexlist_[i]].gameObject.SetActive(false);
            }
        }
        if(powercells_.Count != 0)
        {
            for(int i = 0; i < powercells_.Count; i++)
            {
                cells_[powercells_[i]].gameObject.AddComponent<TeamPowerupTiles>();
                cells_[powercells_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = TeamBuffTile;
                cells_[powercells_[i]].tag = "TeamBuff";
                //HexagonCell newobject = Instantiate(power_up_prefab, cells_[powercells_[i]].transform);
                //newobject.GetComponent<HexagonCell>().coords = cells_[powercells_[i]].GetComponent<HexagonCell>().coords;
                //cells_[powercells_[i]].transform.DetachChildren();
                //cells_[powercells_[i]].gameObject.SetActive(false);
                //newobject.transform.SetParent(GridMap);

              //  cells_[powercells_[i]].gameObject.SetActive(false);

            }
        }
        return cells_;
    }

    public List<int> CreateList()
    {//list of wall tiles
        List<int> hex_list = new List<int>();
       // hex_list.Add(18);
        hex_list.Add(23);
        hex_list.Add(24);
        hex_list.Add(36);
        hex_list.Add(42);
        hex_list.Add(47);
        hex_list.Add(52);
        hex_list.Add(57);
        hex_list.Add(63);
        hex_list.Add(75);
        hex_list.Add(76);
        //hex_list.Add(81);
        return hex_list;
    }
    public List<int> PowerUpList()
    {
        List<int> powerups = new List<int>();
        powerups.Add(18);
        powerups.Add(81);
        return powerups;
    }
}
