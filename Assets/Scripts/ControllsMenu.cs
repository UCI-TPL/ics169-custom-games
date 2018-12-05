using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllsMenu : MonoBehaviour {
    public Canvas controls_menu,pause_menu;

    private void Update()
    {
        GoBack();
    }
    public void GoBack()
    {
        if(Input.GetButtonDown("J1 B Button") || Input.GetButtonDown("J2 B Button"))
        {
            controls_menu.gameObject.SetActive(false);
            pause_menu.gameObject.SetActive(true);
        }
    }
}
