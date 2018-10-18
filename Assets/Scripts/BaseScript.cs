using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript : MonoBehaviour {
    public int mobility; // how far a unit can move
    public int attackRange; // how far a unit can attack
    public int health;
    public int attack;
    public float crit;
    public int attack_loss; // how much attack a unit loses when hit
    public int check_dmg; // check if dmg is greater than this amount to know if you lower the dmg or not
    public int current_health;
    public int current_attack;

    // Use this for initialization
    void Start()
    {
        current_health = health;
        current_attack = attack;
    }

    void DamageUnit(int damage)
    {
        float random_val = Random.value;
        if (random_val < crit)
            damage = damage * 2;


        current_health -= damage;
        if (damage >= check_dmg && current_attack > 10)
        {
            int lower_attack = current_attack / check_dmg;
            current_attack -= lower_attack * attack_loss;
        }
        if (current_health <= 0)
            Destroy(this.gameObject);
    }


}
