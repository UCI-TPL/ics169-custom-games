using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortressHero : HeroUnit {

    public List<StartUnit> wasBuffed = new List<StartUnit>();


    public override void BuffTeam(string team, HexagonCell myCell) // every unit that is in the team, if they are next to the hero, buff them
    {
        if (team == "P1")
        {
            for (int i = 1; i < editor.P1Team.Count; i++) // for every unit
            {
                for (HexagonDirection d = HexagonDirection.NE; d <= HexagonDirection.NW; d++) // for every neighboring cell
                {

                    HexagonCell neighbor = myCell.GetNeighbor(d);
                    if (neighbor.unitOnTile == editor.P1Team[i]) // if the unit is next to the hero
                    {
                        Buff(editor.P1Team[i]);
                        wasBuffed.Add(editor.P1Team[i]);
                        defense += 5;
                    }
                }
            }
        }
        else
        {
            for (int i = 1; i < editor.P2Team.Count; i++) // for every unit
            {
                for (HexagonDirection d = HexagonDirection.NE; d <= HexagonDirection.NW; d++) // for every neighboring cell
                {

                    HexagonCell neighbor = myCell.GetNeighbor(d);
                    //There is some kind of BUG happening here when a unit dies next to a fortess hero
                    if (neighbor.unitOnTile == editor.P2Team[i]) // if the unit is next to the hero
                    {
                        Buff(editor.P2Team[i]);
                        wasBuffed.Add(editor.P2Team[i]);
                        defense += 5;
                    }
                }
            }
        }
    }

    public override void DebufTeam(string team, HexagonCell myCell) // every unit that was buffed gets debuffed
    {

        for (int i = 0; i < wasBuffed.Count; i++) // for every unit that was buffed
        {
            if (wasBuffed[i] != null)
            {
                Debuf(wasBuffed[i]); //debuf them
            }
            defense -= 5;

        }
        wasBuffed.Clear(); //clear the list

    }

    public override void Buff(StartUnit unit)
    {
        Debug.Log(unit.unit_name + " was buffed with +15 defense");
        unit.defense += 15;
        unit.fortress_def_buff = true;
        unit.Shield_Bubble.SetActive(true);
        //activate buff ui element here
        unit.gameObject.GetComponentInChildren<Buff_UI_Manager>().update_current_buffs(unit);
    }

    public override void Debuf(StartUnit unit)
    {
        Debug.Log(unit.unit_name + " lost its buff of +15 defense");
        unit.defense -= 15;
        unit.fortress_def_buff = false;
        unit.Shield_Bubble.SetActive(false);
        //deactivate buff ui element here
        unit.gameObject.GetComponentInChildren<Buff_UI_Manager>().update_current_buffs(unit);
    }
}
