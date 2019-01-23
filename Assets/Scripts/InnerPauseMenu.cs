using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InnerPauseMenu : MonoBehaviour
{
    public GameObject pause_menu, controls_menu,editor,cursor,FirstObject;
    // Update is called once per frame
    public void Update()
    {
        if (Input.GetButtonDown("J1 Start Button") || Input.GetButtonDown("J2 Start Button") || Input.GetButtonDown("J1 B Button") || Input.GetButtonDown("J2 B Button"))
        {
            if (pause_menu.gameObject.activeInHierarchy == false)
            {
                pause_menu.gameObject.SetActive(true);
                editor.gameObject.SetActive(false); //turns off editor so that players cant select units while the pause menu is open
                cursor.gameObject.SetActive(false);// turns off cursor so that it stays in the position before the pause
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
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
    }
}
