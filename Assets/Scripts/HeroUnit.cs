using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroUnit : StartUnit {

    public void Awake()
    {
        base.Start();
        
    }

    public virtual void BuffTeam(string team)
    {
        if (team == "P1")
        {
            for (int i = 1; i < editor.P1Team.Count; i++)
            {
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

    public virtual void Buff(StartUnit unit)
    {
        unit.attack += 1;
    }
    

}
