using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Grid class sets up the board of our game.  It allows for initializatiton of the width
// height of the board.  Here is where the tile coordinates and placement is done
public class Grid : MonoBehaviour {

    // variables
    public int width = 10;
    public int height = 10;
    public Sprite Wall, AttackBuff, Healthbuff, MobilityBuff, CritBuff, AttackRangebuff, SlowingTile, Water, PoweredDown;

    //Grid Details
    HexagonCell[] result;
    List<int> wall_list1 = new List<int>() { 23, 24, 36, 42, 47, 52, 57, 63, 75, 76 };

    List<int> wall_list2 = new List<int>() { 18, 23, 24, 36, 42, 47, 52, 57, 63, 69,
                                             75, 76, 81 , 89, 90, 103, 115, 123, 136,
                                             163, 181, 196, 197, 202, 203, 218, 236,
                                             263, 275, 284, 295, 309, 310, 318, 323,
                                             324, 330, 335, 342, 347, 352, 357, 363,
                                             375, 376, 381};

    List<int> water2 = new List<int> {    147, 148, 149, 150, 151,
                                        167, 168, 169, 170, 171, 172,
                                      186, 187, 188, 190, 191, 192,
                                        207, 208, 209, 210, 211, 212,
                                          227, 228, 229, 230, 231};

    List<int> powerlist1 = new List<int>() { 18, 81 };
    List<int> powerlist2 = new List<int>() { 182, 189, 217};

    List<int> hazards2 = new List<int> { 37, 58, 77, 78, 127, 128, 129,
                                         130, 131, 132, 146, 152, 162,
                                         166, 173, 183, 185, 193, 206,
                                         213, 216, 226, 232, 237, 247,
                                         248, 249, 250, 251, 252,
                                         321, 322, 341,362};
    List<int> hazards = new List<int>() { };
    List<int> water = new List<int>() { };

    // prefabs cell and cellLabel should be children of grid
    public HexagonCell cellPrefab;

    //public Text cellLabelPrefab;

    public Color defaultColor = Color.white;
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
        cell.tag = "Floor";
        //cell.color = defaultColor;

        //Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform);
        //label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        //label.text = cell.coords.ToStringSeparateLines();

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
        //int randmap = Random.Range(0, 2);
        //if (randmap == 0)
        //{
        height = 10;
        width = 10;
        cells = new HexagonCell[height * width]; // create an array of correct length

        for (int b = 0, c = 0; b < height; b++) // fill the array with actual hexagon cells
        {

            for (int a = 0; a < width; a++)
            {
                CreateCell(a, b, c++);
            }
        }
        result = ChangeHexInfo(cells, wall_list1, powerlist1, hazards, water);
        //}

        //if(randmap == 1)
        //{
        //height = 20;
        //width = 20;
        //cells = new HexagonCell[height * width]; // create an array of correct length

        //for (int b = 0, c = 0; b < height; b++) // fill the array with actual hexagon cells
        //{

        //    for (int a = 0; a < width; a++)
        //    {
        //        CreateCell(a, b, c++);
        //    }
        //}
        //result = ChangeHexInfo(cells, wall_list2, powerlist2, hazards2, water2);
        //}

        //if(randmap == 2)
        //{
        //    height = 30;
        //    width = 30;
        //    cells = new HexagonCell[height * width]; // create an array of correct length

        //    for (int b = 0, c = 0; b < height; b++) // fill the array with actual hexagon cells
        //    {

        //        for (int a = 0; a < width; a++)
        //        {
        //            CreateCell(a, b, c++);
        //        }
        //    }

        //    result = ChangeHexInfo(cells, hex_list, power_ups);
        //}

        return result;
    }

    public HexagonCell[] ChangeHexInfo(HexagonCell[] cells_, List<int> hexlist_, List<int> powercells_, List<int> hazards_, List<int> water_)
    {
        if (hexlist_.Count != 0)
        {
            for (int i = 0; i < hexlist_.Count; i++)
            {
                cells_[hexlist_[i]].gameObject.tag = "Wall";
                cells_[hexlist_[i]].gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                cells_[hexlist_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = Wall;
              //  cells_[hexlist_[i]].gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }

        if (powercells_.Count != 0)
        {
            for (int i = 0; i < powercells_.Count; i++)
            {
                int randval = Random.Range(1, 5);
                cells_[powercells_[i]].gameObject.AddComponent<TeamPowerupTiles>();
                cells_[powercells_[i]].tag = "TeamBuff";
                if (randval == 1)
                {
                    cells_[powercells_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = AttackBuff; //Need attack buff sprite
                    cells_[powercells_[i]].gameObject.GetComponent<TeamPowerupTiles>().attackBuff = true;
                }
                if (randval == 2)
                {
                    cells_[powercells_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = Healthbuff; //Need health buff sprite
                    cells_[powercells_[i]].gameObject.GetComponent<TeamPowerupTiles>().healthBuff = true;
                }
                if (randval == 3)
                {
                    cells_[powercells_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = MobilityBuff; //Need mobility buff sprite
                    cells_[powercells_[i]].gameObject.GetComponent<TeamPowerupTiles>().mobilityBuff = true;
                }
                if (randval == 4)
                {
                    cells_[powercells_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = CritBuff; // Need crit buff sprite
                    cells_[powercells_[i]].gameObject.GetComponent<TeamPowerupTiles>().critBuff = true;
                }
                if (randval == 5)
                {
                    cells_[powercells_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = AttackRangebuff; //Need Range buff sprite
                    cells_[powercells_[i]].gameObject.GetComponent<TeamPowerupTiles>().attackrangeBuff = true;
                }
            }
        }

        if (hazards_.Count != 0)
        {
        for (int i = 0; i < hazards_.Count; i++)
            {
                cells_[hazards_[i]].gameObject.tag = "SlowingTile";
                cells_[hazards_[i]].gameObject.AddComponent<TeamPowerupTiles>();
                cells_[hazards_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = SlowingTile;
               // cells_[hazards_[i]].gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }

        if(water_.Count != 0)
        {
            for( int i = 0; i < water_.Count; i++)
            {
                cells_[water_[i]].gameObject.tag = "Water";
                cells_[water_[i]].gameObject.AddComponent<TeamPowerupTiles>();
                cells_[water_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = Water;
              //  cells_[water_[i]].gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
        return cells_;
    }

    public HexagonCell Get_Cell_Index(HexagonCoord coordinates)
    {

        int index = coordinates.X_coord + coordinates.Z_coord * width + coordinates.Z_coord / 2;
        return cells[index];

    }

    public int Get_Index(HexagonCoord coordinates)
    {
        int index = coordinates.X_coord + coordinates.Z_coord * width + coordinates.Z_coord / 2;
        return index;

    }
}
