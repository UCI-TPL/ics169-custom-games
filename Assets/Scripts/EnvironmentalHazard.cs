﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalHazard : MonoBehaviour {
    public struct HazardInfo
    {
        public EnvironmentalHazard type;
        public int x;
        public int y;
        public int z;
        public int timeLeft;
        public int size;
        public bool placedWeatherVane;
        public bool p1;
        public bool turn1;

        public HazardInfo(EnvironmentalHazard _type, int _x, int _y, int _z, int _timeLeft, int _size, bool _weatherVane = false, bool _p1 = false, bool _turn1 = false)
        {
            type = _type;
            x = _x;
            y = _y;
            z = _z;
            timeLeft = _timeLeft;
            size = _size;
            placedWeatherVane = _weatherVane;
            p1 = _p1;
            turn1 = _turn1;
        }

    };
    public string type_name;
    public int timeToCome;
    public int timeOnBoard;
    public float anim_time = 2f;
    public SoundManager soundManager;
    

	// Use this for initialization
	void Start () {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual HazardInfo CreateHazard(int size, HexagonCoord coord, Grid hexGrid)
    {
        //HexagonCoord rand = hexGrid.cells[Random.Range(0, hexGrid.cells.Length)].coords;

        //int size = Random.Range(0, 3); // 0 = 1, 1 = 3, 2 = 5
        Debug.Log("hazard at (" + coord.x + "," + coord.z + ") with size: " + size);
        return new HazardInfo(this, coord.x, coord.Y_coord, coord.z, timeOnBoard, size);
    }

    public virtual void RemoveHazard(Grid hexGrid, int x, int z, int size, bool weatherVane)
    {

    }

    public virtual HazardInfo CreateHazardAt(HexagonCell cell, Grid hexGrid)
    {
        // code to spawn the particle system or whatever to show the effect

        //Debug.Log("shooting poison gas on map");
        HexagonCoord coord = cell.coords;
        int size = Random.Range(0, 3);

        return new HazardInfo(this, coord.x, coord.Y_coord, coord.z, timeOnBoard, size);

    }

    public virtual IEnumerator Effect(HexagonMapEditor editor, Grid hexGrid, int x, int z, int size) // does the effect of the hazard on the tiles that are in its size
    {
        List<HexagonCell> frontier = new List<HexagonCell>(); // list of nodes that the hazard has effect over
        HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(x,z));
        //Debug.Log(type_name  +" hazard epicenter at: " + curr.coords.x + "," + curr.coords.Y_coord + "," + curr.coords.z);
        for (int i = 0; i < hexGrid.cells.Length; i++)
        {

            int distance = curr.coords.FindDistanceTo(hexGrid.cells[i].coords);
            if(distance <= size)
            {
                frontier.Add(hexGrid.cells[i]);
            }
        }
        for(int j = 0; j < frontier.Count; j++)
        {
            if(frontier[j].occupied)
            {
                frontier[j].unitOnTile.current_health -= 10;
                Debug.Log("Hazard effected " + frontier[j].unitOnTile.unit_name + " for 10 damage");
            }
        }
        yield return new WaitForSeconds(anim_time);
        Debug.Log("effect finishing");
    }
}
