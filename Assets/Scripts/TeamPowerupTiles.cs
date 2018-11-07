using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPowerupTiles : MonoBehaviour {
    public bool discovered,attackBuff,healthBuff,mobilityBuff,critBuff,attackrangeBuff,waterDebuff,grassDebuff;
    public List<StartUnit> UnitsTeam;
    public StartUnit UnitonTile;
    public int mobility, debuff;
    // Use this for initialization
    void Start () {
        discovered = false;
        //mobility = UnitonTile.mobility;

	}
	
	// Update is called once per frame
	void Update () {
        Debuff();
        Buff();
	}

    void Buff()
    {
        if (discovered == true)
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
            this.gameObject.tag = "Floor";
            this.gameObject.GetComponent<SpriteRenderer>().sprite = this.gameObject.GetComponent<Grid>().PoweredDown;
            this.gameObject.GetComponent<TeamPowerupTiles>().enabled = !this.gameObject.GetComponent<TeamPowerupTiles>().enabled;
        }
        discovered = false;
    }

    //void Debuff()
    //{
    //    if (discovered && UnitonTile != null)
    //    {
    //        if (grassDebuff)
    //        {
    //            if (UnitonTile.mobility - 1 < 1)
    //                UnitonTile.mobility = 1;
    //            else
    //                UnitonTile.mobility = mobility - 1;
    //            discovered = false;
    //        }

    //        if (waterDebuff)
    //        {
    //            if (UnitonTile.mobility - 2 < 0)
    //                UnitonTile.mobility = 1;
    //            else
    //                UnitonTile.mobility = mobility - 2;
    //            discovered = false;
    //        }
    //    }
    //}
}
