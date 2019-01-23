using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    public Transform pause_menu;
    public Transform editor;
    public Transform cursor;
    public Transform controls_menu;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("J1 Start Button") || Input.GetButtonDown("J2 Start Button"))
        {
            if(pause_menu.gameObject.activeInHierarchy == false)
            {
                editor.gameObject.SetActive(false);
                cursor.gameObject.SetActive(false);
                pause_menu.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                editor.gameObject.SetActive(true);
                cursor.gameObject.SetActive(true);
                pause_menu.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
        //GotoControls();
    }

    public void GotoControls()
    {
        if (Input.GetButtonDown("J1 X Button") || Input.GetButtonDown("J2 X Button"))
        {
            pause_menu.gameObject.SetActive(false);
            controls_menu.gameObject.SetActive(true);
        }
    }
}
