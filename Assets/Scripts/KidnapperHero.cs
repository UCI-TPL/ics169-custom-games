using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidnapperHero : HeroUnit {
    public int specialAttackCounter = 5; // counter to keep track of when to fire off his load
    public int specialRange;

    StartUnit temp_attacked_unit;
    HexagonCell temp_attacked_cell;


    public override IEnumerator BasicAttack(Grid hexGrid, HexagonCell unitCell)
    {

        DecrementCounter();
        if (specialAttackCounter == 0) // ready to kidnap
        {
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(Kidnap(hexGrid, unitCell));
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(NormalBasicAttack(hexGrid, unitCell));
        }
        
    }
    public IEnumerator Kidnap(Grid hexGrid, HexagonCell unitCell) // kidnap a random dumbass
    {
        Debug.Log("kidnap the unit");
        end_attack_without_retaliate = true;
        attacked_unit_has_died = false;

        string name = unitCell.unitOnTile.unit_name;


        //add a call to a retaliate function on the other unit   
        List<HexagonCell> targetable = new List<HexagonCell>();
        //Debug.Log(unitCell.unitOnTile.unit_name + " atttacking: ");
        foreach (HexagonCell cell in hexGrid.cells)
        {
            if (unitCell.coords.FindDistanceTo(cell.coords) <= attackRange
                && unitCell.coords.FindDistanceTo(cell.coords) > 0
                && cell.occupied
                && !cell.unitOnTile.dead
                && tag != cell.unitOnTile.tag)
                targetable.Add(cell);
        }

        if (targetable.Count >= 1)
        {

            editor.cursor.Assign_Position(this.transform.position, hexGrid.GetCell(this.transform.position).coords);
            Vector3 _new_Camera_Pos = new Vector3(this.transform.position.x, this.transform.position.y, editor.Main_Cam.transform.position.z);
            editor.Main_Cam.transform.position = Vector3.Lerp(editor.Main_Cam.transform.position, _new_Camera_Pos, 1f);

            yield return new WaitForSeconds(0.3f);
            int selectedTarget = ChosenEnemy(targetable);

            float crit_chance = Random.value;
            float miss_chance = Random.value;
            float damage = current_attack - targetable[selectedTarget].unitOnTile.defense;
            Debug.Log("Damage: " + damage);
            int dmg_txt = (int)damage;
            bool crit_happened = false;

            editor.printState();
            if (targetable[selectedTarget].unitOnTile.FloatingTextPrefab)
            {
                //Debug.Log("fadef");
                if (miss_chance <= miss)
                    damage = 0;
                else
                {
                    if (crit_chance <= crit && miss_chance > miss)
                    {
                        damage = current_attack * crit_multiplier;
                        crit_happened = true;
                    }
                }
                dmg_txt = (int)damage;
            }

            

            for (HexagonDirection d = HexagonDirection.NE; d <= HexagonDirection.NW; d++)
            {
                HexagonCell neighbor = unitCell.GetNeighbor(d);
                if (!neighbor.occupied && neighbor.tag != "Wall" && targetable[selectedTarget].unitOnTile.GetComponent<HeroUnit>() == null) // if a neighbor to the hero is not occupied or wall unit not hero
                {
                    targetable[selectedTarget].unitOnTile.transform.position = neighbor.transform.position; // move the bitch
                    neighbor.occupied = true;
                    neighbor.unitOnTile = targetable[selectedTarget].unitOnTile;
                    temp_attacked_unit = neighbor.unitOnTile;
                    temp_attacked_cell = neighbor;
                    targetable[selectedTarget].occupied = false;
                    targetable[selectedTarget].unitOnTile = null;
                    break;
                }
            }
            StartUnit attacked_unit;
            HexagonCell attacked_cell;
            if (temp_attacked_unit != null)
            {
                attacked_unit = temp_attacked_unit;
                attacked_cell = temp_attacked_cell;
            }
            else
            {
                attacked_unit = targetable[selectedTarget].unitOnTile;
                attacked_cell = targetable[selectedTarget];
            }
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
                        damagetext.GetComponent<TextMesh>().color = Color.red;
                        damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * ((float)dmg_txt / 75f));
                    }
                    else
                    {
                        damagetext.GetComponent<TextMesh>().color = Color.yellow;
                        damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * ((float)dmg_txt / 75f));
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
            Debug.Log(name + " attacked " + attacked_unit.unit_name + " for " + damage);
            TakeDamage(attacked_unit, damage);


            //Debug.Log("he dead");
            if (attacked_unit.current_health <= 0)
            {
                if (attacked_unit.tag == "TeamBuff") // was a buffmonster
                {
                    int randBuff = Random.Range(0, 4);
                    //give correct buff accordingly
                    Debug.Log("acquiring buff");
                    if (randBuff == 0) // movement buff
                    {
                        Debug.Log(name + " got a movement buff");
                        current_mobility += 1;
                        move_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else if (randBuff == 1) // crit buff
                    {
                        Debug.Log(name + " got a crit buff");
                        crit += 0.20f;
                        crit_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else if (randBuff == 2) // attack buff
                    {
                        Debug.Log(name + " got an attack buff");
                        attack += 25;
                        current_attack += 25;
                        attack_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else // health buff
                    {
                        Debug.Log(name + " got a health buff");
                        health += 100;
                        current_health = health;
                        health_buff = true;
                    }

                }
                end_attack_without_retaliate = true;
                attacked_unit_has_died = true;
                StartCoroutine(Attack(hexGrid, unitCell, attacked_cell));
                //int index = targetable[rand_index].coords.X_coord + targetable[rand_index].coords.Z_coord * hexGrid.width + targetable[rand_index].coords.Z_coord / 2;
                //editor.RemoveUnitInfo(targetable[rand_index], index);

                editor.Units_To_Delete.Add(attacked_cell);
                attacked_unit.dead = true;


                yield return new WaitForSeconds(0.3f);

                StartCoroutine(targetable[selectedTarget].unitOnTile.Hit());
                StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));

                //attacked_unit.Fade_Out_Body();
                //Should start some sort of DEATH ANIMATION COROUTINE HERE
            }
            else
            {
                if (unitCell.coords.FindDistanceTo(attacked_cell.coords) <= attacked_cell.unitOnTile.attackRange)
                {
                    end_attack_without_retaliate = false;
                }
                else
                {
                    end_attack_without_retaliate = true;
                }

                StartCoroutine(Attack(hexGrid, unitCell, attacked_cell));
                yield return new WaitForSeconds(0.3f);
                StartCoroutine(attacked_unit.Hit());
                StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));
            }
        }
        else
        {
            currently_attacking = false;
        }

        specialAttackCounter = 5;
        temp_attacked_cell = null;
        temp_attacked_unit = null;
        
    }

    public IEnumerator NormalBasicAttack(Grid hexGrid, HexagonCell unitCell)
    {
        end_attack_without_retaliate = true;
        attacked_unit_has_died = false;

        string name = unitCell.unitOnTile.unit_name;


        //add a call to a retaliate function on the other unit   
        List<HexagonCell> targetable = new List<HexagonCell>();
        //Debug.Log(unitCell.unitOnTile.unit_name + " atttacking: ");
        foreach (HexagonCell cell in hexGrid.cells)
        {
            if (unitCell.coords.FindDistanceTo(cell.coords) <= attackRange
                && unitCell.coords.FindDistanceTo(cell.coords) > 0
                && cell.occupied
                && !cell.unitOnTile.dead
                && tag != cell.unitOnTile.tag)
                targetable.Add(cell);
        }
        if (targetable.Count >= 1)
        {
            editor.cursor.Assign_Position(this.transform.position, hexGrid.GetCell(this.transform.position).coords);
            Vector3 _new_Camera_Pos = new Vector3(this.transform.position.x, this.transform.position.y, editor.Main_Cam.transform.position.z);
            editor.Main_Cam.transform.position = Vector3.Lerp(editor.Main_Cam.transform.position, _new_Camera_Pos, 1f);

            //StartCoroutine(this.Blink(Color.green, this, Time.time + 0.8f));
            yield return new WaitForSeconds(0.3f);
            int selectedTarget = ChosenEnemy(targetable);
            //int rand_index = Random.Range(0, targetable.Count);

            //int totalWeight = 0;
            //for(int i = 0; i < targetable.Count; i++)
            //{
            //    totalWeight += targetable[i].unitOnTile.weight;
            //}
            //int rand_val = Random.Range(1, totalWeight+1);
            //for(int j = 0; j < targetable.Count; j++)
            //{
            //    if(rand_val - targetable[j].unitOnTile.weight <= 0)
            //    {
            //        selectedTarget = j;
            //        break;
            //    }
            //    rand_val -= targetable[j].unitOnTile.weight;
            //}



            float crit_chance = Random.value;
            float miss_chance = Random.value;
            float damage = current_attack - targetable[selectedTarget].unitOnTile.defense;
            Debug.Log("Damage: " + damage);
            int dmg_txt = (int)damage;
            bool crit_happened = false;

            //if (miss_chance <= miss)
            //    damage = 0;
            //if (crit_chance <= crit && miss_chance > miss)
            //{
            //    damage = current_attack * crit_multiplier;
            //    dmg_txt = (int)damage;
            //}
            //Debug.Log(targetable);
            //Debug.Log("Attacker  Unit: " + unit_name);
            //Debug.Log("Targetted Unit: " + targetable[selectedTarget].unitOnTile.unit_name);
            editor.printState();
            if (targetable[selectedTarget].unitOnTile.FloatingTextPrefab)
            {
                //Debug.Log("fadef");
                if (miss_chance <= miss)
                    damage = 0;
                else
                {
                    if (crit_chance <= crit && miss_chance > miss)
                    {
                        damage = current_attack * crit_multiplier;
                        crit_happened = true;
                    }
                }
                dmg_txt = (int)damage;
            }

            StartUnit attacked_unit = targetable[selectedTarget].unitOnTile;
            HexagonCell attacked_cell = targetable[selectedTarget];
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
                        damagetext.GetComponent<TextMesh>().color = Color.red;
                        damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * ((float)dmg_txt / 75f));
                    }
                    else
                    {
                        damagetext.GetComponent<TextMesh>().color = Color.yellow;
                        damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * ((float)dmg_txt / 75f));
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
            Debug.Log(name + " attacked " + attacked_unit.unit_name + " for " + damage);
            TakeDamage(attacked_unit, damage);


            //Debug.Log("he dead");
            if (targetable[selectedTarget].unitOnTile.current_health <= 0)
            {
                if (targetable[selectedTarget].unitOnTile.tag == "TeamBuff") // was a buffmonster
                {
                    int randBuff = Random.Range(0, 4);
                    //give correct buff accordingly
                    Debug.Log("acquiring buff");
                    if (randBuff == 0) // movement buff
                    {
                        Debug.Log(name + " got a movement buff");
                        current_mobility += 1;
                        move_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else if (randBuff == 1) // crit buff
                    {
                        Debug.Log(name + " got a crit buff");
                        crit += 0.20f;
                        crit_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else if (randBuff == 2) // attack buff
                    {
                        Debug.Log(name + " got an attack buff");
                        attack += 25;
                        current_attack += 25;
                        attack_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else // health buff
                    {
                        Debug.Log(name + " got a health buff");
                        health += 100;
                        current_health = health;
                        health_buff = true;
                    }

                }
                end_attack_without_retaliate = true;
                attacked_unit_has_died = true;
                StartCoroutine(Attack(hexGrid, unitCell, attacked_cell));
                //int index = targetable[rand_index].coords.X_coord + targetable[rand_index].coords.Z_coord * hexGrid.width + targetable[rand_index].coords.Z_coord / 2;
                //editor.RemoveUnitInfo(targetable[rand_index], index);

                editor.Units_To_Delete.Add(attacked_cell);
                attacked_unit.dead = true;


                yield return new WaitForSeconds(0.3f);

                StartCoroutine(targetable[selectedTarget].unitOnTile.Hit());
                StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));

                //attacked_unit.Fade_Out_Body();
                //Should start some sort of DEATH ANIMATION COROUTINE HERE
            }
            else
            {
                if (unitCell.coords.FindDistanceTo(attacked_cell.coords) <= attacked_cell.unitOnTile.attackRange)
                {
                    end_attack_without_retaliate = false;
                }
                else
                {
                    end_attack_without_retaliate = true;
                }

                StartCoroutine(Attack(hexGrid, unitCell, attacked_cell));
                yield return new WaitForSeconds(0.3f);
                StartCoroutine(targetable[selectedTarget].unitOnTile.Hit());
                StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));
            }
        }
        else
        {
            currently_attacking = false;
        }
    
    }

    public void DecrementCounter() // decrease the counter in a nicer way? don't know why i wrote this function honestly
    {
        specialAttackCounter -= 1;
    }
}
