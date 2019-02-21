using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff_UI_Manager : MonoBehaviour {
    public Sprite[] IconImages;
    public GameObject UI_ListPanel;
    public GameObject BuffItemPrefab;
    public GameObject Unit_To_Test;

    ArrayList ListItems;

    void Start()
    {
        // 1. Get the data to be displayed
        //should start empty
        ListItems = new ArrayList();
        IconImages = new Sprite[] {
            Resources.Load<Sprite>("Fortress_Buff_Icon"),
            Resources.Load<Sprite>("Attack_Buff_Icon_ReScaled"),
            Resources.Load<Sprite>("Health_Buff_Icon_ReScaled"),
            Resources.Load<Sprite>("Crit_Buff_Icon_ReScaled"),
            Resources.Load<Sprite>("Move_Buff_Icon_ReScaled")
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            remove_current_buffs();
        }
        if (Input.GetKeyDown("s"))
        {
            update_current_buffs(Unit_To_Test.GetComponent<StartUnit>());
        }

    }

    public void remove_current_buffs()
    {
        for (int i = 0; i < UI_ListPanel.transform.childCount; i++)
        {
            Transform to_delete = UI_ListPanel.transform.GetChild(i);
            if (to_delete.gameObject != UI_ListPanel.gameObject)
            {
                Destroy(to_delete.gameObject);
            }
        }
    }

    public void populate_current_buffs(StartUnit Unit_To_Buff)
    {
        ListItems = new ArrayList();
        //is unit currently selected

        if (Unit_To_Buff.fortress_def_buff)
        {
            ListItems.Add(new Buff_UI_Item(IconImages[0], "Fortress Buff"));
        }
        if (Unit_To_Buff.attack_buff)
        {
            ListItems.Add(new Buff_UI_Item(IconImages[1], "Attack Buff"));
        }
        if (Unit_To_Buff.health_buff)
        {
            ListItems.Add(new Buff_UI_Item(IconImages[2], "Health Buff"));
        }
        if (Unit_To_Buff.crit_buff)
        {
            ListItems.Add(new Buff_UI_Item(IconImages[3], "Crit Buff"));
        }
        if (Unit_To_Buff.move_buff)
        {
            ListItems.Add(new Buff_UI_Item(IconImages[4], "Move Buff"));
        }


        float num_of_items = ListItems.Count;
        float curr_num_counter = 0;
        float desired_width_ratio = 0.2f;
        foreach (Buff_UI_Item buff_item in ListItems)
        {
            GameObject newBuffItem = Instantiate(BuffItemPrefab) as GameObject;
            Buff_UI_Obj controller = newBuffItem.GetComponent<Buff_UI_Obj>();
            controller.Icon_Obj.GetComponent<Image>().sprite = buff_item.Icon;
            controller.Buff_ID = buff_item.Buff_ID;
            newBuffItem.transform.SetParent(UI_ListPanel.transform);
            newBuffItem.transform.localScale = Vector3.one;
            float max_x_anchor = 0 + ((curr_num_counter + 1) * desired_width_ratio);
            float min_x_anchor = 0 + (curr_num_counter * desired_width_ratio);
            newBuffItem.GetComponent<RectTransform>().anchorMax = new Vector2(max_x_anchor, 1);
            newBuffItem.GetComponent<RectTransform>().anchorMin = new Vector2(min_x_anchor, 0);
            newBuffItem.GetComponent<RectTransform>().sizeDelta = UI_ListPanel.GetComponent<RectTransform>().rect.size;
            newBuffItem.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            newBuffItem.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            curr_num_counter += 1;
        }
    }

    public void update_current_buffs(StartUnit Unit_To_Buffdate)
    {
        remove_current_buffs();
        populate_current_buffs(Unit_To_Buffdate);
    }
}
