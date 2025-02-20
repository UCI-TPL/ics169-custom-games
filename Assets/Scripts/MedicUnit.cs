﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedicUnit : StartUnit {

	// Use this for initialization
	void Awake () {
        base.Start();
	}
	
	public override IEnumerator BasicAttack(Grid hexGrid, HexagonCell unitCell)
    {
        base.end_attack_without_retaliate = true;
        //add a call to a retaliate function on the other unit   
        List<HexagonCell> targetable = new List<HexagonCell>();
        foreach (HexagonCell cell in hexGrid.cells)
        {
            if (unitCell.coords.FindDistanceTo(cell.coords) <= attackRange
                && unitCell.coords.FindDistanceTo(cell.coords) > 0
                && cell.occupied
                && !cell.unitOnTile.dead
                && tag == cell.unitOnTile.tag
                && cell.unitOnTile.current_health != cell.unitOnTile.health)
                targetable.Add(cell);
        }
        if (targetable.Count >= 1)
        {
            editor.cursor.Assign_Position(this.transform.position, hexGrid.GetCell(this.transform.position).coords);
            editor.Main_Cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, editor.Main_Cam.transform.position.z);
            //StartCoroutine(this.Blink(Color.green, this, Time.time + 0.8f));
            yield return new WaitForSeconds(0.3f);

            int rand_index = Random.Range(0, targetable.Count);
            float crit_chance = Random.value;
            float miss_chance = Random.value;
            float damage = current_attack;
            int dmg_txt = (int)damage;
            bool crit_happened = false;

            if (miss_chance <= miss)
                damage = 0;
            if (crit_chance <= crit && miss_chance > miss)
            {
                damage = current_attack * crit_multiplier;
                dmg_txt = (int)damage;
            }

            if (targetable[rand_index].unitOnTile.FloatingTextPrefab)
            {
                if (crit_chance < crit)
                {
                    damage = current_attack * crit_multiplier;
                    crit_happened = true;
                }
                dmg_txt = (int)damage;
            }

            StartUnit attacked_unit = targetable[rand_index].unitOnTile;
            HexagonCell attacked_cell = targetable[rand_index];
            HexagonCoord current = unitCell.coords;

            if (attacked_cell.gameObject.transform.position.x > transform.position.x) //unit is to the right
            {
                if (!direction) //facing left, so needs to face right
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    direction = true;
                }
            }
            else //unit is to the left
            {
                if (direction)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    direction = false;
                }
            }

            if (attacked_unit.FloatingTextPrefab)
            {
                GameObject damagetext = Instantiate(attacked_unit.FloatingTextPrefab, attacked_unit.transform.position, Quaternion.identity, attacked_unit.transform);
                if (damage == 0)
                {
                    damagetext.GetComponent<TextMesh>().text = "MISS";
                    damagetext.GetComponent<TextMesh>().color = Color.white;
                    damagetext.GetComponent<TextMesh>().characterSize = 0.06f;
                }


                if (damage != 0)
                {
                    damagetext.GetComponent<TextMesh>().text = dmg_txt.ToString();
                    if (crit_happened)
                    {
                        damagetext.GetComponent<TextMesh>().color = Color.green;
                        damagetext.GetComponent<TextMesh>().characterSize = 0.1f;
                    }
                    else
                    {
                        damagetext.GetComponent<TextMesh>().color = Color.green;
                        damagetext.GetComponent<TextMesh>().characterSize = 0.06f;
                    }
                }

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

            }
            targetable[rand_index].unitOnTile.current_health += damage;
            if (this.gameObject.GetComponent<StartUnit>().current_health - 40 >= 40)
            {
                TakeDamage(this, 40f);
                if (FloatingTextPrefab)
                {
                    GameObject damagetext = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
                    damagetext.GetComponent<TextMesh>().text = 40.ToString();
                    damagetext.GetComponent<TextMesh>().color = Color.yellow;
                    damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * (40f / 75f));
                    if (Mathf.Sign(damagetext.transform.parent.localScale.x) == -1 && Mathf.Sign(damagetext.transform.localScale.x) == 1)
                    {
                        damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                            damagetext.transform.localScale.z);

                    }
                    else
                    {
                        if (Mathf.Sign(damagetext.transform.parent.localScale.x) == 1 && Mathf.Sign(damagetext.transform.localScale.x) == -1)
                        {
                            damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                                damagetext.transform.localScale.z);
                        }
                    }
                }
            }


            //this.GetComponent<StartUnit>().current_health -= 40;
            if (targetable[rand_index].unitOnTile.current_health > (targetable[rand_index].unitOnTile.health * 0.4f))
            {
                targetable[rand_index].unitOnTile.anim.SetBool("Injured", false);
                targetable[rand_index].unitOnTile.Injured = false;
            }
            
            if (targetable[rand_index].unitOnTile.current_health > targetable[rand_index].unitOnTile.health)
                targetable[rand_index].unitOnTile.current_health = targetable[rand_index].unitOnTile.health;
            float healthpercent = targetable[rand_index].unitOnTile.current_health / targetable[rand_index].unitOnTile.health;//    120/180 = .667
            Debug.Log(healthpercent);
            float attack_deduction = 1 - healthpercent;//   1 - .667 = .333
            float reduction = attack_deduction / 2;
            float new_attack = targetable[rand_index].unitOnTile.attack * reduction;//   72 * .333 = 23.76
            targetable[rand_index].unitOnTile.current_attack = targetable[rand_index].unitOnTile.attack + new_attack;// 72 - 23.76 = 48

            if (targetable[rand_index].unitOnTile.current_attack >= targetable[rand_index].unitOnTile.attack)
                targetable[rand_index].unitOnTile.current_attack = targetable[rand_index].unitOnTile.attack;

            attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health; // fix?
            //if (targetable[rand_index].unitOnTile.current_attack > 10)
            //{
            //    float percenthealth = targetable[rand_index].unitOnTile.current_health / targetable[rand_index].unitOnTile.health;
            //    targetable[rand_index].unitOnTile.current_attack *= percenthealth;
            //}



            //Debug.Log("he dead");
            if (targetable[rand_index].unitOnTile.current_health > targetable[rand_index].unitOnTile.health)
            {
                end_attack_without_retaliate = true;
                StartCoroutine(Attack(hexGrid, unitCell, attacked_cell));
                //int index = targetable[rand_index].coords.X_coord + targetable[rand_index].coords.Z_coord * hexGrid.width + targetable[rand_index].coords.Z_coord / 2;
                //editor.RemoveUnitInfo(targetable[rand_index], index);
                targetable[rand_index].unitOnTile.current_health = targetable[rand_index].unitOnTile.health;
            }
            else
            {
                if (unitCell.coords.FindDistanceTo(attacked_cell.coords) <= attacked_cell.unitOnTile.attackRange)
                {
                    end_attack_without_retaliate = true;
                }
                else
                {
                    end_attack_without_retaliate = true;
                }

                StartCoroutine(Attack(hexGrid, unitCell, attacked_cell));
                yield return new WaitForSeconds(0.3f);
                //if (FloatingTextPrefab)
                //{
                //    GameObject damagetext = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
                //    damagetext.GetComponent<TextMesh>().text = 20.ToString();
                //    damagetext.GetComponent<TextMesh>().color = Color.yellow;
                //    damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * (20f / 75f));
                //    if (Mathf.Sign(damagetext.transform.parent.localScale.x) == -1 && Mathf.Sign(damagetext.transform.localScale.x) == 1)
                //    {
                //        damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                //            damagetext.transform.localScale.z);

                //    }
                //    else
                //    {
                //        if (Mathf.Sign(damagetext.transform.parent.localScale.x) == 1 && Mathf.Sign(damagetext.transform.localScale.x) == -1)
                //        {
                //            damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                //                damagetext.transform.localScale.z);
                //        }
                //    }
                //}

                //TakeDamage(this, 20f);
                //StartCoroutine(AttackToHit());
                //StartCoroutine(Blink(editor.Unit_Hurt_Color, this, Time.time + 1f));
                //if (current_health <= 0)// pretty sure there's more code needed here but i'll ask christophe later
                //{
                //    editor.Units_To_Delete.Add(unitCell);
                //    dead = true;
                //}
                StartCoroutine(targetable[rand_index].unitOnTile.Hit());
                StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));
            }
        }
        else
        {
            currently_attacking = false;
        }

        if(this.gameObject.GetComponent<StartUnit>().current_health <= 0)
        {
            editor.Units_To_Delete.Add(unitCell);
            this.dead = true;
        }
    }
}
