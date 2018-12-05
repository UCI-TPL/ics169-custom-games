using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour {
    List<string[]> unit_stats = new List<string[]>();

	// Use this for initialization
	void Awake () {
        TextAsset stats = Resources.Load<TextAsset>("UnitStats");
        string[] row_values = stats.text.Split(new char[] { '\n' });
        for(int i=1; i < row_values.Length; i++)
        {
            string[] unit_values = row_values[i].Split(new char[] { ',' });// creates a list of strings by splitting the row by commas
            unit_stats.Add(unit_values);//adds the items from the split rows into the global array (unit_stats)
        }

        //if (this.gameObject.GetComponent<StartUnit>().unit_ID == 1)
        //{//Tank
        int.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][1], out this.gameObject.GetComponent<StartUnit>().mobility);
        int.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][2], out this.gameObject.GetComponent<StartUnit>().attackRange);
        float.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][3], out this.gameObject.GetComponent<StartUnit>().health);
        float.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][4], out this.gameObject.GetComponent<StartUnit>().attack);
        float.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][4], out this.gameObject.GetComponent<StartUnit>().current_attack);
        int.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][5], out this.gameObject.GetComponent<StartUnit>().basedmg);
        float.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][6], out this.gameObject.GetComponent<StartUnit>().crit);
        float.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][7], out this.gameObject.GetComponent<StartUnit>().miss);
        float.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][8], out this.gameObject.GetComponent<StartUnit>().crit_multiplier);
        int.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][9], out this.gameObject.GetComponent<StartUnit>().cost);
        int.TryParse(unit_stats[this.gameObject.GetComponent<StartUnit>().unit_ID - 1][10], out this.gameObject.GetComponent<StartUnit>().weight);
        //}
        //if (this.gameObject.GetComponent<StartUnit>().unit_ID == 2)
        //{//Melee
        //    int.TryParse(unit_stats[1][1], out this.gameObject.GetComponent<StartUnit>().mobility);
        //    int.TryParse(unit_stats[1][2], out this.gameObject.GetComponent<StartUnit>().attackRange);
        //    float.TryParse(unit_stats[1][3], out this.gameObject.GetComponent<StartUnit>().health);
        //    int.TryParse(unit_stats[1][4], out this.gameObject.GetComponent<StartUnit>().attack);
        //    int.TryParse(unit_stats[1][5], out this.gameObject.GetComponent<StartUnit>().basedmg);
        //    float.TryParse(unit_stats[1][6], out this.gameObject.GetComponent<StartUnit>().crit);
        //    float.TryParse(unit_stats[1][7], out this.gameObject.GetComponent<StartUnit>().miss);
        //    float.TryParse(unit_stats[1][8], out this.gameObject.GetComponent<StartUnit>().crit_multiplier);
        //    int.TryParse(unit_stats[1][9], out this.gameObject.GetComponent<StartUnit>().cost);
        //}
        //if (this.gameObject.GetComponent<StartUnit>().unit_ID == 3)
        //{//Ranger
        //    int.TryParse(unit_stats[2][1], out this.gameObject.GetComponent<StartUnit>().mobility);
        //    int.TryParse(unit_stats[2][2], out this.gameObject.GetComponent<StartUnit>().attackRange);
        //    float.TryParse(unit_stats[2][3], out this.gameObject.GetComponent<StartUnit>().health);
        //    int.TryParse(unit_stats[2][4], out this.gameObject.GetComponent<StartUnit>().attack);
        //    int.TryParse(unit_stats[2][5], out this.gameObject.GetComponent<StartUnit>().basedmg);
        //    float.TryParse(unit_stats[2][6], out this.gameObject.GetComponent<StartUnit>().crit);
        //    float.TryParse(unit_stats[2][7], out this.gameObject.GetComponent<StartUnit>().miss);
        //    float.TryParse(unit_stats[2][8], out this.gameObject.GetComponent<StartUnit>().crit_multiplier);
        //    int.TryParse(unit_stats[2][9], out this.gameObject.GetComponent<StartUnit>().cost);
        //}

    }
}
