using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGas : EnvironmentalHazard {

    public HazardInfo CreateHazardAt(HexagonCoord coord)
    {
        // code to spawn the particle system or whatever to show the effect
        return new HazardInfo(this, coord.x, coord.Y_coord, coord.z, timeOnBoard, 1);
    }

        // Use this for initialization
        void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
