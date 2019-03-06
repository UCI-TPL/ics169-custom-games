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

    public GameObject Missile_Effect;

    public GameObject Poison_Cloud_Effect;

    public GameObject RainObject;

    public GameObject Missile_Obj;

    public GameObject Poison_Cloud_Obj;

    public GameObject Weather_Vane_Obj;

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
        RainObject = Cells_Rain;
        //do some math to make effect render on the correct layer depth
        sort_tile_effect_object(RainObject);
    }

    public void CreateMissile()
    {
        GameObject m = Instantiate(Missile_Effect);
        m.transform.position = gameObject.transform.position;
        Missile_Obj = m;
        sort_tile_effect_object(Missile_Obj);
    }

    public void Create_Poison_Cloud()
    {
        GameObject Poison_Obj = Instantiate(Poison_Cloud_Effect);
        Poison_Obj.transform.localScale = new Vector3(2f, 2f, 2f);
        Poison_Obj.transform.position = this.gameObject.transform.position;
        Poison_Cloud_Obj = Poison_Obj;
        sort_tile_effect_object(Poison_Obj);
    }

    public void Create_Weather_Vane()
    {
        GameObject Weather_Vane = Instantiate(Resources.Load("Temp_Weather_Vane", typeof(GameObject))) as GameObject;
        Vector3 myPos = this.gameObject.transform.position;
        Vector3 beginPos = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 60f, this.gameObject.transform.position.z);
        StartCoroutine(SendDown(Weather_Vane, beginPos, myPos, 1f));
        Weather_Vane_Obj = Weather_Vane;
        sort_tile_effect_object(Weather_Vane_Obj);
    }

    IEnumerator SendDown(GameObject target, Vector3 beginning, Vector3 ending, float timeAmount)
    {
        float startTime = Time.time;
        while(Time.time < startTime + timeAmount)
        {
            target.transform.position = Vector3.Lerp(beginning, ending, (Time.time - startTime) / timeAmount);
            yield return null;
        }
        target.transform.position = ending;
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
