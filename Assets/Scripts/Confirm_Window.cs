using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Confirm_Window : MonoBehaviour {
    public GameObject Conf_Button;
    public GameObject No_Button;
    private GameObject Editor_Obj;
    private HexagonMapEditor Editor;
    public GameObject cursor;
    public EventSystem event_sys;
    private string currently_in_control;
    private string currently_selected;
    //private 

    // Use this for initialization
    void Start () {
        Debug.Log("Shit Bird-------------------------");
        Editor_Obj = GameObject.Find("Editor");
        Editor = Editor_Obj.GetComponent<HexagonMapEditor>();
        //GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(Conf_Button);
        currently_selected = "YES";
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (currently_in_control.Equals("P1"))
        {
            if (Input.GetButtonDown(Editor.PlayerInfo.player1 + "A Button"))
            {
                Confirm();
            }else if (Input.GetButtonDown(Editor.PlayerInfo.player1 + "B Button"))
            {
                Undo();
            }
        }
        else if (currently_in_control.Equals("P2"))
        {
            if (Input.GetButtonDown(Editor.PlayerInfo.player2 + "A Button"))
            {
                Confirm();
            }else if (Input.GetButtonDown(Editor.PlayerInfo.player2 + "B Button"))
            {
                Undo();
            }
        }
        
	}

    public void Activate_Conf_Win()
    {
        //Disable Cursor
        //Editor.cursor.gameObject.SetActive(false);
        cursor.SetActive(false);
        //Show Window
        
        currently_selected = "YES";

        if (Editor.PlayerInfo.one_player)
        {
            //keep player one in control
            currently_in_control = "P1";
            
        }
        else
        {
            if (Editor.currentState == HexagonMapEditor.TurnStates.P1_MOVE)
            {
                //Make player one have control
                currently_in_control = "P1";
            }
            else if (Editor.currentState == HexagonMapEditor.TurnStates.P2_MOVE)
            {
                //Make player two have control
                currently_in_control = "P2";
            }
        }

        gameObject.SetActive(true);
        //GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(Conf_Button);
    }

    public void Hide_Conf_Win()
    {
        //Editor.cursor.gameObject.SetActive(true);
        cursor.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        Debug.Log("--------------------- Confirmation Pressed");
        Editor.move_string = "yes";
        Editor.move_confirmed = true;
        Hide_Conf_Win();
    }

    public void Undo()
    {
        Debug.Log("--------------------- Undo Pressed");
        Editor.move_string = "no";
        Editor.move_confirmed = true;
        Hide_Conf_Win();
    }
}
