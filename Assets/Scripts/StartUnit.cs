using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUnit : MonoBehaviour {

    public int mobility; // how far a unit can move
    public int attackRange; // how far a unit can attack
    public float health;
    public int attack;
    public float crit;
    //public int attack_loss; // how much attack a unit loses when hit
    //public int check_dmg; // check if dmg is greater than this amount to know if you lower the dmg or not
    public float current_health;
    public float current_attack;

    public bool dead = false;


	// Use this for initialization
	void Start () {
        current_health = health;
        current_attack = attack;
    }
	
	// Update is called once per frame
	void Update () {
        if(dead)
            StartCoroutine(Dead());
	}

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);   
    }
}
