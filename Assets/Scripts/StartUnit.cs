﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUnit : MonoBehaviour
{

    public string unit_type;
    public string unit_name;
    public int mobility; // how far a unit can move
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
    }

    // Update is called once per frame
    public void Update()
    {
        if (dead)
        {
            //StartCoroutine(Dead());
            this.gameObject.SetActive(false);
        }

    }

    public IEnumerator Dead()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    public virtual IEnumerator BasicAttack(Grid hexGrid, HexagonCell unitCell) // return bool yes if dead false if no
    {
        List<HexagonCell> targetable = new List<HexagonCell>();
        foreach (HexagonCell cell in hexGrid.cells)
        {
            if (unitCell.coords.FindDistanceTo(cell.coords) <= attackRange
                && unitCell.coords.FindDistanceTo(cell.coords) > 0
                && cell.occupied
                && tag != cell.unitOnTile.tag)
                targetable.Add(cell);
        }
        if (targetable.Count >= 1)
        {
            StartCoroutine(Attack());
            int rand_index = Random.Range(0, targetable.Count);
            float crit_chance = Random.value;
            float miss_chance = Random.value;
            float damage = current_attack;
            int dmg_txt = 0;

            if (miss_chance < miss)
                damage = 0;

            if (miss != 0)
            {
                if (crit_chance < crit)
                    damage = current_attack * crit_multiplier;
                dmg_txt = (int)damage;
            }

            if (targetable[rand_index].unitOnTile.FloatingTextPrefab)
            {
                GameObject damagetext = Instantiate(targetable[rand_index].unitOnTile.FloatingTextPrefab, targetable[rand_index].unitOnTile.transform.position, Quaternion.identity, transform);
                if (damage == 0)
                    damagetext.GetComponent<TextMesh>().text = "MISS";
                if(damage != 0)
                    damagetext.GetComponent<TextMesh>().text = dmg_txt.ToString();
                if (damagetext.transform.localScale.x == -1)
                    damagetext.gameObject.transform.localScale = new Vector3(1,0,0);
            }

            StartUnit attacked_unit = targetable[rand_index].unitOnTile;
            targetable[rand_index].unitOnTile.current_health -= damage;
            attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health; // fix?

            if (targetable[rand_index].unitOnTile.current_attack > 10)
            {
                float percenthealth = targetable[rand_index].unitOnTile.current_health / targetable[rand_index].unitOnTile.health;
                targetable[rand_index].unitOnTile.current_attack *= percenthealth;
            }

            Debug.Log("he dead");
            if (targetable[rand_index].unitOnTile.current_health <= 0)
            {

                int index = targetable[rand_index].coords.X_coord + targetable[rand_index].coords.Z_coord * hexGrid.width + targetable[rand_index].coords.Z_coord / 2;
                editor.RemoveUnitInfo(targetable[rand_index], index);
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
                StartCoroutine(targetable[rand_index].unitOnTile.Hit());
            }
        }
    }

    public IEnumerator HopToPlace(Grid hexGrid, HexagonCell unitCell, int index, int distance)
    {
        
        Stack<HexagonCell> result = hexGrid.FindPath(unitCell, hexGrid.cells[index]);
        //Debug.Log(result.Count);
        HexagonCoord current = unitCell.coords;
        while(result.Count > 0)
        {
            HexagonCell temp = result.Pop();
            //Debug.Log(temp.coords);
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
            transform.position = temp.transform.position;
            current = temp.coords;
            yield return new WaitForSeconds(1f);
        }

    }

    public IEnumerator Attack()
    {
        anim.SetBool("Attacking", true);
        attackSound.Play();
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Attacking", false);

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
}
