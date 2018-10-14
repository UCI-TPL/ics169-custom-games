using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Grid class sets up the board of our game.  It allows for initializatiton of the width
// height of the board.  Here is where the tile coordinates and placement is done
public class Grid : MonoBehaviour {

    // variables
    public int width = 6;
    public int height = 6;

    // prefabs cell and cellLabel should be children of grid
    public HexagonCell cellPrefab;

    public Text cellLabelPrefab;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    HexagonCell[] cells;
    Canvas gridCanvas;
    HexagonMesh hexMesh;

	// Use this for initialization
	void Awake () {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexagonMesh>();
        
        cells = new HexagonCell[height * width]; // create an array of correct length

        for(int b = 0,c = 0; b < height; b++) // fill the array with actual hexagon cells
        {
            
            for(int a = 0; a < width; a++)
            {
                CreateCell(a, b, c++);
            }
        }
	}

    private void Start() // runs after awake()
    {
        hexMesh.Triangulate(cells);
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
        cell.color = defaultColor;

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        label.text = cell.coords.ToStringSeparateLines();

    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
	}

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            TouchCell(hit.point);
        }
    }

    void TouchCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexagonCoord coordinates = HexagonCoord.FromPosition(position);
        int index = coordinates.X_coord + coordinates.Z_coord * width + coordinates.Z_coord / 2;
        HexagonCell cell = cells[index];
        cell.color = touchedColor;
        hexMesh.Triangulate(cells);
        Debug.Log("touched at " + coordinates.ToString());
        Debug.Log("touched at " + position);
    }
}
