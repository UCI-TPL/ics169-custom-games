using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour {

    public int width = 6;
    public int height = 6;

    public HexagonCell cellPrefab;

    HexagonCell[] cells;

    public Text cellLabelPrefab;

    Canvas gridCanvas;

    HexagonMesh hexMesh;

	// Use this for initialization
	void Awake () {
        gridCanvas = GetComponentInChildren<Canvas>();

        hexMesh = GetComponentInChildren<HexagonMesh>();
        
        cells = new HexagonCell[height * width];

        for(int b = 0,c = 0; b < height; b++)
        {
            
            for(int a = 0; a < width; a++)
            {
                CreateCell(a, b, c++);
            }
        }
	}

    private void Start()
    {
        hexMesh.Triangulate(cells);
    }

    void CreateCell(int a, int b, int c)
    {
        Vector3 position;
        position.x = (a + b * 0.5f - b / 2) * (HexagonInfo.innerRadius * 2f);
        position.y = b * (HexagonInfo.outerRadius * 1.5f);
        position.z = 0f;

        HexagonCell cell = cells[c] = Instantiate<HexagonCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coords = HexagonCoord.FromOffsetCoordinates(a, b);

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        label.text = cell.coords.ToStringSeparateLines();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
