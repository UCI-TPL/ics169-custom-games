using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPowerupTiles : MonoBehaviour {
    public bool is_occupied;
    public List<StartUnit> UnitsTeam;
    // Use this for initialization
    void Start () {
        is_occupied = false;
	}
	
	// Update is called once per frame
	void Update () {
        Buff();
	}

    void Buff()
    {
        if (is_occupied == true)
        {
            for (int i = 0; i < UnitsTeam.Count; i++)
            {
                UnitsTeam[i].mobility += 1;
            }
            this.gameObject.GetComponent<TeamPowerupTiles>().enabled = !this.gameObject.GetComponent<TeamPowerupTiles>().enabled;
        }
        is_occupied = false;
    }
}
