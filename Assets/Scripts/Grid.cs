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
    public Sprite Wall, AttackBuff, Healthbuff, MobilityBuff, CritBuff, AttackRangebuff, SlowingTile, Water, PoweredDown, Sky;
    public bool ten, twenty, thirty;
    public int sprites_per_tile;
    public GameObject buffPrefab;

    //Grid Details
    HexagonCell[] result;
    List<int> wall_list1 = new List<int>() { 23, 24, 36, 42, 47, 52, 57, 63, 75, 76 };

    List<int> wall_list2 = new List<int>() { 18, 23, 24, 36, 42, 47, 52, 57, 63, 69,
                                             75, 76, 81 , 89, 90, 103, 115, 123, 136,
                                             163, 176,177,181,197, 202, 203, 216,
                                             263, 275, 284, 295, 309, 310, 318, 323,
                                             324, 330, 335, 342, 347, 352, 357, 363,
                                             375, 376, 381};
    List<int> sky_list = new List<int> { 377, 378, 379, 380, 381, 382, 383, 396, 397, 398, -1 };
    List<int> sky_list2 = new List<int>() { 0, 1, 2, 3, 16, 17, 18, 19, 20, 21, 22, 37, 38, 39, 40, 41, 42,
        58, 59, 60, 61, 78, 79, 80, 81, 99, 100, 119, 280, 299, 300, 319, 320, 321, 338, 339, 340,
        341, 358, 359, 360, 361, 362, 377, 378, 379, 380, 381, 382, 383, 396, 397, 398, 399 };

    List<int> water2 = new List<int> {          128,129,130,131,
                                            147, 148, 149, 150, 151,
                                        167, 168, /*169, 170,*/ 171, 172,
                                      186, 187, /*188, 190,*/ 191, 192,
                                        207, 208, /*209, 210,*/ 211, 212,
                                          227, 228, 229, 230, 231,
                                              248,249,250,251,
                                               162,183,195,217                       };

    List<int> powerlist1 = new List<int>() { 18, 81 };
    List<int> powerlist2 = new List<int>() { 182, 189, 196};

    List<int> hazards2 = new List<int> { 37, 77, 78, 127,107,108,109,110,111, 132, 146,
                                         152, 166, 173, 185,
                                         193, 206, 213, 226, 232,
                                         247, 252,267,268,269,270,271, 321, 322, 341,362};
    List<int> hazards = new List<int>() { };
    List<int> water = new List<int>() { };

    // prefabs cell and cellLabel should be children of grid
    public HexagonCell[] cellPrefabs;

    //public Text cellLabelPrefab;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.cyan;
    public Color attackColor = Color.green;

    public HexagonCell[] cells;
    Canvas gridCanvas;
    HexagonMesh hexMesh;

    public List<GameObject> Clutter_List;

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

    void Add_Clutter(HexagonCell cell)
    {

        if(cell.gameObject.tag == "Water" || cell.gameObject.tag == "Wall" || cell.gameObject.tag == "SlowingTile")
        {
            //don't do
        }
        else
        {
            int rand_clutter_amnt = Random.Range(0, 2);

            for (int i = 0; i < rand_clutter_amnt; i++)
            {
                int rand_clutter = Random.Range(0, Clutter_List.Count);
                GameObject clutter_object = Instantiate<GameObject>(Clutter_List[rand_clutter]);
                clutter_object.transform.SetParent(cell.transform);
                clutter_object.transform.localPosition = new Vector3(0, 0, 0);
                if (clutter_object.GetComponent<Clutter_Obj>().background)
                {
                    float pos_x_rand = Random.Range(-0.13f, 0.13f);
                    float pos_y_rand = Random.Range(0.08f, 0.1f);
                    clutter_object.transform.localPosition = new Vector3(pos_x_rand, pos_y_rand, 0);
                }
                else
                {
                    float pos_x_rand = Random.Range(-0.13f, 0.13f);
                    float pos_y_rand = Random.Range(-0.08f, -0.1f);
                    clutter_object.transform.localPosition = new Vector3(pos_x_rand, pos_y_rand, 0);
                }
            }
        }
    }

    void CreateCell(int a, int b, int c) // should only be called once when initializing the map 
    {
        Vector3 position;
        position.x = (a + b * 0.5f - b / 2) * (HexagonInfo.innerRadius * 2f);
        position.y = b * (HexagonInfo.outerRadius * 1.5f);
        position.z = 0f;
        int rand_tile = Random.Range(0, 3);
        HexagonCell cell = cells[c] = Instantiate<HexagonCell>(cellPrefabs[rand_tile]);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coords = HexagonCoord.FromOffsetCoordinates(a, b);
        cell.spriteRenderer.color = defaultColor;


        if(a > 0)
        {
            cell.SetNeighbor(HexagonDirection.W, cells[c - 1]);
        }
        if (b > 0)
        {
            if ((b & 1) == 0)
            {
                cell.SetNeighbor(HexagonDirection.SE, cells[c - width]);
                if (a > 0)
                    cell.SetNeighbor(HexagonDirection.SW, cells[c - width - 1]);
            }
            else
            {
                cell.SetNeighbor(HexagonDirection.SW, cells[c - width]);
                if(a < width - 1)
                {
                    cell.SetNeighbor(HexagonDirection.SE, cells[c - width + 1]);
                }
            }
        }
    
        cell.tag = "Floor";
        //cell.color = defaultColor;

        //Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform);
        //label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        //label.text = cell.coords.ToStringSeparateLines();
        //cell.Create_Rain();
    }


    public void ShowPath(HexagonCell current, int mobility, int range,Color color_m, Color color_a) // displays the movement capabilities of a unit using A*
    {
        List<HexagonCell> frontier = new List<HexagonCell>();
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
        }

        HexagonCell fromCell = current;
        //fromCell.spriteRenderer.color = color_m;
        fromCell.Show_Move_Icon();
        fromCell.Distance = 0;
        frontier.Add(fromCell);
        while (frontier.Count > 0)
        {

            HexagonCell curr = frontier[0];
            frontier.RemoveAt(0);
            if (curr.distance == mobility)
            {
                //curr.spriteRenderer.color = color_m;
                curr.Show_Move_Icon();
                continue;
            }
            for (HexagonDirection d = HexagonDirection.NE; d <= HexagonDirection.NW; d++)
            {
                
                HexagonCell neighbor = curr.GetNeighbor(d);
                if (neighbor == null || neighbor.Distance != int.MaxValue) // no tile there, or explored already
                {
                    continue;
                }
                if (!neighbor.traversable)// if a wall
                {
                    continue;
                }
                if(neighbor.occupied && neighbor.unitOnTile.tag != current.unitOnTile.tag) // if there is an enemy unit on that tile
                {
                    continue;
                }
                int distance = curr.Distance;
                /*if(water)
                 * distance += 3
                 *else if(grass)
                 * distance += 2
                 */
                if (neighbor.tag == "Water")
                    distance += 2;
                else
                    distance += 1;

                //distance += 1;
                //if (neighbor.Distance == int.MaxValue)
                //{
                //    neighbor.spriteRenderer.color = color_m;
                //    neighbor.Distance = distance;
                //    neighbor.PathFrom = curr;
                //    frontier.Add(neighbor);
                //}
                if (distance > mobility)
                    continue;
                if (distance < neighbor.Distance)
                {
                    //neighbor.spriteRenderer.color = color_m;
                    neighbor.Show_Move_Icon();
                    neighbor.Distance = distance;
                    frontier.Add(neighbor);
                }
                frontier.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            }
        }

}

    public void ClearPath()
    {
        for (int i = 0; i < (width * height); i++)
        {
            //cells[i].spriteRenderer.color = defaultColor;
            cells[i].Hide_Move_Icon();
            cells[i].Hide_Cross_Icon();
            cells[i].Hide_Selected_Icon();
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
        //ten = true;
        if (ten)
        {
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
            result = ChangeHexInfo(cells, wall_list1, powerlist1, hazards,sky_list, water);
            

        }

        if(twenty)
        {
            height = 20;
            width = 20;
            cells = new HexagonCell[height * width]; // create an array of correct length

            for (int b = 0, c = 0; b < height; b++) // fill the array with actual hexagon cells
            {

                for (int a = 0; a < width; a++)
                {
                    CreateCell(a, b, c++);
                }
            }
            result = ChangeHexInfo(cells, wall_list2, powerlist2, hazards2, sky_list2, water2);
            
        }

        //if (thirty)
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

    public HexagonCell[] ChangeHexInfo(HexagonCell[] cells_, List<int> hexlist_, List<int> powercells_, List<int> hazards_, List<int> sky, List<int> water_)
    {
        if (hexlist_.Count != 0)
        {
            for (int i = 0; i < hexlist_.Count; i++)
            {
                cells_[hexlist_[i]].gameObject.tag = "Wall";
                cells_[hexlist_[i]].traversable = false;
                cells_[hexlist_[i]].gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                cells_[hexlist_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = Wall;
                
              //  cells_[hexlist_[i]].gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }

        if (powercells_.Count != 0)
        {
            for (int i = 0; i < powercells_.Count; i++)
            {
                /* old code
                int randval = Random.Range(1, 5);
                cells_[powercells_[i]].gameObject.AddComponent<TeamPowerupTiles>();
                //cells_[powercells_[i]].gameObject.GetComponent<TeamPowerupTiles>().PoweredDownSprite = PoweredDown;
                //cells_[powercells_[i]].gameObject.GetComponent<TeamPowerupTiles>().discovered = true;
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
                */
                GameObject GO = Instantiate(buffPrefab);
                GO.transform.position = cells[powercells_[i]].transform.position;
                cells[powercells_[i]].occupied = true;
                StartUnit startUnit = GO.GetComponent<StartUnit>();
                Anima2D.SpriteMeshInstance[] Unit_Meshes = GO.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
                for (int k = 0; k < Unit_Meshes.Length; k++)
                {
                    //puts each unit in a section of the sorting layer according to the tile they are on.
                    Unit_Meshes[k].sortingOrder = Unit_Meshes[k].GetComponent<Mesh_Layer>()._ordered_layer
                        + ((cells[powercells_[i]].coords.X_coord + cells[powercells_[i]].coords.Y_coord) * sprites_per_tile);
                    //Debug.Log("Color_Changed");
                }
                cells[powercells_[i]].unitOnTile = startUnit;
                startUnit.Unit_Stats_Panel.GetComponent<BattleUI>().Hide();
                //Anima2D.SpriteMeshInstance[] Unit_Meshes = startUnit.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
            }
        }

        if (hazards_.Count != 0)
        {
            for (int i = 0; i < hazards_.Count; i++)
                {
                  
                    cells_[hazards_[i]].gameObject.tag = "SlowingTile";
                    cells_[hazards_[i]].gameObject.AddComponent<TeamPowerupTiles>();
                    cells_[hazards_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = SlowingTile;
                    //cells_[hazards_[i]].gameObject.GetComponent<TeamPowerupTiles>().discovered = true;
                    cells_[hazards_[i]].gameObject.GetComponent<TeamPowerupTiles>().grassDebuff = true;
                   // cells_[hazards_[i]].gameObject.GetComponent<SpriteRenderer>().color = Color.green;
               
                }
        }

        if(sky.Count != 0)
        {
            for (int i = 0; i < sky.Count; i++)
            {
                cells_[sky[i]].gameObject.tag = "Wall";
                cells_[sky[i]].traversable = false;
                cells_[sky[i]].gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                cells_[sky[i]].gameObject.GetComponent<SpriteRenderer>().sprite = Sky;
            }
        }


        if (water_.Count != 0)
        {
            for( int i = 0; i < water_.Count; i++)
            {
                cells_[water_[i]].gameObject.tag = "Water";
                cells_[water_[i]].gameObject.AddComponent<TeamPowerupTiles>();
                cells_[water_[i]].gameObject.GetComponent<SpriteRenderer>().sprite = Water;
               // cells_[hazards_[i]].gameObject.GetComponent<TeamPowerupTiles>().discovered = true;
                cells_[water_[i]].gameObject.GetComponent<TeamPowerupTiles>().waterDebuff = true;
              //  cells_[water_[i]].gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }

        for(int i = 0; i < cells_.Length; i++)
        {
            Add_Clutter(cells_[i]);
            Order_Cell(cells_[i], sprites_per_tile);
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
    public Stack<HexagonCell> FindPath(HexagonCell fromCell, HexagonCell toCell)
    {
        StopAllCoroutines();
        return Search(fromCell, toCell);
    }


    //used for putting sprites in the right order, so that everything appears as it should (further away tiles appear behind those closer up)
    public void Order_Cell(HexagonCell _cell, int num_sprites_per_cell) 
    {
        //int _current_sorting_order = _cell.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        //_cell.gameObject.GetComponent<SpriteRenderer>().sortingOrder = _current_sorting_order +
        //    ((_cell.coords.X_coord + _cell.coords.Y_coord) * num_sprites_per_cell);

        int count = 0;
        SpriteRenderer[] sprites = _cell.gameObject.GetComponentsInChildren<SpriteRenderer>(true);
        //Debug.Log("--------------------" + sprites.Length);
        foreach(SpriteRenderer sprite_renderer in sprites)
        {
            sprite_renderer.sortingOrder = sprite_renderer.sortingOrder 
                + ((_cell.coords.X_coord + _cell.coords.Y_coord) * num_sprites_per_cell);
            count += 1;
            //Debug.Log("" + count + ": " + num_sprites_per_cell + " " + sprite_renderer.sortingOrder);
        }
    }

    Stack<HexagonCell> Search (HexagonCell fromCell, HexagonCell toCell) // searrch creates a stack of the shortest path given a to and from tile.  this is used for movement animations
    {
        Stack<HexagonCell> result = new Stack<HexagonCell>();
        for(int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
        }
        List<HexagonCell> frontier = new List<HexagonCell>();
        fromCell.Distance = 0;
        frontier.Add(fromCell);
        while (frontier.Count > 0)
        {
            HexagonCell current = frontier[0];
            frontier.RemoveAt(0);

            if (current == toCell) {
                result.Push(toCell);
                current = current.PathFrom;
                while(current != fromCell)
                {
                    result.Push(current);
                    current = current.PathFrom;
                }
                return result;
            }
            for (HexagonDirection d = HexagonDirection.NE; d <= HexagonDirection.NW; d++)
            {
                HexagonCell neighbor = current.GetNeighbor(d);
                if(neighbor == null || neighbor.Distance != int.MaxValue)
                {
                    continue;
                }
                if(!neighbor.traversable)
                {
                    continue;
                }
                int distance = current.Distance;
                /*if(water)
                 * distance += 3
                 *else if(grass)
                 * distance += 2
                 */
                if (current.tag == "Water")
                    distance += 2;
                else 
                    distance += 1;
     
                if (neighbor.Distance == int.MaxValue)
                {
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    frontier.Add(neighbor);
                }
                else if(distance < neighbor.Distance)
                {
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                }
                frontier.Sort((x,y) => x.Distance.CompareTo(y.Distance));
            }
        }
        return result = new Stack<HexagonCell>();
    }
}
