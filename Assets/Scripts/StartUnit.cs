using System.Collections;
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
    public float attack;
    public int basedmg;
    public float crit;
    public float miss;
    public float crit_multiplier;
    public int weight;

    public int defense = 0;
    //public int selectedTarget;

    public Sprite Icon,attackBuff,healthBuff,mobilityBuff,critBuff;
    public int cost;
    public string description;
    public int slowing_counter;
    public bool direction = true; // right = true, left = false
    //public int attack_loss; // how much attack a unit loses when hit
    //public int check_dmg; // check if dmg is greater than this amount to know if you lower the dmg or not
    public float current_health;
    public float current_attack;
    public GameObject FloatingTextPrefab,FloatingBuffPrefab;
    public bool dead = false;
    public GameObject health_bar;
    public Animator anim;
    public HexagonMapEditor editor;
    public GameObject Unit_Stats_Panel;
    public bool currently_attacking;
    private float time = 0.0f;
    public Color32 attack_blink_color;
    public bool removed = false;
    public bool slowed = false;
    //to determine if a retaliation is neccessary
    public bool end_attack_without_retaliate;
    public bool attacked_unit_has_died;
    public bool crit_buff;
    public bool attack_buff;
    public bool health_buff;
    public bool move_buff;
    public bool fortress_def_buff;

    private float dmg_txt_char_size;

    public GameObject Unit_Getting_Attacked;
    public GameObject Hit_Effect;
    //Attack, Hit, and Move sounds
    public AudioSource attackSound, hitSound, moveSound;
    public GameObject selection_arrow;
    [SerializeField]
    public AudioSource[] Moving_Lines_List = new AudioSource[0];
    [SerializeField]
    public AudioSource[] Attacking_Lines_List = new AudioSource[0];
    [SerializeField]
    public AudioSource[] Getting_Hit_Lines_List = new AudioSource[0];
    Color health_color;

    public bool Injured = false;

    public float extraWaitTime = 0;

    public GameObject Shield_Bubble;

    public Color32 color_should_be;

    // Use this for initialization
    public void Start()
    {
    	attackSound.playOnAwake = false;
    	hitSound.playOnAwake = false;
    	moveSound.playOnAwake = false;

        editor = FindObjectOfType<HexagonMapEditor>();
        anim = GetComponent<Animator>();
        current_health = health;
        current_mobility = mobility;
        currently_attacking = false;
        crit_buff = false;
        attack_buff = false;
        health_buff = false;
        move_buff = false;
        fortress_def_buff = false;
        health_color = health_bar.GetComponent<Image>().color;
        color_should_be = Color.white;
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

            //Below Is for Effects Triggered On Other Units That Get Attacked
            Unit_Getting_Attacked = attacked_unit.gameObject;
            //Above is for effects triggered on other units that get attacked

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
                    if(rand == 0)
                        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayOneFromList(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().missSounds);
                    damagetext.GetComponent<TextMesh>().color = Color.white;
                    damagetext.GetComponent<TextMesh>().characterSize = 0.06f;
                }
                    
                
                if(damage != 0)
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
                    if(Mathf.Sign(damagetext.transform.parent.localScale.x) == 1 && Mathf.Sign(damagetext.transform.localScale.x) == -1)
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
                    else if(randBuff == 2) // attack buff
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
                    if(current_health > (health * 0.4f))
                    {
                        this.anim.SetBool("Injured", false);
                        this.Injured = false;
                    }
                    float healthpercent = current_health / health;//    120/180 = .667

                    float attack_deduction = 1 - healthpercent;//   1 - .667 = .333
                    float reduction = attack_deduction / 2;
                    float new_attack = attack * reduction;//   72 * .333 = 23.76
                    current_attack = attack + new_attack;// 72 - 23.76 = 48
                    if(current_attack >= attack)
                    {
                        current_attack = attack;
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
                if (unitCell.coords.FindDistanceTo(attacked_cell.coords) <= attacked_cell.unitOnTile.attackRange && attacked_unit.gameObject.GetComponent<FortressHero>() == null)
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



                if(attacked_unit.gameObject.GetComponent<FortressHero>() != null && damage != 0) // handling of if attacking fortress hero
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
                    if (current_health <= 0)// pretty sure there's more code needed here but i'll ask christophe later
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

    public virtual IEnumerator Retaliate(Grid hexGrid, HexagonCell unitCell_to_attack, HexagonCell unitCell_is_attacking) // return bool yes if dead false if no
    {
        //for effects that trigger on a unit getting attacked
        Unit_Getting_Attacked = unitCell_to_attack.unitOnTile.gameObject;
        //Debug.Log("Called_Retaliate");
        attacked_unit_has_died = false;
        string attacker = unitCell_is_attacking.unitOnTile.unit_name;
       	string receiver = unitCell_to_attack.unitOnTile.unit_name;
        editor.cursor.Assign_Position(this.transform.position, hexGrid.GetCell(this.transform.position).coords);
        editor.Main_Cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, editor.Main_Cam.transform.position.z);
        //StartCoroutine(this.Blink(Color.green, this, Time.time + 0.8f));
        yield return new WaitForSeconds(0.3f);
        

        StartUnit attacked_unit = unitCell_to_attack.unitOnTile;
        HexagonCell attacked_cell = unitCell_to_attack;
        HexagonCoord current = unitCell_is_attacking.coords;

        float crit_chance = Random.value;
        float miss_chance = Random.value;
        float damage = current_attack - attacked_unit.defense;
        int dmg_txt = (int)damage;
        bool crit_happened = false;

        //deals with missing or critting

        if (miss_chance <= miss || damage <= 0)
        {
            damage = 0;
        }
        else if (crit_chance <= crit)
        {
            damage = current_attack * crit_multiplier;
            dmg_txt = (int)damage;
            int rand = Random.Range(0, 2);
            if (rand == 0)
                GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayOneFromList(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().critSounds);
            crit_happened = true;
        }

        //Half Damage On Retaliate???
        if(!unitCell_is_attacking.unitOnTile.gameObject.CompareTag("TeamBuff"))
        {
            damage = damage * 0.5f;
            dmg_txt = (int)damage;
        }
        else
        {
            Debug.Log("Buff Mob Retaliated ------->");
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

        //Deal with change in health on attacked unit

        int to_deal = (int)(damage * -1);
        Change_Health(to_deal, attacked_unit);

        //Deals with damage health scaling... prolly shouldn't be done like this TBH

        //if (attacked_unit.current_attack > 10)
        //{
        //    float percenthealth = attacked_unit.current_health / attacked_unit.health;
        //    attacked_unit.current_attack *= percenthealth;
        //}
        Debug.Log("Retaliation: " + attacker + "hit " + receiver + "for " + damage);
        TakeDamage(attacked_unit, damage);

        if(attacked_unit.current_health <= 0)
        {
            attacked_unit_has_died = true;
        }

        StartCoroutine(Retaliate_Anim(attacked_unit));
        //Debug.Log("he dead");
        if (attacked_unit.current_health <= 0)
        {
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayOneFromList(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().killSounds);
            //int index = attacked_cell.coords.X_coord + attacked_cell.coords.Z_coord * hexGrid.width + attacked_cell.coords.Z_coord / 2;
            //editor.RemoveUnitInfo(attacked_cell, index);
            //Debug.Log("adding unit to delete list in relatiation");


            attacked_unit.dead = true;
            editor.Units_To_Delete.Add(attacked_cell);

            yield return new WaitForSeconds(0.3f);

            StartCoroutine(attacked_unit.Hit());
            StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));
            
            //Some Kinda Death Animation Coroutine Here
        }
        else
        {
            
            yield return new WaitForSeconds(0.3f);

            if (attacked_unit.gameObject.GetComponent<FortressHero>() != null && damage != 0) // handling of if attacking fortress hero
            {
                Debug.Log("Hurt by fortress hero's armor in retaliation");
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
                    editor.Units_To_Delete.Add(unitCell_is_attacking);
                    dead = true;
                }

            }

            StartCoroutine(attacked_unit.Hit());
            StartCoroutine(attacked_unit.Blink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f));
        }
        
    }

    public IEnumerator HopToPlace(Grid hexGrid, HexagonCell unitCell, int index, int distance)
    {
        //All Movement Audio Goes Here
        if(Moving_Lines_List.Length > 0)
        {
            int Chosen_Voice_Line_Index = Random.Range(0, Moving_Lines_List.Length);
            Moving_Lines_List[Chosen_Voice_Line_Index].Play();
        }

        string name = unitCell.unitOnTile.unit_name;
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
        if (slowed)
        {
            slowing_counter -= 1;
            Debug.Log(name + " is slowed for " + slowing_counter + " more turns");
            if (slowing_counter == 0)
            {
                current_mobility = mobility;
                slowed = false;
                Debug.Log(name + " is no longer slowed");
            }
        }

    }

    //should be depreciated... doesn't prock retaliate... however it is used as a way to cause a heal on medic... 
    public IEnumerator Attack()
    {
        if (Attacking_Lines_List.Length > 0)
        {
            int Chosen_Voice_Line_Index = Random.Range(0, Attacking_Lines_List.Length);
            Attacking_Lines_List[Chosen_Voice_Line_Index].Play();
        }
        anim.SetBool("Attacking", true);
        //attackSound.Play();
        yield return new WaitForSeconds(0.5f);
        
        anim.SetBool("Attacking", false);
        yield return new WaitForSeconds(1f);
        if(extraWaitTime > 0)
            yield return new WaitForSeconds(extraWaitTime); // if theres more wait time
        Debug.Log("exiting attack anim");
        currently_attacking = false;
    }

    public IEnumerator Attack(Grid hexGrid, HexagonCell target, HexagonCell retaliator)
    {
        if (Attacking_Lines_List.Length > 0)
        {
            int Chosen_Voice_Line_Index = Random.Range(0, Attacking_Lines_List.Length);
            Attacking_Lines_List[Chosen_Voice_Line_Index].Play();
        }
        Camera.main.gameObject.GetComponent<CameraBounder>().Lerp_Change_Zoom(75f);
        anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(0.2f);
        //attackSound.Play();
        //Camera.main.gameObject.GetComponent<CameraBounder>().Shake_Camera(2f, 20f);
        yield return new WaitForSeconds(0.8f);
        
        anim.SetBool("Attacking", false);
        Debug.Log("here");
        Debug.Log(extraWaitTime);
        yield return new WaitForSeconds(extraWaitTime); // if theres more wait time
        extraWaitTime = 0f;
        Debug.Log("exiting attack anim");
        //yield return new WaitForSeconds(0.5f);
        if (end_attack_without_retaliate)
        {
            if (attacked_unit_has_died)
            {
                Debug.Log("----------------------------------------- Unit Dead ---------------------------------------");
                editor.cursor.Assign_Position(retaliator.gameObject.transform.position, retaliator.coords);
                Camera.main.gameObject.GetComponent<CameraBounder>().Lerp_Change_Zoom(140f);
                //Camera.main.gameObject.GetComponent<CameraBounder>().Shake_Camera(20f, 20f);
                yield return new WaitForSeconds(1f);
                Camera.main.gameObject.GetComponent<CameraBounder>().Lerp_Reset_Zoom();
                currently_attacking = false;
            }
            else
            {
                Camera.main.gameObject.GetComponent<CameraBounder>().Lerp_Reset_Zoom();
                yield return new WaitForSeconds(0.1f);
                currently_attacking = false;
            }
        }
        else
        {
            //call retaliate I guess
            //Debug.Log("Retaliated");
            StartCoroutine(retaliator.unitOnTile.Retaliate(hexGrid, target, retaliator));
        }
    }

    public IEnumerator Retaliate_Anim(StartUnit retaliated_upon_unit)
    {
        if (Attacking_Lines_List.Length > 0)
        {
            int Chosen_Voice_Line_Index = Random.Range(0, Attacking_Lines_List.Length);
            Attacking_Lines_List[Chosen_Voice_Line_Index].Play();
        }
        anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(0.2f);
        //attackSound.Play();
        //Camera.main.gameObject.GetComponent<CameraBounder>().Shake_Camera(2f, 20f);
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("Attacking", false);
        //yield return new WaitForSeconds(0.5f);
        Camera.main.gameObject.GetComponent<CameraBounder>().Lerp_Reset_Zoom();
        yield return new WaitForSeconds(0.1f);
        retaliated_upon_unit.currently_attacking = false;
        
    }

    //public void Play_AttackSound()
    //{
    //    attackSound.Play();
    //}

    //public void Shake_Camera()
    //{
    //    Camera.main.gameObject.GetComponent<CameraBounder>().Shake_Camera();
    //}
    public void PlayHit() // for objects not actually in the scene to call coroutines ex) AcidRain.Effect()
    {
        StartCoroutine(Hit());
    }


    public IEnumerator AttackToHit() // specifically for fortress hero armor damage
    {
        anim.SetBool("Attacking", false);
        anim.SetBool("Hurt", true);
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("Hurt", false);
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

    public void PlayBlink(Color32 color, StartUnit unit, float time_until) // similar to PlayHit() to pass initiative to the object so it can actually perform the coroutine
    {
        StartCoroutine(Blink(color, unit, time_until));
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

        for (int i = 0; i < Unit_Meshes.Length; i++)
        {
            Unit_Meshes[i].color = color_should_be;
            //Debug.Log("Color_Changed");
        }

    }


    public virtual void TakeDamage(StartUnit attacked_unit, float damage)
    {

        attacked_unit.current_health -= damage;
        attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health;
        //Debug.Log("Made it");
        float healthpercent = attacked_unit.current_health / attacked_unit.health;//    120/180 = .667

        float attack_deduction = 1 - healthpercent;//   1 - .667 = .333
        float reduction = attack_deduction / 2;
        float new_attack = attacked_unit.attack * reduction;//   72 * .333 = 23.76
        attacked_unit.current_attack = attacked_unit.attack - new_attack;// 72 - 23.76 = 48

        if (attacked_unit.current_health <= (attacked_unit.health * 0.4f))
        {
            attacked_unit.gameObject.GetComponent<Animator>().SetBool("Injured",true);
            attacked_unit.Injured = true;
            StartCoroutine(attacked_unit.Injured_Blinking());
        }

        //float attack_deduction = attacked_unit.current_attack * (current_attack - attacked_unit.current_health / attacked_unit.health);
        //if (attack_deduction > attacked_unit.basedmg)
        //    attacked_unit.current_attack = attack_deduction;
        //else
        //{
        //    if (attack_deduction <= basedmg)
        //    {
        //        attacked_unit.current_attack = basedmg;
        //    }
        //}
    }

    public void Change_Health(int change_by, StartUnit target)
    {
        target.current_health = target.current_health + change_by;
        target.health_bar.GetComponent<Image>().fillAmount = target.current_health / target.health;
    }

    public void Show_Arrow_Select()
    {
        selection_arrow.SetActive(true);
    }

    public void Hide_Arrow_Select()
    {
        selection_arrow.SetActive(false);
    }

    public int ChosenEnemy(List<HexagonCell> targetable)
    {
        int result = 0;
        int totalWeight = 0;
        for (int i = 0; i < targetable.Count; i++)
        {
            totalWeight += targetable[i].unitOnTile.weight;
        }
        int rand_val = Random.Range(1, totalWeight + 1);
        for (int j = 0; j < targetable.Count; j++)
        {
            if (rand_val - targetable[j].unitOnTile.weight <= 0)
            {
                result = j;
                break;
            }
            rand_val -= targetable[j].unitOnTile.weight;
        }
        return result;
    }

    public void Play_Attack_Sound_On_Event()
    {
        attackSound.Play();
        Debug.Log("Attack_Sound_Played");
    }

    public void Hide_All_Visible_Aspects()
    {
        foreach(SpriteRenderer sp_render in this.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            sp_render.enabled = false;
        }

        foreach(SkinnedMeshRenderer sk_render in this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            sk_render.enabled = false;
        }        
    }

    public void Fade_Out_Body()
    {
        foreach (Anima2D.SpriteMeshInstance sm_instance in this.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>())
        {
            fade_out(sm_instance);
        }
    }

    public IEnumerator co_Fade_Body()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Anima2D.SpriteMeshInstance sm_instance in this.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>())
        {
            fade_out(sm_instance);
        }
    }

    public IEnumerator fade_out(Anima2D.SpriteMeshInstance sm_instance)
    {
        Color color = sm_instance.color;
        while (color.a > 0)
        {
            color.a -= 0.1f * Time.deltaTime;
            sm_instance.color = color;
            yield return null;
        }
    }

    public void trigger_death_animation()
    {
        if(dead == true)
        {
            anim.SetBool("Hurt", false);
            anim.SetBool("Death", true);
            StartCoroutine(co_Fade_Body());
            Debug.Log("This Character is dead ----------------------------------- ");
        }
    }


    public void trigger_visual_effect()
    {
        Spawn_Flash flash_spawn = GetComponentInChildren<Spawn_Flash>();
        flash_spawn.Show_Flash();
    }

    public void trigger_effect_on_unit()
    {
        HexagonCell _cell = editor.hexGrid.GetCell(Unit_Getting_Attacked.transform.position);
        Spawn_Flash flash_spawn = GetComponentInChildren<Spawn_Flash>();
        GameObject flash_hit_obj = flash_spawn.Show_Flash_Hit(Unit_Getting_Attacked);
        re_sort_object_position(_cell, flash_hit_obj.GetComponent<SpriteRenderer>());
    }

    public void re_sort_object_position(HexagonCell _target_location, SpriteRenderer sprite_rend)
    {
        
        sprite_rend.sortingOrder = sprite_rend.GetComponent<Mesh_Layer>()._ordered_layer
            + ((_target_location.coords.X_coord + _target_location.coords.Y_coord) * editor.max_sprites_per_unit);
        
    }

    public IEnumerator Injured_Blinking()
    {
        while (Injured)
        {
            health_bar.GetComponent<Image>().color = Color.Lerp(health_color, Color.white, Mathf.PingPong(Time.time, 0.5f));
            yield return null;
        }

        health_bar.GetComponent<Image>().color = health_color;
        
    }
}
