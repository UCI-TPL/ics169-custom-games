﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGas : EnvironmentalHazard {
    public int damageDealt = 30; // amount of damage dealt by this effect

    public override HazardInfo CreateHazardAt(HexagonCell cell, Grid hexGrid)
    {
        // code to spawn the particle system or whatever to show the effect

        //Debug.Log("shooting poison gas on map");
        HexagonCoord coord = cell.coords;
        int size = 1;
        List<HexagonCell> frontier = new List<HexagonCell>();
        //HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(rand.x, rand.z));

        //HexagonCell hexa_cell = coord.

        for (int i = 0; i < hexGrid.cells.Length; i++)
        {

            int distance = cell.coords.FindDistanceTo(hexGrid.cells[i].coords);
            if (distance <= size)
            {
                frontier.Add(hexGrid.cells[i]);
            }
        }

        for (int j = 0; j < frontier.Count; j++)
        {
            frontier[j].Create_Poison_Cloud();
        }
        //GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().lightningSound.Play();
        //GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().rainSound.Play();


        return new HazardInfo(this, coord.x, coord.Y_coord, coord.z, timeOnBoard, 1);

    }

    public override void RemoveHazard(Grid hexGrid, int x, int z, int size, bool weatherVane)
    {
        //Debug.Log("removing poison gas from map");
        List<HexagonCell> frontier = new List<HexagonCell>();
        HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(x, z));
        for (int i = 0; i < hexGrid.cells.Length; i++)
        {


            int distance = curr.coords.FindDistanceTo(hexGrid.cells[i].coords);
            if (distance <= size)
            {
                frontier.Add(hexGrid.cells[i]);
            }
        }
        for (int j = 0; j < frontier.Count; j++)
        {
            Destroy(frontier[j].Poison_Cloud_Obj);
        }
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().fizzlefinishSound.Play();

    }

    public override IEnumerator Effect(HexagonMapEditor editor, Grid hexGrid, int x, int z, int size)
    {
        //Debug.Log("poison gas hurting people");
        List<HexagonCell> frontier = new List<HexagonCell>(); // list of nodes that the hazard has effect over
        HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(x, z));
        //Debug.Log(type_name + " hazard epicenter at: " + curr.coords.x + "," + curr.coords.Y_coord + "," + curr.coords.z);
        for (int i = 0; i < hexGrid.cells.Length; i++)
        {

            int distance = curr.coords.FindDistanceTo(hexGrid.cells[i].coords);
            if (distance <= size)
            {
                frontier.Add(hexGrid.cells[i]);
            }
        }
        for (int j = 0; j < frontier.Count; j++)
        {
            if (frontier[j].occupied && frontier[j].unitOnTile.gameObject.tag != gameObject.tag)
            {
                //frontier[j].unitOnTile.current_health -= damageDealt;

                StartUnit attacked_unit = frontier[j].unitOnTile;
                GameObject damagetext = Instantiate(attacked_unit.FloatingTextPrefab, attacked_unit.transform.position, Quaternion.identity, attacked_unit.transform);
                damagetext.GetComponent<TextMesh>().color = Color.yellow;
                damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * ((float)10 / 75f));
                damagetext.GetComponent<TextMesh>().text = damageDealt.ToString(); 

                if (Mathf.Sign(damagetext.transform.parent.localScale.x) == -1 && Mathf.Sign(damagetext.transform.localScale.x) == 1)
                {
                    damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                        damagetext.transform.localScale.z);

                    //damagetext.GetComponent<TextMesh>().color = Color.green;
                    //Debug.Log("BackWards Text");
                }
                else
                {
                    if (Mathf.Sign(damagetext.transform.parent.localScale.x) == 1 && Mathf.Sign(damagetext.transform.localScale.x) == -1)
                    {
                        damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                            damagetext.transform.localScale.z);
                    }
                }
                attacked_unit.TakeDamage(attacked_unit, damageDealt); // true damage so no need to incorporate 
                attacked_unit.PlayHit();
                attacked_unit.PlayBlink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f);

                if (attacked_unit.current_health <= 0)
                {

                    attacked_unit.dead = true;
                    editor.Units_To_Delete.Add(frontier[j]);
                    PoisonHero pHero;
                    if(gameObject.tag == "Player 1")
                    {
                        pHero = editor.P1Team[0].GetComponent<PoisonHero>();
                    }
                    else
                    {
                        pHero = editor.P2Team[0].GetComponent<PoisonHero>();
                    }
                    if (attacked_unit.tag == "TeamBuff") // was a buffmonster
                    {
                        GameObject buffItem = Instantiate(pHero.FloatingBuffPrefab, pHero.transform.position, Quaternion.identity, pHero.transform);
                        int randBuff = Random.Range(0, 4);
                        //give correct buff accordingly
                        Debug.Log("acquiring buff");
                        if (randBuff == 0) // movement buff
                        {
                            buffItem.GetComponent<SpriteRenderer>().sprite = pHero.mobilityBuff;
                            Debug.Log(name + " got a movement buff");
                            pHero.current_mobility += 1;
                            pHero.move_buff = true;
                            if (pHero.current_health != pHero.health)
                                pHero.current_health += 10;
                        }
                        else if (randBuff == 1) // crit buff
                        {
                            buffItem.GetComponent<SpriteRenderer>().sprite = pHero.critBuff;
                            Debug.Log(name + " got a crit buff");
                            pHero.crit += 0.20f;
                            pHero.crit_buff = true;
                            if (pHero.current_health != pHero.health)
                                pHero.current_health += 10;
                        }
                        else if (randBuff == 2) // attack buff
                        {
                            Debug.Log(name + " got an attack buff");
                            buffItem.GetComponent<SpriteRenderer>().sprite = pHero.attackBuff;
                            pHero.attack += 25;
                            pHero.current_attack += 25;
                            pHero.attack_buff = true;
                            if (pHero.current_health != pHero.health)
                                pHero.current_health += 10;
                        }
                        else // health buff
                        {
                            Debug.Log(name + " got a health buff");
                            buffItem.GetComponent<SpriteRenderer>().sprite = pHero.healthBuff;
                            pHero.health += 100;
                            pHero.current_health += 100;

                            pHero.health_buff = true;
                        }
                        if (pHero.current_health > (pHero.health * 0.4f))
                        {
                            pHero.anim.SetBool("Injured", false);
                            pHero.Injured = false;
                        }
                        float healthpercent = pHero.current_health / pHero.health;//    120/180 = .667

                        float attack_deduction = 1 - healthpercent;//   1 - .667 = .333
                        float reduction = attack_deduction / 2;
                        float new_attack = pHero.attack * reduction;//   72 * .333 = 23.76
                        pHero.current_attack = pHero.attack + new_attack;// 72 - 23.76 = 48

                        gameObject.GetComponentInChildren<Buff_UI_Manager>().update_current_buffs(pHero);
                    }
                }
            }
        }
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().fizzleSound.Play();
        yield return new WaitForSeconds(anim_time);
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().fizzleSound.Stop();
        Debug.Log("effect finishing");
    }
}
