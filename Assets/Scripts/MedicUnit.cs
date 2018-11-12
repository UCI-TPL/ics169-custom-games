using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedicUnit : StartUnit {

	// Use this for initialization
	void Awake () {
        base.Start();
	}
	
	public override IEnumerator BasicAttack(Grid hexGrid, HexagonCell unitCell)
    {
        List<HexagonCell> targetable = new List<HexagonCell>();
        foreach (HexagonCell cell in hexGrid.cells)
        {
            if (unitCell.coords.FindDistanceTo(cell.coords) <= attackRange
                && unitCell.coords.FindDistanceTo(cell.coords) > 0
                && cell.occupied
                && tag == cell.unitOnTile.tag)
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
                if (damage != 0)
                    damagetext.GetComponent<TextMesh>().text = dmg_txt.ToString();
                if (damagetext.transform.localScale.x == -1)
                    damagetext.gameObject.transform.localScale = new Vector3(1, 0, 0);
            }

            StartUnit attacked_unit = targetable[rand_index].unitOnTile;
            targetable[rand_index].unitOnTile.current_health += damage;
            attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health; // fix?

            if (targetable[rand_index].unitOnTile.current_attack > 10)
            {
                float percenthealth = targetable[rand_index].unitOnTile.current_health / targetable[rand_index].unitOnTile.health;
                targetable[rand_index].unitOnTile.current_attack *= percenthealth;
            }

            Debug.Log("he healed");
            if (targetable[rand_index].unitOnTile.current_health > targetable[rand_index].unitOnTile.health)
            {
                targetable[rand_index].unitOnTile.current_health = targetable[rand_index].unitOnTile.health;
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
                StartCoroutine(targetable[rand_index].unitOnTile.Hit());
            }
        }
    }
}
