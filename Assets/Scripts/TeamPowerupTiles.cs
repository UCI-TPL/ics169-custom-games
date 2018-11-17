using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPowerupTiles : HexagonCell {
    public bool discovered,attackBuff,healthBuff,mobilityBuff,critBuff,attackrangeBuff,waterDebuff,grassDebuff;
    public List<StartUnit> UnitsTeam;
    public StartUnit UnitonTile;
    public int mobility, debuff;
    public Sprite PoweredDownSprite;
    // Use this for initialization
    void Start () {
        discovered = false;
        //mobility = UnitonTile.mobility;

	}
	
	// Update is called once per frame
	void Update () {
        //Debuff();
        Buff();
	}

    void Buff()
    {
        if (discovered == true)
        {
            if (attackBuff)
            {
                if (occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].attack += 20;
                        UnitsTeam[i].current_attack += 20;
                    }
                    discovered = false;
                }
                else
                {
                    if (!occupied && UnitsTeam.Count != 0)
                    {
                        for (int i = 0; i < UnitsTeam.Count; i++)
                        {
                            UnitsTeam[i].attack -= 20;
                            UnitsTeam[i].current_attack -= 20;
                            UnitsTeam.Remove(UnitsTeam[i]);
                        }
                        discovered = false;
                    }
                }
            }
            if (healthBuff)
            {
                if (occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].health += 100;
                        UnitsTeam[i].current_health += 100;
                    }
                    discovered = false;
                }
                else
                {
                    if (!occupied && UnitsTeam.Count != 0)
                    {
                        for (int i = 0; i < UnitsTeam.Count; i++)
                        {
                            UnitsTeam[i].health -= 100;
                            UnitsTeam[i].current_health -= 100;
                            UnitsTeam.Remove(UnitsTeam[i]);
                        }
                        discovered = false;
                    }
                }
            }
            if (mobilityBuff)
            {
                if (occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].mobility += 1;
                    }
                    discovered = false;
                }
                else
                {
                    if (!occupied && UnitsTeam.Count != 0)
                    {
                        for (int i = 0; i < UnitsTeam.Count; i++)
                        {
                            UnitsTeam[i].mobility -= 1;
                            UnitsTeam.Remove(UnitsTeam[i]);
                        }
                        discovered = false;
                    }
                }
            }
            if (critBuff)
            {
                if (occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].crit += .2f;
                    }
                    discovered = false;
                }
                else
                {
                    if (!occupied && UnitsTeam.Count != 0)
                    {
                        for (int i = 0; i < UnitsTeam.Count; i++)
                        {
                            UnitsTeam[i].crit -= .2f;
                            UnitsTeam.Remove(UnitsTeam[i]);
                        }
                        discovered = false;
                    }
                }
            }
            if (attackrangeBuff)
            {
                if (occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].attackRange += 1;
                    }
                    discovered = false;
                }
                else
                {
                    if (!occupied && UnitsTeam.Count != 0)
                    {
                        for (int i = 0; i < UnitsTeam.Count; i++)
                        {
                            UnitsTeam[i].attackRange -= 1;
                            UnitsTeam.Remove(UnitsTeam[i]);
                        }
                        discovered = false;
                    }
                }
            }
            //this.gameObject.tag = "Floor";
            //this.gameObject.GetComponent<SpriteRenderer>().sprite = this.gameObject.GetComponent<Grid>().PoweredDown;
            //this.gameObject.GetComponent<SpriteRenderer>().sprite = PoweredDownSprite;
            //this.gameObject.GetComponent<TeamPowerupTiles>().enabled = !this.gameObject.GetComponent<TeamPowerupTiles>().enabled;
        }
        //discovered = false;
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
