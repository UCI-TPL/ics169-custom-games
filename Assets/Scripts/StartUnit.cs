﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUnit : MonoBehaviour
{

    public string unit_type;
    public string unit_name;
    public int unit_ID;
    public int mobility,current_mobility; // how far a unit can move
    public int attackRange; // how far a unit can attack
    public float health;
    public int attack;
    public int basedmg;
    public float crit;
    public float miss;
    public float crit_multiplier;
    public Sprite Icon;
    public int cost;
    public bool direction = true; // right = true, left = false
    //public int attack_loss; // how much attack a unit loses when hit
    //public int check_dmg; // check if dmg is greater than this amount to know if you lower the dmg or not
    public float current_health;
    public float current_attack;
    public GameObject FloatingTextPrefab;
    public bool dead = false;
    public GameObject health_bar;
    public Animator anim;
    public HexagonMapEditor editor;
    public GameObject Unit_Stats_Panel;
    public bool currently_attacking;
    private float time = 0.0f;
    public Color32 attack_blink_color;
    public bool removed = false;

    //to determine if a retaliation is neccessary
    private bool end_attack_without_retaliate;


    private float dmg_txt_char_size;


    //Attack, Hit, and Move sounds
    public AudioSource attackSound, hitSound, moveSound;



    // Use this for initialization
    public void Start()
    {
    	attackSound.playOnAwake = false;
    	hitSound.playOnAwake = false;
    	moveSound.playOnAwake = false;

        editor = FindObjectOfType<HexagonMapEditor>();
        anim = GetComponent<Animator>();
        current_health = health;
        current_attack = attack;
        current_mobility = mobility;
        currently_attacking = false;
        
    }

    // Update is called once per frame
    public void Update()
    {
        if (dead && removed)
        {
            //StartCoroutine(Dead());
            this.gameObject.SetActive(false);
            
        }

    }

    // The way update portraits works makes destroying units not something that should be done
    public IEnumerator Dead()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }



    public virtual IEnumerator BasicAttack(Grid hexGrid, HexagonCell unitCell) // return bool yes if dead false if no
    {
        end_attack_without_retaliate = true;
        //add a call to a retaliate function on the other unit   
        List<HexagonCell> targetable = new List<HexagonCell>();
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
            editor.Main_Cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, editor.Main_Cam.transform.position.z);
            //StartCoroutine(this.Blink(Color.green, this, Time.time + 0.8f));
            yield return new WaitForSeconds(1.5f);
            
            int rand_index = Random.Range(0, targetable.Count);
            float crit_chance = Random.value;
            float miss_chance = Random.value;
            float damage = current_attack;
            int dmg_txt = (int)damage;
            bool crit_happened = false;

            //if (miss_chance <= miss)
            //    damage = 0;
            //if (crit_chance <= crit && miss_chance > miss)
            //{
            //    damage = current_attack * crit_multiplier;
            //    dmg_txt = (int)damage;
            //}
                  
            if (targetable[rand_index].unitOnTile.FloatingTextPrefab)
            {
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
                    damagetext.GetComponent<TextMesh>().color = Color.gray;
                    damagetext.GetComponent<TextMesh>().characterSize = 0.06f;
                }
                    
                
                if(damage != 0)
                {
                    damagetext.GetComponent<TextMesh>().text = dmg_txt.ToString();
                    if (crit_happened)
                    {
                        damagetext.GetComponent<TextMesh>().color = Color.yellow;
                        damagetext.GetComponent<TextMesh>().characterSize = 0.1f;
                    }
                    else
                    {
                        damagetext.GetComponent<TextMesh>().color = Color.white;
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
                    if(Mathf.Sign(damagetext.transform.parent.localScale.x) == 1 && Mathf.Sign(damagetext.transform.localScale.x) == -1)
                    {
                        damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                            damagetext.transform.localScale.z);
                    }
                }
                    
            }


            //attacked_unit.current_health -= damage;
            //attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health; // fix?

            //float attack_deduction = attacked_unit.current_attack * (current_attack - attacked_unit.current_health / attacked_unit.health);
            //if (attacked_unit.unit_ID == 6)//if attacked unit is berzerker then add to current attack
            //{
            //    float attack_increase = current_attack - attack_deduction;
            //    current_attack += attack_increase;
            //}
            //else // reduce the units attack by certain amount
            //{
            //    if (attack_deduction > attacked_unit.basedmg)
            //        attacked_unit.current_attack = attack_deduction;
            //    else
            //    {
            //        if (attack_deduction <= basedmg)
            //        {
            //            attacked_unit.current_attack = basedmg;
            //        }
            //    }
            //}

            

            //Debug.Log("he dead");
            if (targetable[rand_index].unitOnTile.current_health <= 0)
            {
                end_attack_without_retaliate = true;
                StartCoroutine(Attack(hexGrid, unitCell, attacked_cell));
                //int index = targetable[rand_index].coords.X_coord + targetable[rand_index].coords.Z_coord * hexGrid.width + targetable[rand_index].coords.Z_coord / 2;
                //editor.RemoveUnitInfo(targetable[rand_index], index);
                editor.Units_To_Delete.Add(attacked_cell);
                attacked_unit.dead = true;
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
                StartCoroutine(targetable[rand_index].unitOnTile.Hit());
                StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));
            }
        }
        else
        {
            currently_attacking = false;
        }
    }

    public virtual IEnumerator Retaliate(Grid hexGrid, HexagonCell unitCell_to_attack, HexagonCell unitCell_is_attacking) // return bool yes if dead false if no
    {
        //Debug.Log("Called_Retaliate");
        editor.cursor.Assign_Position(this.transform.position, hexGrid.GetCell(this.transform.position).coords);
        editor.Main_Cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, editor.Main_Cam.transform.position.z);
        //StartCoroutine(this.Blink(Color.green, this, Time.time + 0.8f));
        yield return new WaitForSeconds(1.5f);
        

        StartUnit attacked_unit = unitCell_to_attack.unitOnTile;
        HexagonCell attacked_cell = unitCell_to_attack;
        HexagonCoord current = unitCell_is_attacking.coords;

        float crit_chance = Random.value;
        float miss_chance = Random.value;
        float damage = current_attack;
        int dmg_txt = (int)damage;
        bool crit_happened = false;

        //deals with missing or critting

        if (miss_chance <= miss)
        {
            damage = 0;
        }
        else if (crit_chance <= crit)
        {
            damage = current_attack * crit_multiplier;
            dmg_txt = (int)damage;
            crit_happened = true;
        }

        //Deals with facing the individual that is getting attacked

        if (attacked_cell.gameObject.transform.position.x > transform.position.x) //going right
        {
            if (!direction) //facing left
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                direction = true;
            }
        }
        else //going left
        {
            if (direction)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                direction = false;
            }
        }

        //Deals with Creating the correct Damage Text

        if (attacked_unit.FloatingTextPrefab)
        {
            GameObject damagetext = Instantiate(attacked_unit.FloatingTextPrefab, attacked_unit.transform.position, Quaternion.identity, attacked_unit.transform);
            if (damage == 0)
            {
                damagetext.GetComponent<TextMesh>().text = "MISS";
                damagetext.GetComponent<TextMesh>().color = Color.gray;
                damagetext.GetComponent<TextMesh>().characterSize = 0.06f;
            }


            if (damage != 0)
            {
                damagetext.GetComponent<TextMesh>().text = dmg_txt.ToString();
                if (crit_happened)
                {
                    damagetext.GetComponent<TextMesh>().color = Color.yellow;
                    damagetext.GetComponent<TextMesh>().characterSize = 0.1f;
                }
                else
                {
                    damagetext.GetComponent<TextMesh>().color = Color.white;
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

        //Deal with change in health on attacked unit

        int to_deal = (int)(damage * -1);
        Change_Health(to_deal, attacked_unit);

        //Deals with damage health scaling... prolly shouldn't be done like this TBH

        if (attacked_unit.current_attack > 10)
        {
            float percenthealth = attacked_unit.current_health / attacked_unit.health;
            attacked_unit.current_attack *= percenthealth;
        }


        StartCoroutine(Retaliate_Anim(attacked_unit));
        //Debug.Log("he dead");
        if (attacked_unit.current_health <= 0)
        {

            //int index = attacked_cell.coords.X_coord + attacked_cell.coords.Z_coord * hexGrid.width + attacked_cell.coords.Z_coord / 2;
            //editor.RemoveUnitInfo(attacked_cell, index);
            attacked_unit.dead = true;
            editor.Units_To_Delete.Add(attacked_cell);
        }
        else
        {
            
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(attacked_unit.Hit());
            StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));
        }
        
    }

    public IEnumerator HopToPlace(Grid hexGrid, HexagonCell unitCell, int index, int distance)
    {
        
        Stack<HexagonCell> result = hexGrid.FindPath(unitCell, hexGrid.cells[index]);
        HexagonCoord current = unitCell.coords;
        while(result.Count > 0)
        {
            HexagonCell temp = result.Pop();
            if (temp.coords.x > current.x || (temp.coords.x == current.x && temp.coords.z == current.z + 1)) //going right
            {
                if (!direction) //facing left
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    direction = true;
                }
            }
            else //going left
            {
                if (direction)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    direction = false;
                }
            }
            StartCoroutine(Moving());
            yield return new WaitForSeconds(0.3f);
            transform.position = temp.transform.position;
            current = temp.coords;
            editor.re_sort_unit_position(this, hexGrid.GetCell(temp.transform.position));
            yield return new WaitForSeconds(0.3f);
        }

    }

    //should be depreciated... doesn't prock retaliate... however it is used as a way to cause a heal on medic... 
    public IEnumerator Attack()
    {
        anim.SetBool("Attacking", true);
        attackSound.Play();
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Attacking", false);
        yield return new WaitForSeconds(1f);
        currently_attacking = false;
    }

    public IEnumerator Attack(Grid hexGrid, HexagonCell target, HexagonCell retaliator)
    {
        anim.SetBool("Attacking", true);
        attackSound.Play();
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Attacking", false);
        yield return new WaitForSeconds(1f);
        if (end_attack_without_retaliate)
        {
            currently_attacking = false;
        }
        else
        {
            //call retaliate I guess
            Debug.Log("Retaliated");
            StartCoroutine(retaliator.unitOnTile.Retaliate(hexGrid, target, retaliator));
        }
    }

    public IEnumerator Retaliate_Anim(StartUnit retaliated_upon_unit)
    {
        anim.SetBool("Attacking", true);
        attackSound.Play();
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Attacking", false);
        yield return new WaitForSeconds(1f);
        
        retaliated_upon_unit.currently_attacking = false;
        
    }

    public IEnumerator Hit()
    {
        anim.SetBool("Hurt", true);
        hitSound.Play();
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("Hurt", false);
    }

    public IEnumerator Moving()
    {
        anim.SetBool("Moving", true);
        moveSound.Play(); 
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("Moving", false);
        moveSound.Stop();
    }

    public IEnumerator Blink(Color32 color, StartUnit unit, float time_until)
    {
        //Debug.Log("Blinking?");
        Anima2D.SpriteMeshInstance[] Unit_Meshes = unit.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();

        Color32 prev_color = Unit_Meshes[0].color;

        while (time_until >= Time.time)
        {
            
            for (int i = 0; i < Unit_Meshes.Length; i++)
            {
                Unit_Meshes[i].color = color;
                //Debug.Log("Color_Changed");
            }

            yield return new WaitForSeconds(0.2f);
            
            for (int i = 0; i < Unit_Meshes.Length; i++)
            {
                Unit_Meshes[i].color = prev_color;
                //Debug.Log("Color_Changed");
            }

            yield return new WaitForSeconds(0.2f);
        }

    }


    public virtual takeDamage(StartUnit attacked_unit, float damage)
    {
        attacked_unit.current_health -= damage;
        attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health; // fix?

        float attack_deduction = attacked_unit.current_attack * (current_attack - attacked_unit.current_health / attacked_unit.health);
        if (attack_deduction > attacked_unit.basedmg)
           attacked_unit.current_attack = attack_deduction;
        else
        {
            if (attack_deduction <= basedmg)
            {
                    attacked_unit.current_attack = basedmg;
            }
        }

    public void Change_Health(int change_by, StartUnit target)
    {
        target.current_health = target.current_health + change_by;
        target.health_bar.GetComponent<Image>().fillAmount = target.current_health / target.health;
    }
}
