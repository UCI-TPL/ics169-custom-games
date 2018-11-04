using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPowerupTiles : MonoBehaviour {
    public bool is_occupied,attackBuff,healthBuff,mobilityBuff,critBuff,attackrangeBuff;
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
            if (attackBuff)
            {
                for (int i = 0; i < UnitsTeam.Count; i++)
                {
                    UnitsTeam[i].attack += 20;
                }
            }
            if (healthBuff)
            {
                for (int i = 0; i < UnitsTeam.Count; i++)
                {
                    UnitsTeam[i].health += 100;
                }
            }
            if (mobilityBuff)
            {
                for (int i = 0; i < UnitsTeam.Count; i++)
                {
                    UnitsTeam[i].mobility += 1;
                }
            }
            if (critBuff)
            {
                for (int i = 0; i < UnitsTeam.Count; i++)
                {
                    UnitsTeam[i].crit += .2f;
                }
            }
            if (attackrangeBuff)
            {
                for (int i = 0; i < UnitsTeam.Count; i++)
                {
                    UnitsTeam[i].attackRange += 1;
                }
            }
            this.gameObject.GetComponent<TeamPowerupTiles>().enabled = !this.gameObject.GetComponent<TeamPowerupTiles>().enabled;
        }
        is_occupied = false;
    }
}
