using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BerzerkerUnit : StartUnit {

    public void Awake()
    {
        base.Start();
    }

    public override void TakeDamage(StartUnit attacked_unit, float damage)
    {
        attacked_unit.current_health -= damage;
        attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health; // fix?

        float attack_deduction = attacked_unit.current_attack * (current_attack - attacked_unit.current_health / attacked_unit.health);
        float attack_increase = current_attack - attack_deduction;
        current_attack += attack_increase;
    }

}
