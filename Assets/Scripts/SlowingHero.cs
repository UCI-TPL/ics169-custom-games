using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowingHero : StartUnit {

	// Use this for initialization
	void Awake () {
        base.Start();
	}
    public override void TakeDamage(StartUnit attacked_unit, float damage)
    {
        attacked_unit.current_health -= damage;
        attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health; // fix?

        float healthpercent = attacked_unit.current_health / attacked_unit.health;//    120/180 = .667
        float attack_deduction = 1 - healthpercent;//   1 - .667 = .333
        float new_attack = attacked_unit.attack * attack_deduction;//   72 * .333 = 23.76
        attacked_unit.current_attack = attacked_unit.attack - new_attack;// 72 - 23.76 = 48

        if (attacked_unit.current_mobility - 1 <= 0)
            attacked_unit.current_mobility = 1;
        else
            attacked_unit.current_mobility -= 1;
        attacked_unit.slowed = true;
    }
}
