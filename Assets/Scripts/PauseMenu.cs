using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    public GameObject UI_P1_Sel;
    public GameObject UI_P2_Sel;
    public GameObject UI_P1_Portraits;
    public GameObject UI_P2_Portraits;
    public GameObject UI_Turn;
    public GameObject pause_menu;

    public bool pause_on = false;
    public float next_time = 0.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton("J1 Start Button") && next_time < Time.time)
        {
            if(pause_on) // pause on, set everything back on
            {
                next_time += 1f;
                Time.timeScale = 1.0f;
                pause_menu.SetActive(false);
                pause_on = false;
            }
            else // pause off, freeze everything
            {
                next_time += 1f;
                Time.timeScale = 0.0f;
                pause_menu.SetActive(true);
                pause_on = true;
            }
        }
	}
}
