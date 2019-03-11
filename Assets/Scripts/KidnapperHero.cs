using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidnapperHero : HeroUnit {
    public int specialAttackCounter = 5; // counter to keep track of when to fire off his load
    public int specialRange;

    StartUnit temp_attacked_unit;
    HexagonCell temp_attacked_cell;

    private void Update()
    {
        if (specialAttackCounter == 0)
            attackRange = specialRange;
    }
    public override IEnumerator BasicAttack(Grid hexGrid, HexagonCell unitCell)
    {

        
        if (specialAttackCounter <= 0) // ready to kidnap
        {
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(Kidnap(hexGrid, unitCell));
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(NormalBasicAttack(hexGrid, unitCell));
        }
        DecrementCounter();
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
            float damage;
            if (targetable[selectedTarget].GetComponent<HeroUnit>() != null)
                damage = (current_attack * 1.5f) - targetable[selectedTarget].unitOnTile.defense;
            else
                damage = current_attack - targetable[selectedTarget].unitOnTile.defense;
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
                        int rand = Random.Range(0, 2);
                        if (rand == 0)
                            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayOneFromList(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().critSounds);
                        damage = current_attack * crit_multiplier;
                        crit_happened = true;
                    }
                }
                dmg_txt = (int)damage;
            }

            

            //for (HexagonDirection d = HexagonDirection.NE; d <= HexagonDirection.NW; d++)
            //{
            //    HexagonCell neighbor = unitCell.GetNeighbor(d);
            //    if (!neighbor.occupied && neighbor.tag != "Wall" && targetable[selectedTarget].unitOnTile.GetComponent<HeroUnit>() == null) // if a neighbor to the hero is not occupied or wall unit not hero
            //    {
            //        targetable[selectedTarget].unitOnTile.transform.position = neighbor.transform.position; // move the bitch
            //        neighbor.occupied = true;
            //        neighbor.unitOnTile = targetable[selectedTarget].unitOnTile;
            //        temp_attacked_unit = neighbor.unitOnTile;
            //        temp_attacked_cell = neighbor;
            //        targetable[selectedTarget].occupied = false;
            //        targetable[selectedTarget].unitOnTile = null;
            //        //correctly sort kidnapped unit's meshes
            //        editor.re_sort_unit_position(temp_attacked_unit, temp_attacked_cell);
            //        break;
            //    }
            //}

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

            //-------------------------- Testing Kidnapp Stuff ----------------------

            StartCoroutine(move_to_kidnap(attacked_unit.gameObject,attacked_unit,attacked_cell, targetable[selectedTarget]));
            if(targetable[selectedTarget].unitOnTile.GetComponent<HeroUnit>() == null)
            {
                yield return new WaitForSeconds(1.40f); //---- for sync with kidnap to
            }
            else
            {
                yield return new WaitForSeconds(2.10f); //---- for sync with kidnap to
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
                    //correctly sort kidnapped unit's meshes
                    editor.re_sort_unit_position(temp_attacked_unit, temp_attacked_cell);
                    break;
                }
            }

            yield return new WaitForSeconds(0.26f); //---- for sync with kidnap to


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

            //-------------------------- Testing Kidnapp Stuff ---------------------- ^^^^

            if (attacked_unit.FloatingTextPrefab)
            {
                GameObject damagetext = Instantiate(attacked_unit.FloatingTextPrefab, attacked_unit.transform.position, Quaternion.identity, attacked_unit.transform);
                if (damage == 0)
                {
                    damagetext.GetComponent<TextMesh>().text = "MISS";
                    int rand = Random.Range(0, 2);
                    if (rand == 0)
                        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayOneFromList(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().missSounds);
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
                GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayOneFromList(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().killSounds);

                if (attacked_unit.tag == "TeamBuff") // was a buffmonster
                {
                    GameObject buffItem = Instantiate(FloatingBuffPrefab, transform.position, Quaternion.identity, transform);
                    int randBuff = Random.Range(0, 4);
                    //give correct buff accordingly
                    Debug.Log("acquiring buff");
                    if (randBuff == 0) // movement buff
                    {
                        buffItem.GetComponent<SpriteRenderer>().sprite = mobilityBuff;
                        Debug.Log(name + " got a movement buff");
                        current_mobility += 1;
                        move_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else if (randBuff == 1) // crit buff
                    {
                        buffItem.GetComponent<SpriteRenderer>().sprite = critBuff;
                        Debug.Log(name + " got a crit buff");
                        crit += 0.20f;
                        crit_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else if (randBuff == 2) // attack buff
                    {
                        Debug.Log(name + " got an attack buff");
                        buffItem.GetComponent<SpriteRenderer>().sprite = attackBuff;
                        attack += 25;
                        current_attack += 25;
                        attack_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else // health buff
                    {
                        Debug.Log(name + " got a health buff");
                        buffItem.GetComponent<SpriteRenderer>().sprite = healthBuff;
                        health += 100;
                        current_health += 100;

                        health_buff = true;
                    }

                    if (current_health > (health * 0.4f))
                    {
                        this.anim.SetBool("Injured", false);
                        this.Injured = false;
                    }

                    gameObject.GetComponentInChildren<Buff_UI_Manager>().update_current_buffs(this);

                    float healthpercent = current_health / health;//    120/180 = .667

                    float attack_deduction = 1 - healthpercent;//   1 - .667 = .333
                    float reduction = attack_deduction / 2;
                    float new_attack = attacked_unit.attack * reduction;//   72 * .333 = 23.76
                    current_attack = attack + new_attack;// 72 - 23.76 = 48

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
                //if (attacked_unit.gameObject.GetComponent<FortressHero>() != null && damage != 0) // handling of if attacking fortress hero 
                //{
                //    Debug.Log("Hurt by fortress hero's armor");
                //    if (FloatingTextPrefab)
                //    {
                //        GameObject damagetext = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
                //        damagetext.GetComponent<TextMesh>().text = 20.ToString();
                //        damagetext.GetComponent<TextMesh>().color = Color.yellow;
                //        damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * (20f / 75f));
                //        if (Mathf.Sign(damagetext.transform.parent.localScale.x) == -1 && Mathf.Sign(damagetext.transform.localScale.x) == 1)
                //        {
                //            damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                //                damagetext.transform.localScale.z);

                //            //damagetext.GetComponent<TextMesh>().color = Color.green;
                //            //Debug.Log("BackWards Text");
                //        }
                //        else
                //        {
                //            if (Mathf.Sign(damagetext.transform.parent.localScale.x) == 1 && Mathf.Sign(damagetext.transform.localScale.x) == -1)
                //            {
                //                damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                //                    damagetext.transform.localScale.z);
                //            }
                //        }
                //    }

                //    TakeDamage(this, 20f);
                //    StartCoroutine(AttackToHit());
                //    StartCoroutine(Blink(editor.Unit_Hurt_Color, this, Time.time + 1f));
                //    if (current_health <= 0) // pretty sure there's more code needed here but i'll ask christophe later
                //    {
                //        editor.Units_To_Delete.Add(unitCell);
                //        dead = true;
                //    }

                //}
                StartCoroutine(attacked_unit.Hit());
                StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));
            }
            specialAttackCounter = 3;
            attackRange = 1;
            temp_attacked_cell = null;
            temp_attacked_unit = null;
        }
        else
        {
            specialAttackCounter = 0;
            currently_attacking = false;
        }

        
        
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
                        int rand = Random.Range(0, 2);
                        if (rand == 0)
                            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayOneFromList(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().critSounds);
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
                    int rand = Random.Range(0, 2);
                    if (rand == 0)
                        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayOneFromList(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().missSounds);
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
                GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayOneFromList(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().killSounds);

                if (targetable[selectedTarget].unitOnTile.tag == "TeamBuff") // was a buffmonster
                {
                    GameObject buffItem = Instantiate(FloatingBuffPrefab, transform.position, Quaternion.identity, transform);
                    int randBuff = Random.Range(0, 4);
                    //give correct buff accordingly
                    Debug.Log("acquiring buff");
                    if (randBuff == 0) // movement buff
                    {
                        buffItem.GetComponent<SpriteRenderer>().sprite = mobilityBuff;
                        Debug.Log(name + " got a movement buff");
                        current_mobility += 1;
                        move_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else if (randBuff == 1) // crit buff
                    {
                        buffItem.GetComponent<SpriteRenderer>().sprite = critBuff;
                        Debug.Log(name + " got a crit buff");
                        crit += 0.20f;
                        crit_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else if (randBuff == 2) // attack buff
                    {
                        Debug.Log(name + " got an attack buff");
                        buffItem.GetComponent<SpriteRenderer>().sprite = attackBuff;
                        attack += 25;
                        current_attack += 25;
                        attack_buff = true;
                        if (current_health != health)
                            current_health += 10;
                    }
                    else // health buff
                    {
                        Debug.Log(name + " got a health buff");
                        buffItem.GetComponent<SpriteRenderer>().sprite = healthBuff;
                        health += 100;
                        current_health += 100;

                        health_buff = true;
                    }

                    if (current_health > (health * 0.4f))
                    {
                        this.anim.SetBool("Injured", false);
                        this.Injured = false;
                    }

                    gameObject.GetComponentInChildren<Buff_UI_Manager>().update_current_buffs(this);

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
                if (current_health - 20 <= 0)
                {
                    end_attack_without_retaliate = true;
                }
                StartCoroutine(Attack(hexGrid, unitCell, attacked_cell));
                yield return new WaitForSeconds(0.3f);

                if (attacked_unit.gameObject.GetComponent<FortressHero>() != null && damage != 0) // handling of if attacking fortress hero
                {
                    Debug.Log("Hurt by fortress hero's armor");
                    if (FloatingTextPrefab)
                    {
                        GameObject damagetext = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
                        damagetext.GetComponent<TextMesh>().text = 20.ToString();
                        damagetext.GetComponent<TextMesh>().color = Color.yellow;
                        damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * (20f / 75f));
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

                    TakeDamage(this, 20f);
                    StartCoroutine(AttackToHit());
                    StartCoroutine(Blink(editor.Unit_Hurt_Color, this, Time.time + 1f));
                    if (current_health <= 0) // pretty sure there's more code needed here but i'll ask christophe later
                    {
                        editor.Units_To_Delete.Add(unitCell);
                        dead = true;
                    }

                }
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
        if (specialAttackCounter > 0)
            specialAttackCounter -= 1;
    }

    public IEnumerator move_to_kidnap(GameObject Attacked_Unit, StartUnit attacked_unit, HexagonCell attacked_cell, HexagonCell cell_in_q)
    {
        if(cell_in_q.unitOnTile.GetComponent<HeroUnit>() == null)
        {
            this.anim.SetBool("Flash_Stepping", true);
            yield return new WaitForSeconds(0.37f);
            Vector3 Origin_Pos = this.gameObject.transform.position;
            Vector3 Attacked_Pos = Attacked_Unit.transform.position;
            if (direction)
            {
                //facing right
                this.gameObject.transform.position = new Vector3(Attacked_Pos.x - 10, Attacked_Pos.y, Attacked_Pos.z);
            }
            else
            {
                //facing left
                this.gameObject.transform.position = new Vector3(Attacked_Pos.x + 10, Attacked_Pos.y, Attacked_Pos.z);
            }
            yield return new WaitForSeconds(1.03f);
            this.gameObject.transform.position = Origin_Pos;


            //---------------------- face unit as comes back
            if (temp_attacked_unit != null)
            {
                attacked_unit = temp_attacked_unit;
                attacked_cell = temp_attacked_cell;
            }
            else
            {
                attacked_unit = cell_in_q.unitOnTile;
                attacked_cell = cell_in_q;
            }

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
            //--------------- face unit as comes back


            yield return new WaitForSeconds(0.26f);
            this.anim.SetBool("Flash_Stepping", false);
        }
        else
        {
            this.anim.SetBool("Flash_Stepping_Hero", true);
            yield return new WaitForSeconds(0.37f);
            Vector3 Origin_Pos = this.gameObject.transform.position;
            Vector3 Attacked_Pos = Attacked_Unit.transform.position;
            if (direction)
            {
                //facing right
                this.gameObject.transform.position = new Vector3(Attacked_Pos.x - 13, Attacked_Pos.y, Attacked_Pos.z);
            }
            else
            {
                //facing left
                this.gameObject.transform.position = new Vector3(Attacked_Pos.x + 13, Attacked_Pos.y, Attacked_Pos.z);
            }
            yield return new WaitForSeconds(1.73f);
            this.gameObject.transform.position = Origin_Pos;


            //---------------------- face unit as comes back
            if (temp_attacked_unit != null)
            {
                attacked_unit = temp_attacked_unit;
                attacked_cell = temp_attacked_cell;
            }
            else
            {
                attacked_unit = cell_in_q.unitOnTile;
                attacked_cell = cell_in_q;
            }

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
            //--------------- face unit as comes back


            yield return new WaitForSeconds(0.26f);
            this.anim.SetBool("Flash_Stepping_Hero", false);
        }
        
    }
}
