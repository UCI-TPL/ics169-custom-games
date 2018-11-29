using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPowerupTiles : MonoBehaviour {
    public bool discovered,unoccupied,attackBuff,healthBuff,mobilityBuff,critBuff,attackrangeBuff,waterDebuff,grassDebuff;
    public List<StartUnit> UnitsTeam;
    public StartUnit PrevUnit;
    public int mobility;
    //public Sprite PoweredDownSprite;
    // Use this for initialization
    void Start () {
        discovered = true;
	}
	
	// Update is called once per frame
	void Update () {
        Debuff();
        Buff();
        ResetStats();
	}

    void Buff()
    {
        if (discovered == true)
        {
            if (attackBuff)
            {
                if (this.GetComponent<HexagonCell>().occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].attack += 20;
                        UnitsTeam[i].current_attack += 20;
                    }
                    discovered = false;
                    unoccupied = true;
                }
            }
            if (healthBuff)
            {
                if (this.GetComponent<HexagonCell>().occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].health += 100;
                        UnitsTeam[i].current_health += 100;
                    }
                    discovered = false;
                    unoccupied = true;
                }
            }
            if (mobilityBuff)
            {
                if (this.GetComponent<HexagonCell>().occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].mobility += 1;
                    }
                    discovered = false;
                    unoccupied = true;
                }
            }
            if (critBuff)
            {
                if (this.GetComponent<HexagonCell>().occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].crit += .2f;
                    }
                    discovered = false;
                    unoccupied = true;
                }
            }
            if (attackrangeBuff)
            {
                if (this.GetComponent<HexagonCell>().occupied)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].attackRange += 1;
                    }
                    discovered = false;
                    unoccupied = true;
                }
            }
            //this.gameObject.tag = "Floor";
            //this.gameObject.GetComponent<SpriteRenderer>().sprite = this.gameObject.GetComponent<Grid>().PoweredDown;
            //this.gameObject.GetComponent<SpriteRenderer>().sprite = PoweredDownSprite;
            //this.gameObject.GetComponent<TeamPowerupTiles>().enabled = !this.gameObject.GetComponent<TeamPowerupTiles>().enabled;
        }
        if (unoccupied)
        {
            if (attackBuff)
            {
                if (!this.GetComponent<HexagonCell>().occupied && UnitsTeam.Count != 0)
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
            if (healthBuff)
            {
                if (!this.GetComponent<HexagonCell>().occupied && UnitsTeam.Count != 0)
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
            if (mobilityBuff)
            {
                    if (!this.GetComponent<HexagonCell>().occupied && UnitsTeam.Count != 0)
                    {
                        for (int i = 0; i < UnitsTeam.Count; i++)
                        {
                            UnitsTeam[i].mobility -= 1;
                            UnitsTeam.Remove(UnitsTeam[i]);
                        }
                        discovered = false;
                    }
            }
            if (critBuff)
            {

                if (!this.GetComponent<HexagonCell>().occupied && UnitsTeam.Count != 0)
                {
                    for (int i = 0; i < UnitsTeam.Count; i++)
                    {
                        UnitsTeam[i].crit -= .2f;
                        UnitsTeam.Remove(UnitsTeam[i]);
                    }
                    discovered = false;
                }
            }
            if (attackrangeBuff)
            {
                if (!this.GetComponent<HexagonCell>().occupied && UnitsTeam.Count != 0)
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
        //discovered = false;
    }

    void Debuff()
    {
        if (this.GetComponent<HexagonCell>().occupied) //if someones on this tile
        {
            mobility = this.GetComponent<HexagonCell>().unitOnTile.mobility;
            PrevUnit = this.GetComponent<HexagonCell>().unitOnTile;
            if (this.GetComponent<HexagonCell>().unitOnTile.current_mobility == mobility)//if our mobility equals our og mobility then debuff
            {
                if (grassDebuff)
                {
                    if (this.GetComponent<HexagonCell>().unitOnTile.current_mobility - 1 <= 0)
                    {
                        this.GetComponent<HexagonCell>().unitOnTile.current_mobility = 1;
                        //discovered = false;
                    }
                    else
                    {
                        this.GetComponent<HexagonCell>().unitOnTile.current_mobility = this.GetComponent<HexagonCell>().unitOnTile.current_mobility - 1;
                        //discovered = false;
                    }
                }
                if (waterDebuff)
                {
                    if (this.GetComponent<HexagonCell>().unitOnTile.current_mobility - 2 <= 0)
                    {
                        this.GetComponent<HexagonCell>().unitOnTile.current_mobility = 1;
                       // discovered = false;
                    }
                    else
                    {
                        this.GetComponent<HexagonCell>().unitOnTile.current_mobility = this.GetComponent<HexagonCell>().unitOnTile.current_mobility - 2;
                       // discovered = false;
                    }
                }
            }
        }
        //if (discovered && this.GetComponent<HexagonCell>().unitOnTile != null)
        //{
        //    if (grassDebuff)
        //    {
        //        if (this.GetComponent<HexagonCell>().occupied)
        //        {
        //            PrevUnit = this.GetComponent<HexagonCell>().unitOnTile;
        //            if (this.GetComponent<HexagonCell>().unitOnTile.mobility - 1 == 0)
        //            {
        //                this.GetComponent<HexagonCell>().unitOnTile.mobility = 1;
        //                discovered = false;
        //            }
        //            else
        //            {
        //                this.GetComponent<HexagonCell>().unitOnTile.mobility = this.GetComponent<HexagonCell>().unitOnTile.mobility - 1;
        //                discovered = false;
        //            }
        //        }
        //    }

        //    if (waterDebuff)
        //    {
        //        if (this.GetComponent<HexagonCell>().occupied)
        //        {
        //            PrevUnit = this.GetComponent<HexagonCell>().unitOnTile;
        //            if (this.GetComponent<HexagonCell>().unitOnTile.mobility - 2 == 0)
        //            {
        //                this.GetComponent<HexagonCell>().unitOnTile.mobility = 1;
        //                discovered = false;
        //            }
        //            else
        //            {
        //                this.GetComponent<HexagonCell>().unitOnTile.mobility = this.GetComponent<HexagonCell>().unitOnTile.mobility - 2;
        //                discovered = false;
        //            }
        //        }
        //    }
        //}
    }

    void ResetStats()
    {
        if (PrevUnit != null)
        {
            if (grassDebuff)
            {
                if (!this.GetComponent<HexagonCell>().occupied)
                {
                    if (PrevUnit.current_mobility + 1 >= mobility)
                        PrevUnit.current_mobility = mobility;
                    else
                    {
                        PrevUnit.current_mobility = PrevUnit.current_mobility + 1;
                    }
                    PrevUnit = null;
                }
            }
            if (waterDebuff)
            {
                if (PrevUnit.current_mobility + 2 >= mobility)
                    PrevUnit.current_mobility = mobility;
                else
                {
                    PrevUnit.current_mobility = PrevUnit.current_mobility + 2;
                }
                PrevUnit = null;
            }
        }
    }
}
