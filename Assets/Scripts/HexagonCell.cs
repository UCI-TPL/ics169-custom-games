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
}
