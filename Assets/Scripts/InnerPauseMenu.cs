using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerPauseMenu : MonoBehaviour
{
    public Canvas pausecanvas, controls_menu;
    // Update is called once per frame
    public void Options()
    {
        if (Input.GetButtonDown("J1 X Button") || Input.GetButtonDown("J2 X Button"))
        {
            pausecanvas.gameObject.SetActive(false);
            controls_menu.gameObject.SetActive(true);
        }
    }
}
