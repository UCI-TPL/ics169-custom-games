using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllsMenu : MonoBehaviour {
    public GameObject controls_menu,pause_menu,editor,cursor;
    public GameObject FirstSelect;
    public void Update()
    {
        GoBack();
    }
    public void GoBack()
    {
        if(controls_menu.activeInHierarchy && (Input.GetButtonDown("J1 B Button") || Input.GetButtonDown("J2 B Button")))
        {
            pause_menu.gameObject.SetActive(true);
            controls_menu.gameObject.SetActive(false);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstSelect);
        }
        else if(pause_menu.activeInHierarchy && (Input.GetButtonDown("J1 B Button") || Input.GetButtonDown("J2 B Button")))
        {
            pause_menu.gameObject.SetActive(false);
            editor.SetActive(true);
            cursor.SetActive(true);
            Time.timeScale = 1;
        }
    }
}
