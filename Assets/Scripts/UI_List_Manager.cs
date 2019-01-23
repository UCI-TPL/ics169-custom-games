using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_List_Manager : MonoBehaviour {
    public Sprite[] IconImages;
    public GameObject UI_ListPanel;
    public GameObject ListItemPrefab;
    public GameObject Editor_Obj;
    private HexagonMapEditor Editor;

    ArrayList ListItems;

    void Start()
    {
        Editor = Editor_Obj.GetComponent<HexagonMapEditor>();
        // 1. Get the data to be displayed
        ListItems = new ArrayList(){
            new UI_List_Item(IconImages[0],
                "Select Unit"),
            new UI_List_Item(IconImages[1],
                "Deselect Unit"),
            new UI_List_Item(IconImages[2],
                "Pause Game"),
            new UI_List_Item(IconImages[0],
                "Move Unit"),
            new UI_List_Item(IconImages[3],
                "Cycle Through Units")
        };

        // 2. Iterate through the data, 
        //	  instantiate prefab, 
        //	  set the data, 
        //	  add it to panel
        foreach (UI_List_Item list_item in ListItems)
        {
            GameObject newListItem = Instantiate(ListItemPrefab) as GameObject;
            UI_List controller = newListItem.GetComponent<UI_List>();
            controller.Icon_Obj.GetComponent<Image>().sprite = list_item.Icon;
            controller.Con_Text_Obj.GetComponent<Text>().text = list_item.Con_Text;
            newListItem.transform.parent = UI_ListPanel.transform;
            newListItem.transform.localScale = Vector3.one;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            remove_current_controls();
        }
        if (Input.GetKeyDown("s"))
        {
            update_current_controls();
        }

    }

    public void remove_current_controls()
    {
        for (int i = 0; i < UI_ListPanel.transform.childCount; i++)
        {
            Transform to_delete = UI_ListPanel.transform.GetChild(i);
            if(to_delete.gameObject != UI_ListPanel.gameObject)
            {
                Destroy(to_delete.gameObject);
            }
        }
    }
    
    public void populate_current_controls()
    {
        ListItems = new ArrayList();
        if (Editor.isUnitSelected)
        {
            if(Editor.MoveableUnits.Contains(Editor.SelectedUnit)){
                ListItems.Add(new UI_List_Item(IconImages[0],
                "Move Unit"));
            }
            
            ListItems.Add(new UI_List_Item(IconImages[1],
                "Deselect Unit"));
            ListItems.Add(new UI_List_Item(IconImages[3],
                "Cycle Through Units"));
            ListItems.Add(new UI_List_Item(IconImages[2],
                "Pause Game"));
        }
        else
        {
            ListItems.Add(new UI_List_Item(IconImages[0],
                "Select Unit"));
            ListItems.Add(new UI_List_Item(IconImages[3],
                "Cycle Through Units"));
            ListItems.Add(new UI_List_Item(IconImages[2],
                "Pause Game"));
        }

        foreach (UI_List_Item list_item in ListItems)
        {
            GameObject newListItem = Instantiate(ListItemPrefab) as GameObject;
            UI_List controller = newListItem.GetComponent<UI_List>();
            controller.Icon_Obj.GetComponent<Image>().sprite = list_item.Icon;
            controller.Con_Text_Obj.GetComponent<Text>().text = list_item.Con_Text;
            newListItem.transform.parent = UI_ListPanel.transform;
            newListItem.transform.localScale = Vector3.one;
        }
    }

    public void update_current_controls()
    {
        remove_current_controls();
        populate_current_controls();
    }
}
