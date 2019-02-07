using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuScroll : MonoBehaviour {
    public List<GameObject> scrollItems;
    public int currentItem;
    public GameObject controlsMenu, maincanvas, FirstObject,B_button;
	// Use this for initialization
	void Start () {
        currentItem = 0;
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("J1 A Button") || Input.GetButtonDown("J2 A Button"))
        {
            if (currentItem == scrollItems.Count - 1)
            {
                currentItem = 0;
                B_button.SetActive(false);
            }
            else
            {
                currentItem++;
                if (currentItem != 0)
                    B_button.SetActive(true);
                else
                {
                    B_button.SetActive(false);
                }
            }
            for (int i = 0; i < scrollItems.Count; i++)
            {
                if (i != currentItem)
                {
                    scrollItems[i].SetActive(false);
                }
                else
                {
                    scrollItems[i].SetActive(true);
                }
            }
        }

        if (Input.GetButtonDown("J1 B Button") || Input.GetButtonDown("J2 B Button"))
        {
            if(currentItem == 0 || currentItem == scrollItems.Count-1)
            {
                controlsMenu.SetActive(false);
                maincanvas.SetActive(true);
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
            }

            else
            {
                currentItem--;
                if (currentItem != 0)
                {
                    B_button.SetActive(true);
                }
                else
                {
                    B_button.SetActive(false);
                }
                for(int i = 0; i < scrollItems.Count; i++)
                {
                    if( i != currentItem)
                    {
                        scrollItems[i].SetActive(false);
                    }
                    else
                    {
                        scrollItems[i].SetActive(true);
                    }
                }
            }

        }
	}
}
