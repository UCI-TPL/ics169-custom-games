using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUnit : MonoBehaviour {

    public string unit_type;
    public string unit_name;
    public int mobility; // how far a unit can move
    public int attackRange; // how far a unit can attack
    public float health;
    public int attack;
    public int basedmg;
    public float crit;

    public int cost;
    public float miss;
    public float crit_multiplier;
    public Sprite Icon;

    //public int attack_loss; // how much attack a unit loses when hit
    //public int check_dmg; // check if dmg is greater than this amount to know if you lower the dmg or not
    public float current_health;
    public float current_attack;
    public GameObject FloatingTextPrefab;
    public bool dead = false;
    public GameObject health_bar;
    Animator anim;
    public HexagonMapEditor editor;
    public GameObject Unit_Stats_Panel;


    //Attack, Hit, and Move sounds
    public AudioSource attackSound, hitSound, moveSound;



	// Use this for initialization
	void Start () {
        editor = FindObjectOfType<HexagonMapEditor>();
        attackSound = GetComponent<AudioSource>();
        hitSound = GetComponent<AudioSource>();
        moveSound = GetComponent<AudioSource>();
        attackSound.playOnAwake = false;
        hitSound.playOnAwake = false;
        moveSound.playOnAwake = false;

        anim = GetComponent<Animator>();
        current_health = health;
        current_attack = attack;
    }
	
	// Update is called once per frame
	void Update () {
        if (dead)
        {
            //StartCoroutine(Dead());
            this.gameObject.SetActive(false);
        }
            
	}

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);   
    }

    public IEnumerator BasicAttack(Grid hexGrid, HexagonCell unitCell) // return bool yes if dead false if no
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
            float random_val = Random.value;
            float damage = current_attack;
            if (random_val < crit)
                damage = current_attack * 2;
            int dmg_txt = (int)damage;
            if (targetable[rand_index].unitOnTile.FloatingTextPrefab)
            {
                GameObject damagetext = Instantiate(targetable[rand_index].unitOnTile.FloatingTextPrefab, targetable[rand_index].unitOnTile.transform.position, Quaternion.identity, transform);
                damagetext.GetComponent<TextMesh>().text = dmg_txt.ToString();
            }
            StartUnit attacked_unit = targetable[rand_index].unitOnTile;
            targetable[rand_index].unitOnTile.current_health -= damage;
            attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health; // fix?

            //if (targetable[rand_index].unitOnTile.current_attack > 10)
            //{
            float percenthealth = targetable[rand_index].unitOnTile.current_health / targetable[rand_index].unitOnTile.health;
            if(targetable[rand_index].unitOnTile.current_attack * percenthealth > 10)
                targetable[rand_index].unitOnTile.current_attack *= percenthealth;
            //}

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
        //Debug.Log("moving");
        anim.SetBool("Moving", true);
        moveSound.Play(); 
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("Moving", false);
    }
}
