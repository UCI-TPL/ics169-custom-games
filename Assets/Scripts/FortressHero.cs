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
                    if (neighbor.unitOnTile == editor.P2Team[i]) // if the unit is next to the hero
                    {
                        Buff(editor.P2Team[i]);
                        wasBuffed.Add(editor.P2Team[i]);
                    }
                }
            }
        }
    }

    public override void DebufTeam(string team, HexagonCell myCell) // every unit that was buffed gets debuffed
    {

        for (int i = 0; i < wasBuffed.Count; i++) // for every unit that was buffed
        {
            Debuf(wasBuffed[i]); //debuf them
        }
        wasBuffed.Clear(); //clear the list

    }

    public override void Buff(StartUnit unit)
    {
        unit.defense += 30;
    }

    public override void Debuf(StartUnit unit)
    {
        unit.defense -= 30;
    }
}
