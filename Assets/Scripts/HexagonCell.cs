using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//HexagonCell contains just its coordinates and is set by Grid when initalizing the board
// the coordinates can be seen in the inspector.
public class HexagonCell : MonoBehaviour {

    public Color color;

    public bool occupied = false;
    
    public HexagonCoord coords;

    public StartUnit unitOnTile;

    public SpriteRenderer spriteRenderer;

    public bool traversable = true;

    public HexagonCell PathFrom { get; set; }

    public int distance;

    [SerializeField]
    HexagonCell[] neighbors;

    public GameObject move_tile;

    public GameObject cross_tile;

    public GameObject selected_tile;

    public GameObject Acid_Rain_Effect;

    public GameObject Poison_Cloud_Effect;

    public GameObject HazardObject;

    public GameObject Poison_Cloud_Obj;

    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
            
        }
    }

    public int SearchHeuristic { get; set; }

    public int SearchPriority
    {
        get
        {
            return distance + SearchHeuristic;
        }
    }

    public HexagonCell NextWithSamePriority { get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public HexagonCell GetNeighbor(HexagonDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexagonDirection direction, HexagonCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public bool Occupied()
    {
        if(unitOnTile != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Show_Move_Icon()
    {
        move_tile.SetActive(true);
        move_tile.GetComponent<Blinking>().next_blink = Time.time;
        move_tile.GetComponent<Blinking>().color_bool = true;
    }

    public void Hide_Move_Icon()
    {
        move_tile.SetActive(false);
    }

    public void Show_Cross_Icon()
    {
        cross_tile.SetActive(true);
        move_tile.GetComponent<Blinking>().next_blink = Time.time;
        move_tile.GetComponent<Blinking>().color_bool = true;
    }

    public void Hide_Cross_Icon()
    {
        cross_tile.SetActive(false);
    }

    public void Show_Selected_Icon()
    {
        selected_tile.SetActive(true);
        selected_tile.GetComponent<Blinking>().next_blink = Time.time;
        selected_tile.GetComponent<Blinking>().color_bool = true;
    }

    public void Hide_Selected_Icon()
    {
        selected_tile.SetActive(false);
    }

    public IEnumerator Create_Rain_On_Delay()
    {
        //time delay is a bad solution to waiting for everything else to get created
        yield return new WaitForSeconds(1f);
        GameObject Cells_Rain = Instantiate(Acid_Rain_Effect);
        Cells_Rain.transform.position = this.gameObject.transform.position;
        //do some math to make effect render on the correct layer depth
        sort_tile_effect_object(Cells_Rain);
    }

    public void Create_Rain()
    {
        GameObject Cells_Rain = Instantiate(Acid_Rain_Effect);
        Cells_Rain.transform.position = this.gameObject.transform.position;
        HazardObject = Cells_Rain;
        //do some math to make effect render on the correct layer depth
        sort_tile_effect_object(Cells_Rain);
    }

    public void Create_Poison_Cloud()
    {
        GameObject Poison_Obj = Instantiate(Poison_Cloud_Effect);
        Poison_Obj.transform.position = this.gameObject.transform.position;
        Poison_Cloud_Obj = Poison_Obj;
        sort_tile_effect_object(Poison_Obj);
    }

    public void sort_tile_effect_object(GameObject to_be_sorted)
    {
        //takes all children of object that have a sorting layer and sorts them based on the position of the current tile
        ParticleSystemRenderer[] PS_list_to_sort = to_be_sorted.GetComponentsInChildren<ParticleSystemRenderer>();
        SpriteRenderer[] SR_list_to_sort = to_be_sorted.GetComponentsInChildren<SpriteRenderer>();
        //The scaling number is created in hexagonmap editor

        foreach (ParticleSystemRenderer PS_obj in PS_list_to_sort)
        {
            PS_obj.sortingOrder = PS_obj.sortingOrder + ((coords.X_coord + coords.Y_coord) * 4);
        }

        foreach (SpriteRenderer SR_obj in SR_list_to_sort)
        {
            SR_obj.sortingOrder = SR_obj.sortingOrder + ((coords.X_coord + coords.Y_coord) * 4);
        }

        Debug.Log("------------------------ Looped Through");

        //now must move object back a certain z as to not interfier with other rain planes
        int scale_by_plane_size = 10;
        to_be_sorted.transform.position = new Vector3(to_be_sorted.transform.position.x, to_be_sorted.transform.position.y,
            (to_be_sorted.transform.position.z + ((coords.X_coord + coords.Y_coord) * scale_by_plane_size) + 200) );
    }
}
