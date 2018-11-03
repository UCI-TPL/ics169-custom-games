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
