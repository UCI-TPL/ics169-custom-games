using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUnit : MonoBehaviour
{

    public string unit_type;
    public string unit_name;
    public int unit_ID;
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
    public bool currently_attacking;
    private float time = 0.0f;
    public Color32 attack_blink_color;

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
        currently_attacking = false;
        
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
            editor.cursor.Assign_Position(this.transform.position, hexGrid.GetCell(this.transform.position).coords);
            editor.Main_Cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, editor.Main_Cam.transform.position.z);
            //StartCoroutine(this.Blink(Color.green, this, Time.time + 0.8f));
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(Attack());
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

            if (attacked_cell.coords.x > current.x || (attacked_cell.coords.x == current.x && attacked_cell.coords.z == current.z + 1)) //going right
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
            targetable[rand_index].unitOnTile.current_health -= damage;
            attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health; // fix?

            if (targetable[rand_index].unitOnTile.current_attack > 10)
            {
                float percenthealth = targetable[rand_index].unitOnTile.current_health / targetable[rand_index].unitOnTile.health;
                targetable[rand_index].unitOnTile.current_attack *= percenthealth;
            }

            //Debug.Log("he dead");
            if (targetable[rand_index].unitOnTile.current_health <= 0)
            {
                int index = targetable[rand_index].coords.X_coord + targetable[rand_index].coords.Z_coord * hexGrid.width + targetable[rand_index].coords.Z_coord / 2;
                editor.RemoveUnitInfo(targetable[rand_index], index);
            }
            else
            {
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
        yield return new WaitForSeconds(1f);
        currently_attacking = false;
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
        Debug.Log("Blinking?");
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

    public void Change_Health(int change_by, StartUnit target)
    {
        current_health = current_health + change_by;
        target.health_bar.GetComponent<Image>().fillAmount = target.current_health / target.health;
    }
}
