using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealUnit : BaseScript {
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void HealMember(int heal)
    {
        if (current_health + heal > health)
            current_health = health;
        else
            current_health += heal;

        int result = attack / check_dmg;
        if (current_attack + result > attack)
            current_attack = attack;
        current_attack += result * attack_loss;
    }
}
