using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroUnit : StartUnit {
    public enum BuffType
    {
        OneTime,
        EveryTurn,
        BasicAttack
    }
    public BuffType myType;

    public virtual void Awake()
    {
        base.Start();
        
    }

    public virtual void BuffTeam(string team, HexagonCell myCell)
    {
        if (team == "P1")
        {
            for (int i = 1; i < editor.P1Team.Count; i++)
            {
                Debug.Log("buffing unit");
                Buff(editor.P1Team[i]);
            }
        }
        else
        {
            for(int i = 1; i < editor.P2Team.Count; i++)
            {
                Buff(editor.P2Team[i]);
            }
        }
    }

    public virtual void DebufTeam(string team, HexagonCell myCell)
    {
        if (team == "P1")
        {
            for (int i = 1; i < editor.P1Team.Count; i++)
            {
                Debug.Log("Debufing unit");
                Debuf(editor.P1Team[i]);
            }
        }
        else
        {
            for (int i = 1; i < editor.P2Team.Count; i++)
            {
               Debuf(editor.P2Team[i]);
            }
        }
    }


    public virtual void Buff(StartUnit unit)
    {
        //if (unit.current_attack != unit.attack)
        //{
        //    unit.current_attack += 1;
        //}
        //else
        //{
        //    unit.attack += 1;
        //    unit.current_attack += 1;
        //}
        unit.current_attack += 1;

    }

    public virtual void Debuf(StartUnit unit)
    {
        unit.current_attack -= 1;
    }
    

}
