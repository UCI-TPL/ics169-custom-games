using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_UI_Item{
    public Sprite Icon;
    public string Buff_ID;

    public Buff_UI_Item(Sprite icon, string buff_ID)
    {
        Icon = icon;
        Buff_ID = buff_ID;
    }
}
