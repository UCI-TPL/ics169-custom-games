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
    public GameObject cursor_obj;
    private Cursor cursor;
    public GameObject Grid_obj;
    private Grid _Grid;

    ArrayList ListItems;

    void Start()
    {
        Editor = Editor_Obj.GetComponent<HexagonMapEditor>();
        cursor = cursor_obj.GetComponent<Cursor>();
        _Grid = Grid_obj.GetComponent<Grid>();
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
        float num_of_items = ListItems.Count;
        float curr_num_counter = 0;
        float desired_width_ratio = 0.1666f; //one eighth of the total size
        foreach (UI_List_Item list_item in ListItems)
        {
            GameObject newListItem = Instantiate(ListItemPrefab) as GameObject;
            UI_List controller = newListItem.GetComponent<UI_List>();
            controller.Icon_Obj.GetComponent<Image>().sprite = list_item.Icon;
            controller.Con_Text_Obj.GetComponent<Text>().text = list_item.Con_Text;
            newListItem.transform.SetParent(UI_ListPanel.transform,false);
            newListItem.transform.localScale = Vector3.one;
            //max x and min x should always be 1 and 0 respectively, but min and max y should change
            float max_y_anchor = 1 - (curr_num_counter * desired_width_ratio);
            float min_y_anchor = 1 - ((curr_num_counter + 1) * desired_width_ratio);
            newListItem.GetComponent<RectTransform>().anchorMax = new Vector2(1, max_y_anchor);
            newListItem.GetComponent<RectTransform>().anchorMin = new Vector2(0, min_y_anchor);
            newListItem.GetComponent<RectTransform>().sizeDelta = UI_ListPanel.GetComponent<RectTransform>().rect.size;
            newListItem.GetComponent<RectTransform>().offsetMax = new Vector2(0,0);
            newListItem.GetComponent<RectTransform>().offsetMin = new Vector2(0,0);
            curr_num_counter += 1;
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
        //is unit currently selected
        if (Editor.isUnitSelected)
        {
            
            //Check if cursor over other unit or over non-occupied tile
            if(_Grid.Get_Cell_Index(cursor.coords).unitOnTile != null && _Grid.Get_Cell_Index(cursor.coords).unitOnTile != Editor.SelectedUnit 
                && Editor.MoveableUnits.Contains(_Grid.Get_Cell_Index(cursor.coords).unitOnTile))
            {
                ListItems.Add(new UI_List_Item(IconImages[0], "Select Unit"));
            }
            else if(_Grid.Get_Cell_Index(cursor.coords).unitOnTile == null || _Grid.Get_Cell_Index(cursor.coords).unitOnTile == Editor.SelectedUnit)
            {
                ListItems.Add(new UI_List_Item(IconImages[0], "Move Unit"));
            }
            
            ListItems.Add(new UI_List_Item(IconImages[1],"Deselect Unit"));
            ListItems.Add(new UI_List_Item(IconImages[3],"Cycle Through Units"));
            ListItems.Add(new UI_List_Item(IconImages[2],"Pause Game"));
        }
        else
        {
            if(_Grid.Get_Cell_Index(cursor.coords).unitOnTile != null && Editor.MoveableUnits.Contains(_Grid.Get_Cell_Index(cursor.coords).unitOnTile))
            {
                ListItems.Add(new UI_List_Item(IconImages[0],"Select Unit"));
            }
            
            ListItems.Add(new UI_List_Item(IconImages[3],"Cycle Through Units"));
            ListItems.Add(new UI_List_Item(IconImages[2],"Pause Game"));
        }

        float num_of_items = ListItems.Count;
        float curr_num_counter = 0;
        float desired_width_ratio = 0.1666f;
        foreach (UI_List_Item list_item in ListItems)
        {
            GameObject newListItem = Instantiate(ListItemPrefab) as GameObject;
            UI_List controller = newListItem.GetComponent<UI_List>();
            controller.Icon_Obj.GetComponent<Image>().sprite = list_item.Icon;
            controller.Con_Text_Obj.GetComponent<Text>().text = list_item.Con_Text;
            newListItem.transform.SetParent(UI_ListPanel.transform);
            newListItem.transform.localScale = Vector3.one;
            float max_y_anchor = 1 - (curr_num_counter * desired_width_ratio);
            float min_y_anchor = 1 - ((curr_num_counter + 1) * desired_width_ratio);
            newListItem.GetComponent<RectTransform>().anchorMax = new Vector2(1, max_y_anchor);
            newListItem.GetComponent<RectTransform>().anchorMin = new Vector2(0, min_y_anchor);
            newListItem.GetComponent<RectTransform>().sizeDelta = UI_ListPanel.GetComponent<RectTransform>().rect.size;
            newListItem.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            newListItem.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            curr_num_counter += 1;
        }
    }

    public void update_current_controls()
    {
        remove_current_controls();
        populate_current_controls();
    }
}
